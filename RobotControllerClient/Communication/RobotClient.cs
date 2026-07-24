using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RobotControllerClient.Protocol;

namespace RobotControllerClient.Communication
{
    public enum LogDirection
    {
        Sent,
        Received,
        Info,
        Error
    }

    public class CommEventArgs : EventArgs
    {
        public LogDirection Direction { get; private set; }
        public string Text { get; private set; }

        public CommEventArgs(LogDirection direction, string text)
        {
            Direction = direction;
            Text = text;
        }
    }

    public class RobotClient : IDisposable
    {
        private TcpClient _tcp;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private volatile bool _running;
        private readonly object _sendLock = new object();
        private readonly BlockingCollection<string> _inbox = new BlockingCollection<string>();

        public int ResponseTimeoutMs { get; set; }

        public event EventHandler<CommEventArgs> Log;
        public event EventHandler<bool> ConnectionChanged;

        public bool IsConnected
        {
            get { return _tcp != null && _tcp.Connected; }
        }

        public RobotClient()
        {
            ResponseTimeoutMs = 30000;
        }

        public void Connect(string host, int port)
        {
            Disconnect();

            RaiseLog(LogDirection.Info, string.Format("연결 시도: {0}:{1}", host, port));
            _tcp = new TcpClient();
            _tcp.NoDelay = true;
            _tcp.Connect(host, port);
            _stream = _tcp.GetStream();

            _running = true;
            _receiveThread = new Thread(ReceiveLoop) { IsBackground = true, Name = "RobotClientReceive" };
            _receiveThread.Start();

            RaiseLog(LogDirection.Info, string.Format("연결 성공: {0}:{1}", host, port));
            RaiseConnectionChanged(true);
        }

        public void Disconnect()
        {
            bool wasConnected = IsConnected;
            _running = false;

            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                }
            }
            catch { }

            try
            {
                if (_tcp != null)
                {
                    _tcp.Close();
                }
            }
            catch { }

            _stream = null;
            _tcp = null;

            if (_receiveThread != null && _receiveThread.IsAlive && _receiveThread != Thread.CurrentThread)
            {
                _receiveThread.Join(500);
            }
            _receiveThread = null;

            while (_inbox.TryTake(out _)) { }

            if (wasConnected)
            {
                RaiseLog(LogDirection.Info, "연결 해제됨");
                RaiseConnectionChanged(false);
            }
        }

        public void SendRaw(string text)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("연결되어 있지 않습니다.");
            }

            byte[] frame = RobotProtocol.Frame(text);
            lock (_sendLock)
            {
                _stream.Write(frame, 0, frame.Length);
                _stream.Flush();
            }
            RaiseLog(LogDirection.Sent, text);
        }

        public CommandResult SendCommand(string commandText, CancellationToken cancel)
        {
            CommandResult result = new CommandResult { CommandText = commandText };

            if (!IsConnected)
            {
                result.Status = ResultStatus.NotConnected;
                RaiseLog(LogDirection.Error, "전송 실패: 연결되어 있지 않습니다.");
                return result;
            }

            while (_inbox.TryTake(out _)) { }

            SendRaw(commandText);

            DateTime deadline = DateTime.UtcNow.AddMilliseconds(ResponseTimeoutMs);

            while (true)
            {
                if (cancel.IsCancellationRequested)
                {
                    result.Status = ResultStatus.Timeout;
                    return result;
                }

                int remaining = (int)(deadline - DateTime.UtcNow).TotalMilliseconds;
                if (remaining <= 0)
                {
                    result.Status = ResultStatus.Timeout;
                    RaiseLog(LogDirection.Error, string.Format("응답 타임아웃: {0}", commandText));
                    return result;
                }

                string line;
                if (!_inbox.TryTake(out line, Math.Min(remaining, 200)))
                {
                    continue;
                }

                result.Responses.Add(line);

                if (RobotProtocol.IsAck(line))
                {
                    result.Acked = true;
                    continue;
                }

                if (RobotProtocol.IsNak(line))
                {
                    result.Status = ResultStatus.Nak;
                    return result;
                }

                if (RobotProtocol.IsError(line))
                {
                    result.Status = ResultStatus.Error;
                    result.ErrorCode = RobotProtocol.ExtractErrorCode(line);
                    result.ErrorDescription = ErrorCodes.Describe(result.ErrorCode);
                    // Error 회신 이후 준비회신(_RDY)이 뒤따를 수 있으므로 잠시 더 흡수한다.
                    DrainReady(result);
                    return result;
                }

                if (RobotProtocol.IsReady(line))
                {
                    result.Status = ResultStatus.Success;
                    return result;
                }
            }
        }

        private void DrainReady(CommandResult result)
        {
            DateTime until = DateTime.UtcNow.AddMilliseconds(1000);
            while (DateTime.UtcNow < until)
            {
                string line;
                int remaining = (int)(until - DateTime.UtcNow).TotalMilliseconds;
                if (remaining <= 0)
                {
                    break;
                }
                if (_inbox.TryTake(out line, Math.Min(remaining, 100)))
                {
                    result.Responses.Add(line);
                    if (RobotProtocol.IsReady(line))
                    {
                        break;
                    }
                }
            }
        }

        private void ReceiveLoop()
        {
            byte[] buffer = new byte[4096];
            StringBuilder pending = new StringBuilder();

            try
            {
                while (_running)
                {
                    int read = _stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        break;
                    }

                    for (int i = 0; i < read; i++)
                    {
                        byte b = buffer[i];
                        if (b == RobotProtocol.CR)
                        {
                            EmitLine(pending);
                        }
                        else if (b == 0x0A)
                        {
                            continue;
                        }
                        else
                        {
                            pending.Append((char)b);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_running)
                {
                    RaiseLog(LogDirection.Error, "수신 오류: " + ex.Message);
                }
            }
            finally
            {
                if (_running)
                {
                    _running = false;
                    RaiseLog(LogDirection.Info, "연결이 원격에서 종료되었습니다.");
                    RaiseConnectionChanged(false);
                }
            }
        }

        private void EmitLine(StringBuilder pending)
        {
            string line = pending.ToString().Trim();
            pending.Clear();
            if (line.Length == 0)
            {
                return;
            }
            RaiseLog(LogDirection.Received, line);
            _inbox.Add(line);
        }

        private void RaiseLog(LogDirection dir, string text)
        {
            EventHandler<CommEventArgs> handler = Log;
            if (handler != null)
            {
                handler(this, new CommEventArgs(dir, text));
            }
        }

        private void RaiseConnectionChanged(bool connected)
        {
            EventHandler<bool> handler = ConnectionChanged;
            if (handler != null)
            {
                handler(this, connected);
            }
        }

        public void Dispose()
        {
            Disconnect();
            _inbox.Dispose();
        }
    }
}

using System;
using System.IO;
using System.Text;
using RobotControllerClient.Communication;

namespace RobotControllerClient.Logging
{
    public class CommLogger : IDisposable
    {
        private readonly object _fileLock = new object();
        private readonly string _logDirectory;
        private StreamWriter _writer;
        private string _currentDate;

        public string LogDirectory
        {
            get { return _logDirectory; }
        }

        public CommLogger(string logDirectory)
        {
            _logDirectory = logDirectory;
            Directory.CreateDirectory(_logDirectory);
        }

        public string Write(LogDirection direction, string text)
        {
            DateTime now = DateTime.Now;
            string line = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] {1} {2}",
                now, Tag(direction), text);

            lock (_fileLock)
            {
                EnsureWriter(now);
                _writer.WriteLine(line);
                _writer.Flush();
            }

            return line;
        }

        private void EnsureWriter(DateTime now)
        {
            string date = now.ToString("yyyy-MM-dd");
            if (_writer != null && _currentDate == date)
            {
                return;
            }

            CloseWriter();
            string path = Path.Combine(_logDirectory, date + ".log");
            _writer = new StreamWriter(path, true, new UTF8Encoding(true));
            _currentDate = date;
        }

        private static string Tag(LogDirection direction)
        {
            switch (direction)
            {
                case LogDirection.Sent:
                    return "[SEND]";
                case LogDirection.Received:
                    return "[RECV]";
                case LogDirection.Error:
                    return "[ERR ]";
                default:
                    return "[INFO]";
            }
        }

        private void CloseWriter()
        {
            if (_writer != null)
            {
                try { _writer.Flush(); _writer.Dispose(); } catch { }
                _writer = null;
            }
        }

        public void Dispose()
        {
            lock (_fileLock)
            {
                CloseWriter();
            }
        }
    }
}

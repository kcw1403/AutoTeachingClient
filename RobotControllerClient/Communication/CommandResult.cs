using System.Collections.Generic;

namespace RobotControllerClient.Communication
{
    public enum ResultStatus
    {
        Success,
        Nak,
        Error,
        Timeout,
        NotConnected
    }

    public class CommandResult
    {
        public string CommandText { get; set; }
        public ResultStatus Status { get; set; }
        public bool Acked { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public List<string> Responses { get; set; }

        public CommandResult()
        {
            Responses = new List<string>();
        }

        public bool IsSuccess
        {
            get { return Status == ResultStatus.Success; }
        }

        public string DataResponse
        {
            get
            {
                for (int i = Responses.Count - 1; i >= 0; i--)
                {
                    string line = Responses[i];
                    if (line == Protocol.RobotProtocol.Ack ||
                        line == Protocol.RobotProtocol.Nak ||
                        Protocol.RobotProtocol.IsReady(line) ||
                        Protocol.RobotProtocol.IsError(line))
                    {
                        continue;
                    }
                    return line;
                }
                return null;
            }
        }
    }
}

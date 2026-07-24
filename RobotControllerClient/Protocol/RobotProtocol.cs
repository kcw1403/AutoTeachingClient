using System.Text;

namespace RobotControllerClient.Protocol
{
    public static class RobotProtocol
    {
        public const byte CR = 0x0D;

        public const string Ack = "_ACK";
        public const string Nak = "_NAK";
        public const string Ready = "_RDY";
        public const string ReadyAligner = "_RDY_ALIGNER";
        public const string ErrorPrefix = "_ERR";

        public static readonly Encoding Encoding = Encoding.ASCII;

        public static byte[] Frame(string command)
        {
            string trimmed = (command ?? string.Empty).TrimEnd('\r', '\n');
            byte[] body = Encoding.GetBytes(trimmed);
            byte[] framed = new byte[body.Length + 1];
            System.Array.Copy(body, framed, body.Length);
            framed[body.Length] = CR;
            return framed;
        }

        public static bool IsAck(string line)
        {
            return line == Ack;
        }

        public static bool IsNak(string line)
        {
            return line == Nak;
        }

        public static bool IsReady(string line)
        {
            return line == Ready || line == ReadyAligner;
        }

        public static bool IsError(string line)
        {
            return line != null && line.StartsWith(ErrorPrefix);
        }

        public static string ExtractErrorCode(string line)
        {
            if (!IsError(line))
            {
                return null;
            }

            string rest = line.Substring(ErrorPrefix.Length).Trim();
            return rest.Length == 0 ? null : rest;
        }
    }
}

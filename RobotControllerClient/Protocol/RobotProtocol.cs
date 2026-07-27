using System.Text;

namespace RobotControllerClient.Protocol
{
    public enum LineTerminator
    {
        Cr,
        Lf,
        CrLf
    }

    public static class RobotProtocol
    {
        public const byte CR = 0x0D;
        public const byte LF = 0x0A;

        public const string Ack = "_ACK";
        public const string Nak = "_NAK";
        public const string Ready = "_RDY";
        public const string ReadyAligner = "_RDY_ALIGNER";
        public const string ErrorPrefix = "_ERR";

        public static readonly Encoding Encoding = Encoding.ASCII;

        public static byte[] Frame(string command)
        {
            return Frame(command, LineTerminator.Cr);
        }

        public static byte[] Frame(string command, LineTerminator terminator)
        {
            string trimmed = (command ?? string.Empty).TrimEnd('\r', '\n');
            byte[] body = Encoding.GetBytes(trimmed);
            byte[] suffix = TerminatorBytes(terminator);

            byte[] framed = new byte[body.Length + suffix.Length];
            System.Array.Copy(body, framed, body.Length);
            System.Array.Copy(suffix, 0, framed, body.Length, suffix.Length);
            return framed;
        }

        public static byte[] TerminatorBytes(LineTerminator terminator)
        {
            switch (terminator)
            {
                case LineTerminator.Lf:
                    return new[] { LF };
                case LineTerminator.CrLf:
                    return new[] { CR, LF };
                default:
                    return new[] { CR };
            }
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

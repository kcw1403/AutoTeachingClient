using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RobotControllerClient.Cycle
{
    public static class CycleFile
    {
        public const string Extension = ".cyc";
        public const string FileDialogFilter = "사이클 파일 (*.cyc)|*.cyc|모든 파일 (*.*)|*.*";

        private const string Header = "#RobotCycle v1";
        private const char Separator = '\t';

        public static void Save(string path, IList<CycleStep> steps)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Header);
            foreach (CycleStep step in steps)
            {
                sb.Append(Escape(step.CommandId)).Append(Separator);
                sb.Append(Escape(step.CommandText)).Append(Separator);
                sb.Append(step.DelayAfterMs.ToString(CultureInfo.InvariantCulture)).Append(Separator);
                sb.Append(step.IsDelay ? "1" : "0");
                sb.Append('\n');
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        public static List<CycleStep> Load(string path)
        {
            List<CycleStep> steps = new List<CycleStep>();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);

            foreach (string raw in lines)
            {
                string line = raw.TrimEnd('\r', '\n');
                if (line.Length == 0 || line.StartsWith("#"))
                {
                    continue;
                }

                string[] parts = line.Split(Separator);
                if (parts.Length < 4)
                {
                    continue;
                }

                int delay;
                if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out delay))
                {
                    delay = 0;
                }
                bool isDelay = parts[3] == "1";

                steps.Add(new CycleStep(Unescape(parts[0]), Unescape(parts[1]), delay, isDelay));
            }

            return steps;
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        private static string Unescape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '\\' && i + 1 < value.Length)
                {
                    char next = value[++i];
                    switch (next)
                    {
                        case 't': sb.Append('\t'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case '\\': sb.Append('\\'); break;
                        default: sb.Append(next); break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}

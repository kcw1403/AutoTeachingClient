using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RobotControllerClient.Protocol
{
    public class TeachDiffStore
    {
        private const string Header = "#TeachDiff v1";
        private const char Separator = '\t';

        private readonly string _path;
        private readonly List<TeachDiffRecord> _records = new List<TeachDiffRecord>();
        private readonly object _lock = new object();

        public TeachDiffStore(string path)
        {
            _path = path;
            Load();
        }

        public string FilePath { get { return _path; } }

        public void Add(TeachDiffRecord record)
        {
            if (record == null)
            {
                return;
            }

            lock (_lock)
            {
                _records.Add(record);
                AppendToFile(record);
            }
        }

        public List<TeachDiffRecord> All()
        {
            lock (_lock)
            {
                return new List<TeachDiffRecord>(_records);
            }
        }

        public List<TeachDiffRecord> ForStation(int station)
        {
            lock (_lock)
            {
                List<TeachDiffRecord> result = new List<TeachDiffRecord>();
                foreach (TeachDiffRecord r in _records)
                {
                    if (r.Station == station)
                    {
                        result.Add(r);
                    }
                }
                return result;
            }
        }

        public List<int> Stations()
        {
            lock (_lock)
            {
                List<int> stations = new List<int>();
                foreach (TeachDiffRecord r in _records)
                {
                    if (!stations.Contains(r.Station))
                    {
                        stations.Add(r.Station);
                    }
                }
                stations.Sort();
                return stations;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _records.Clear();
                try
                {
                    if (File.Exists(_path))
                    {
                        File.Delete(_path);
                    }
                }
                catch { }
            }
        }

        private void AppendToFile(TeachDiffRecord record)
        {
            try
            {
                string dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                bool needHeader = !File.Exists(_path);
                using (StreamWriter writer = new StreamWriter(_path, true, Encoding.UTF8))
                {
                    if (needHeader)
                    {
                        writer.WriteLine(Header);
                    }
                    writer.WriteLine(Serialize(record));
                }
            }
            catch { }
        }

        private void Load()
        {
            lock (_lock)
            {
                _records.Clear();
                if (!File.Exists(_path))
                {
                    return;
                }

                try
                {
                    string[] lines = File.ReadAllLines(_path, Encoding.UTF8);
                    foreach (string raw in lines)
                    {
                        string line = raw.TrimEnd('\r', '\n');
                        if (line.Length == 0 || line.StartsWith("#"))
                        {
                            continue;
                        }

                        TeachDiffRecord record;
                        if (TryDeserialize(line, out record))
                        {
                            _records.Add(record);
                        }
                    }
                }
                catch { }
            }
        }

        private static string Serialize(TeachDiffRecord r)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append(Separator);
            sb.Append(r.Station.ToString(CultureInfo.InvariantCulture)).Append(Separator);
            sb.Append(r.Slot.ToString(CultureInfo.InvariantCulture)).Append(Separator);
            sb.Append(string.IsNullOrEmpty(r.Arm) ? "-" : r.Arm).Append(Separator);
            AppendPose(sb, r.Origin);
            AppendPose(sb, r.Current);
            return sb.ToString().TrimEnd(Separator);
        }

        private static void AppendPose(StringBuilder sb, TeachPose p)
        {
            sb.Append(F(p.X)).Append(Separator);
            sb.Append(F(p.Y)).Append(Separator);
            sb.Append(F(p.Z)).Append(Separator);
            sb.Append(F(p.Yaw)).Append(Separator);
            sb.Append(F(p.Pitch)).Append(Separator);
            sb.Append(F(p.Roll)).Append(Separator);
        }

        private static string F(double v)
        {
            return v.ToString("0.######", CultureInfo.InvariantCulture);
        }

        private static bool TryDeserialize(string line, out TeachDiffRecord record)
        {
            record = null;
            string[] parts = line.Split(Separator);
            if (parts.Length < 16)
            {
                return false;
            }

            DateTime ts;
            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out ts))
            {
                ts = DateTime.Now;
            }

            int station = ParseInt(parts[1]);
            int slot = ParseInt(parts[2]);
            string arm = parts[3] == "-" ? string.Empty : parts[3];

            record = new TeachDiffRecord
            {
                Timestamp = ts,
                Station = station,
                Slot = slot,
                Arm = arm,
                Origin = new TeachPose
                {
                    X = ParseD(parts[4]), Y = ParseD(parts[5]), Z = ParseD(parts[6]),
                    Yaw = ParseD(parts[7]), Pitch = ParseD(parts[8]), Roll = ParseD(parts[9])
                },
                Current = new TeachPose
                {
                    X = ParseD(parts[10]), Y = ParseD(parts[11]), Z = ParseD(parts[12]),
                    Yaw = ParseD(parts[13]), Pitch = ParseD(parts[14]), Roll = ParseD(parts[15])
                }
            };
            return true;
        }

        private static int ParseInt(string s)
        {
            int v;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }

        private static double ParseD(string s)
        {
            double v;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : 0.0;
        }
    }
}

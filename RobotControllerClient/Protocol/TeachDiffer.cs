using System.Globalization;

namespace RobotControllerClient.Protocol
{
    public class TeachPose
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Yaw { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }
    }

    public class TeachDiffer
    {
        public const string Prefix = "TEACH_DIFFER";

        public TeachPose Origin { get; set; }
        public TeachPose Current { get; set; }

        public TeachDiffer()
        {
            Origin = new TeachPose();
            Current = new TeachPose();
        }

        public double DeviationX { get { return Current.X - Origin.X; } }
        public double DeviationY { get { return Current.Y - Origin.Y; } }
        public double DeviationZ { get { return Current.Z - Origin.Z; } }
        public double DeviationYaw { get { return Current.Yaw - Origin.Yaw; } }
        public double DeviationPitch { get { return Current.Pitch - Origin.Pitch; } }
        public double DeviationRoll { get { return Current.Roll - Origin.Roll; } }

        public static bool IsTeachDiffer(string line)
        {
            return line != null && line.TrimStart().StartsWith(Prefix);
        }

        public static bool TryParse(string line, out TeachDiffer result)
        {
            result = null;
            if (!IsTeachDiffer(line))
            {
                return false;
            }

            string[] tokens = line.Trim().Split(new[] { ' ', '\t' },
                System.StringSplitOptions.RemoveEmptyEntries);

            double[] values = new double[12];
            int found = 0;
            for (int i = 1; i < tokens.Length && found < 12; i++)
            {
                double v;
                if (double.TryParse(tokens[i], NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                {
                    values[found++] = v;
                }
            }

            if (found < 12)
            {
                return false;
            }

            result = new TeachDiffer
            {
                Origin = new TeachPose
                {
                    X = values[0],
                    Y = values[1],
                    Z = values[2],
                    Yaw = values[3],
                    Pitch = values[4],
                    Roll = values[5]
                },
                Current = new TeachPose
                {
                    X = values[6],
                    Y = values[7],
                    Z = values[8],
                    Yaw = values[9],
                    Pitch = values[10],
                    Roll = values[11]
                }
            };
            return true;
        }
    }
}

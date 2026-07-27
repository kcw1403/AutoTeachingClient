using System;

namespace RobotControllerClient.Protocol
{
    public class TeachDiffRecord
    {
        public DateTime Timestamp { get; set; }
        public int Station { get; set; }
        public int Slot { get; set; }
        public string Arm { get; set; }

        public TeachPose Origin { get; set; }
        public TeachPose Current { get; set; }

        public TeachDiffRecord()
        {
            Timestamp = DateTime.Now;
            Arm = string.Empty;
            Origin = new TeachPose();
            Current = new TeachPose();
        }

        public double DeviationX { get { return Current.X - Origin.X; } }
        public double DeviationY { get { return Current.Y - Origin.Y; } }
        public double DeviationZ { get { return Current.Z - Origin.Z; } }
        public double DeviationYaw { get { return Current.Yaw - Origin.Yaw; } }
        public double DeviationPitch { get { return Current.Pitch - Origin.Pitch; } }
        public double DeviationRoll { get { return Current.Roll - Origin.Roll; } }

        public TeachDiffer ToDiffer()
        {
            return new TeachDiffer
            {
                Origin = new TeachPose
                {
                    X = Origin.X, Y = Origin.Y, Z = Origin.Z,
                    Yaw = Origin.Yaw, Pitch = Origin.Pitch, Roll = Origin.Roll
                },
                Current = new TeachPose
                {
                    X = Current.X, Y = Current.Y, Z = Current.Z,
                    Yaw = Current.Yaw, Pitch = Current.Pitch, Roll = Current.Roll
                }
            };
        }

        public static TeachDiffRecord From(TeachDiffer differ, int station, int slot, string arm)
        {
            TeachDiffRecord record = new TeachDiffRecord
            {
                Timestamp = DateTime.Now,
                Station = station,
                Slot = slot,
                Arm = arm ?? string.Empty
            };

            if (differ != null)
            {
                record.Origin = new TeachPose
                {
                    X = differ.Origin.X, Y = differ.Origin.Y, Z = differ.Origin.Z,
                    Yaw = differ.Origin.Yaw, Pitch = differ.Origin.Pitch, Roll = differ.Origin.Roll
                };
                record.Current = new TeachPose
                {
                    X = differ.Current.X, Y = differ.Current.Y, Z = differ.Current.Z,
                    Yaw = differ.Current.Yaw, Pitch = differ.Current.Pitch, Roll = differ.Current.Roll
                };
            }

            return record;
        }
    }
}

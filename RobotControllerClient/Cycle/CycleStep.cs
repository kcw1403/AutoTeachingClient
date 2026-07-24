namespace RobotControllerClient.Cycle
{
    public class CycleStep
    {
        public string CommandId { get; set; }
        public string CommandText { get; set; }
        public int DelayAfterMs { get; set; }

        public CycleStep()
        {
            DelayAfterMs = 0;
        }

        public CycleStep(string commandId, string commandText, int delayAfterMs = 0)
        {
            CommandId = commandId;
            CommandText = commandText;
            DelayAfterMs = delayAfterMs;
        }

        public CycleStep Clone()
        {
            return new CycleStep(CommandId, CommandText, DelayAfterMs);
        }

        public override string ToString()
        {
            return string.Format("{0}  (딜레이 {1}ms)", CommandText, DelayAfterMs);
        }
    }
}

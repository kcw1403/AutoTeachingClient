namespace RobotControllerClient.Cycle
{
    public class CycleStep
    {
        public string CommandId { get; set; }
        public string CommandText { get; set; }
        public int DelayAfterMs { get; set; }
        public bool IsDelay { get; set; }

        public CycleStep()
        {
            DelayAfterMs = 0;
        }

        public CycleStep(string commandId, string commandText, int delayAfterMs = 0, bool isDelay = false)
        {
            CommandId = commandId;
            CommandText = commandText;
            DelayAfterMs = delayAfterMs;
            IsDelay = isDelay;
        }

        public CycleStep Clone()
        {
            return new CycleStep(CommandId, CommandText, DelayAfterMs, IsDelay);
        }

        public override string ToString()
        {
            if (IsDelay)
            {
                return string.Format("⏱ 대기 {0}ms", DelayAfterMs);
            }
            return CommandText;
        }
    }
}

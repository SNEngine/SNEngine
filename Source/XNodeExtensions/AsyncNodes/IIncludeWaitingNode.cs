namespace SNEngine.AsyncNodes
{
    public interface IIncludeWaitingNode
    {
        public bool IsWorking { get; }

        void SkipWait();
    }
}

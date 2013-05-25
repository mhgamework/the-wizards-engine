namespace MHGameWork.TheWizards.Engine.Diagnostics.Tracing
{
    /// <summary>
    /// Decorates a simulator with tracing functionality
    /// </summary>
    public class TracingDecoratorSimulator : ISimulator
    {
        private readonly ITraceLogger logger;
        private readonly ISimulator sim;
        private string simulatorName;

        public TracingDecoratorSimulator(ITraceLogger logger, ISimulator sim)
        {
            this.logger = logger;
            this.sim = sim;

            simulatorName = sim.GetType().FullName;
        }

        public void Simulate()
        {
            logger.Log("Enter Simulator: " + simulatorName);
            sim.Simulate();
            logger.Log("Exit Simulator: " + simulatorName);

        }
    }
}
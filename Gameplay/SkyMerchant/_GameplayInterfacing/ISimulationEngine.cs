namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    public interface ISimulationEngine
    {
        float Elapsed { get; }
        float CurrentTime { get; }
    }
    public class SimpleSimulationEngine : ISimulationEngine
    {
        public float Elapsed { get { return TW.Graphics.Elapsed; } }
        public float CurrentTime { get { return TW.Graphics.TotalRunTime; } }
    }
}
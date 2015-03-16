namespace MHGameWork.TheWizards.LogicRTS
{
    /// <summary>
    /// Represents the simulation scheduler for the gameloop
    /// </summary>
    public class Scheduler
    {
        public float CurrentTime
        {
            get { return TW.Graphics.TotalRunTime; }
        }
    }
}
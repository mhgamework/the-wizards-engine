using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;

namespace MHGameWork.TheWizards.RTSTestCase1.Simulators
{
    /// <summary>
    /// Simulates the river physics.
    /// Rename worldsimulator or smth?
    /// </summary>
    public class WorldSimulator : ISimulator
    {
        public WorldResourceGenerationSimulator WorldResourceGenerationSimulator { get; set; }
        public SimpleItemPhysicsUpdater SimpleItemPhysicsUpdater { get; set; }
        public void Simulate()
        {
            SimpleItemPhysicsUpdater.Simulate(TW.Graphics.Elapsed);
            WorldResourceGenerationSimulator.Simulate();
        }
    }
}
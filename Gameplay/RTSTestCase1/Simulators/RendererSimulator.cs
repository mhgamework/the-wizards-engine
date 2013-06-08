using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;

namespace MHGameWork.TheWizards.RTSTestCase1.Simulators
{
    /// <summary>
    /// Updates the renderer with state from TW.Data
    /// </summary>
    public class RendererSimulator : ISimulator
    {
        public PlayerCameraSimulator PlayerCameraSimulator { get; set; }
        public PhysicalSimulator PhysicalSimulator { get; set; }
        public WorldRenderingSimulator WorldRenderingSimulator { get; set; }
        public CrystalInfoDrawSimulator CrystalInfoDrawSimulator { get; set; }

        public RTSEntitySimulator RTSEntitySimulator { get; set; }

        public void Simulate()
        {
            PlayerCameraSimulator.Simulate();

            RTSEntitySimulator.Simulate(); //Remove: this is replace by the physical simulator
            PhysicalSimulator.Simulate();

            CrystalInfoDrawSimulator.Simulate();
            WorldRenderingSimulator.Simulate();

        }
    }
}
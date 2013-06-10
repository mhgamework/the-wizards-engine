using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using System.Linq;
using SlimDX;

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
            attachObjects();

            PlayerCameraSimulator.Simulate();

            RTSEntitySimulator.Simulate(); //Remove: this is replace by the physical simulator
            PhysicalSimulator.Simulate();

            CrystalInfoDrawSimulator.Simulate();
            WorldRenderingSimulator.Simulate();

        }

       

        private void attachObjects()
        {
            //foreach (var o in TW.Data.Objects.OfType<i>())
            //{
            //    o.ItemHolder.SetHeldItemDefaultPosition();
            //}

            foreach (var c in TW.Data.Objects.OfType<ICartHolder>())
            {
                c.CartHolder.SetHeldItemDefaultPosition();
            }

            foreach (var i in TW.Data.Objects.OfType<IItemStorage>())
            {
                i.ItemStorage.UpdateItemLocations();
            }
        }
    }
}
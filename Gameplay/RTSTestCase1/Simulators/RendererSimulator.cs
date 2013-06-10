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
            simulateDroppedThingPhysics();

            attachObjects();

            PlayerCameraSimulator.Simulate();

            RTSEntitySimulator.Simulate(); //Remove: this is replace by the physical simulator
            PhysicalSimulator.Simulate();

            CrystalInfoDrawSimulator.Simulate();
            WorldRenderingSimulator.Simulate();

        }

        private void simulateDroppedThingPhysics()
        {
            //TODO: add component for this
            foreach (var t in TW.Data.Objects.OfType<IItem>())
            {
                if (!t.Item.Free) continue;
                var p = t as IPhysical;
                var pos = p.Physical.GetBoundingBox().Minimum;
                if (Math.Abs(pos.Y) > 0.001)
                    p.Physical.WorldMatrix *= Matrix.Translation(0, -Math.Sign(pos.Y) * TW.Graphics.Elapsed, 0);

            }
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Characters;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS
{
    [PersistanceScope]
    public class PlayerPickupSimulator : ISimulator
    {
        //private PlayerRTS player = TW.Data.GetSingleton<PlayerRTS>();

        private Engine.WorldRendering.Entity pickupEntity;

        public PlayerPickupSimulator()
        {
            pickupEntity = new Engine.WorldRendering.Entity();

        }

        public void Simulate()
        {
            simulateKeyE();
            updateRenderer();
        }

        private void updateRenderer()
        {
            //pickupEntity.Visible = player.Holding != null;
            //pickupEntity.WorldMatrix = Matrix.Translation(calculateHoldPosition());
            //pickupEntity.Kinematic = true;
            //pickupEntity.Solid = true;
            //if (player.Holding != null)
            //    pickupEntity.Mesh = player.Holding.CreateMesh();

        }

        private void simulateKeyE()
        {
            //if (!TW.Graphics.Keyboard.IsKeyPressed(Key.E)) return;

            //if (player.Holding == null)
            //    tryPickup();
            //else
            //    tryDrop();
        }

        private void tryDrop()
        {
            //var dropped = new DroppedThing();
            //dropped.InitialPosition = calculateHoldPosition();
            //if (dropped.InitialPosition.Y < 0.2f) dropped.InitialPosition = new Vector3(dropped.InitialPosition.X, 0.2f, dropped.InitialPosition.Z);

            //dropped.Thing = player.Holding;
            //player.Holding = null;


        }

        private Vector3 calculateHoldPosition()
        {
            Matrix viewInverse = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse;
            return (viewInverse.xna().Translation + viewInverse.xna().Forward * 3).dx();
        }

        private void tryPickup()
        {
            var obj = TW.Data.GetSingleton<Engine.WorldRendering.World>()
                        .Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), e => e.Tag is DroppedThing);
            if (!obj.IsHit) return;
            if (obj.Distance > 5) return;
            if (obj.Object == null) return;

            var ent = obj.Object;
            var found = ent.Tag as DroppedThing;
            if (found == null) return;


            //TODO: player.Holding = found;


        }
    }
}

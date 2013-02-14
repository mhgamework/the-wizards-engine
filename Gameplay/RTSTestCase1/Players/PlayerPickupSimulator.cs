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
using MHGameWork.TheWizards.RTSTestCase1.Players;
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
            foreach (var pl in TW.Data.Objects.Where(o => o is UserPlayer).Cast<UserPlayer>().ToArray())
                simulateKeyE(pl);

        }

        private void simulateKeyE(UserPlayer player)
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.E)) return;

            if (player.Holding == null)
                tryPickup(player);
            else
                tryDrop(player);
        }

        private void tryDrop(UserPlayer player)
        {
            player.DropHolding();
            //var dropped = new DroppedThing();
            //dropped.InitialPosition = calculateHoldPosition();
            //if (dropped.InitialPosition.Y < 0.2f) dropped.InitialPosition = new Vector3(dropped.InitialPosition.X, 0.2f, dropped.InitialPosition.Z);

            //dropped.Thing = player.Holding;
            //player.Holding = null;


        }

        private void tryPickup(UserPlayer player)
        {
            //var obj = TW.Data.GetSingleton<Engine.WorldRendering.World>()
            //            .Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), e => e.Tag is DroppedThing);
            var obj = player.Targeted;
            if (obj == null) return;
            if (player.TargetDistance > 5) return;
            if (!(obj.Tag is DroppedThing)) return;

            var found = obj.Tag as DroppedThing;

            player.Holding = found;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTS.Commands;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS
{
    public class PlayerPickupSimulator : ISimulator
    {
        private PlayerRTS player = TW.Data.GetSingleton<PlayerRTS>();

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
            pickupEntity.Visible = player.Holding != null;
            pickupEntity.WorldMatrix = Matrix.Translation(calculateHoldPosition());
        }

        private void simulateKeyE()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.E)) return;

            if (player.Holding == null)
                tryPickup();
            else
                tryDrop();
        }

        private void tryDrop()
        {
            var dropped = new DroppedThing();
            dropped.Position = calculateHoldPosition();
            if (dropped.Position.Y < 0.2f) dropped.Position = new Vector3(dropped.Position.X, 0.2f, dropped.Position.Z);

            dropped.Thing = player.Holding;
            player.Holding = null;


        } 

        private Vector3 calculateHoldPosition()
        {
            Matrix viewInverse = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse;
            return (viewInverse.xna().Translation + viewInverse.xna().Forward * 3).dx();
        }

        private void tryPickup()
        {
            var obj = TW.Data.GetSingleton<Engine.WorldRendering.World>()
                        .Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(),
                        e => TW.Data.Objects.Where(o => o is DroppedThing).Cast<DroppedThing>().Count(o => o.get<Engine.WorldRendering.Entity>() == e)> 0 );
            if (!obj.IsHit) return;
            if (obj.Distance > 5) return;
            if (!(obj.Object is Engine.WorldRendering.Entity)) return;

            var ent = obj.Object as Engine.WorldRendering.Entity;
            DroppedThing found = TW.Data.Objects.Where(o => o is DroppedThing).Cast<DroppedThing>().FirstOrDefault(o => o.get<Engine.WorldRendering.Entity>() == ent);
            if (found == null) return;

            player.Holding = found.Thing;
            TW.Data.Objects.Remove(found);

            pickupEntity.Mesh = found.get<Engine.WorldRendering.Entity>().Mesh;

        }
    }
}

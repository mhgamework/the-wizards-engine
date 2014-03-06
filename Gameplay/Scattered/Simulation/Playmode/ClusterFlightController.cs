using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11.Input;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using SlimDX.DirectInput;
using Castle.Core.Internal;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation.Playmode
{
    public class ClusterFlightController
    {
        private readonly TWKeyboard keyboard;
        private HashSet<Island> hashSet;

        public ClusterFlightController(TWKeyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        public void SimulateFlightStep(Island island)
        {
            var dir = new Vector3();
            if (keyboard.IsKeyDown(Key.W)) island.Velocity += island.GetForward() * TW.Graphics.Elapsed;
            if (keyboard.IsKeyDown(Key.S)) island.Velocity -= island.GetForward() * TW.Graphics.Elapsed;


            var turnSpeed = 0.8f;
            if (keyboard.IsKeyDown(Key.A)) turnCluster(island, turnSpeed * TW.Graphics.Elapsed);
            if (keyboard.IsKeyDown(Key.D)) turnCluster(island, -turnSpeed * TW.Graphics.Elapsed);

            // Add up and down?

            island.GetIslandsInCluster().ForEach(c => c.Velocity = island.Velocity);

        }

        private void turnCluster(Island island, float angle)
        {
            var all = island.GetIslandsInCluster();
            all.ForEach(c => c.RotationY += angle);
            var mean = all.Aggregate(new Vector3(), (acc, el) => acc + el.Position) / all.Count();

            var rotation = Matrix.Translation(-mean) * Matrix.RotationY(angle) * Matrix.Translation(mean);

            all.ForEach(c => c.Position = Vector3.TransformCoordinate(c.Position, rotation));
        }

      
    }
}
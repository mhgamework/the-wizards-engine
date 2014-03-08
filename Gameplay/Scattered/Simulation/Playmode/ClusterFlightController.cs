using System;
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
            if (keyboard.IsKeyDown(Key.W)) island.Velocity += island.GetForward() * TW.Graphics.Elapsed * 10;
            if (keyboard.IsKeyDown(Key.S)) island.Velocity -= island.GetForward() * TW.Graphics.Elapsed * 10;


            var turnSpeed = 0.8f;
            if (keyboard.IsKeyDown(Key.A)) turnCluster(island, turnSpeed * TW.Graphics.Elapsed);
            if (keyboard.IsKeyDown(Key.D)) turnCluster(island, -turnSpeed * TW.Graphics.Elapsed);

            if (keyboard.IsKeyDown(Key.LeftArrow)) island.Velocity -= island.GetWorldMatrix().xna().Right.dx() * TW.Graphics.Elapsed;
            if (keyboard.IsKeyDown(Key.RightArrow)) island.Velocity += island.GetWorldMatrix().xna().Right.dx() * TW.Graphics.Elapsed;
            if (keyboard.IsKeyDown(Key.UpArrow)) island.Velocity += island.GetForward() * TW.Graphics.Elapsed;
            if (keyboard.IsKeyDown(Key.DownArrow)) island.Velocity -= island.GetForward() * TW.Graphics.Elapsed;

            if (island.Velocity.Length() < 0.1f
                && !keyboard.IsKeyDown(Key.W)
                && !keyboard.IsKeyDown(Key.S)
                && !keyboard.IsKeyDown(Key.LeftArrow)
                && !keyboard.IsKeyDown(Key.RightArrow)
                && !keyboard.IsKeyDown(Key.UpArrow)
                && !keyboard.IsKeyDown(Key.DownArrow)
                ) island.Velocity = new Vector3();
            // Add up and down?

            island.GetIslandsInCluster().ForEach(c => c.Velocity = island.Velocity);

            adjustForDocking(island);
        }

        private void adjustForDocking(Island island)
        {
            var all = island.GetIslandsInCluster();
            var almostConnections = all.SelectMany(i => i.BridgeConnectors)
               .SelectMany(
                   i => island.Level.Islands.Except(all).SelectMany(c => c.BridgeConnectors)
                              .Select(c => new
                                  {
                                      Mine = i,
                                      Remote = c,
                                      Dist = Vector3.Distance(i.GetAbsolutePosition(), c.GetAbsolutePosition()),
                                      Angle = (float)Math.Acos(Vector3.Dot(i.GetAbsoluteDirection(), -c.GetAbsoluteDirection()))
                                  })
                              .Where(c => CanAutoDock(c.Mine, c.Remote)));


            if (!almostConnections.Any()) return;
            //var closestConnection = getClosest(almostConnections);
            var closestConnection = almostConnections.OrderBy(c => Math.Abs(c.Dist) + Math.Abs(c.Angle)).First();

            var rot = -closestConnection.Angle * Math.Sign(Vector3.Cross(closestConnection.Mine.GetAbsoluteDirection(), closestConnection.Remote.GetAbsoluteDirection()).Y);

            turnCluster(island, rot);
            moveCluster(island,
                        closestConnection.Remote.GetAbsolutePosition() - closestConnection.Mine.GetAbsolutePosition());

            all.ForEach(v => v.Velocity = closestConnection.Remote.Island.Velocity);

            closestConnection.Mine.Island.AddBridgeTo(closestConnection.Remote.Island);
        }

        public bool CanAutoDock(Island.BridgeConnector A, Island.BridgeConnector B)
        {
            return Vector3.Distance(A.GetAbsolutePosition(), B.GetAbsolutePosition()) < 0.1f
                   && (float)Math.Acos(Vector3.Dot(A.GetAbsoluteDirection(), -B.GetAbsoluteDirection())) < 0.8f;
        }

        private void turnCluster(Island island, float angle)
        {
            var all = island.GetIslandsInCluster();
            all.ForEach(c => c.RotationY += angle);
            var mean = all.Aggregate(new Vector3(), (acc, el) => acc + el.Position) / all.Count();

            var rotation = Matrix.Translation(-mean) * Matrix.RotationY(angle) * Matrix.Translation(mean);

            all.ForEach(c => c.Position = Vector3.TransformCoordinate(c.Position, rotation));
        }

        private void moveCluster(Island island, Vector3 offset)
        {
            island.GetIslandsInCluster().ForEach(c => c.Position += offset);
        }


    }
}
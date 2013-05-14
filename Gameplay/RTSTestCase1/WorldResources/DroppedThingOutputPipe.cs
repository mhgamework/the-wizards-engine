using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.Rendering;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    /// <summary>
    /// Control PhysX stuff to safely create a new DroppedThing (without causing jumping items)
    /// The New item is spawned in a location which is surrounded by a PhysX invisible box, which makes this area free of collisions
    /// </summary>
    public class DroppedThingOutputPipe
    {
        private readonly Vector3 spawnPosition;
        private readonly Vector3 moveDirection;
        private DroppedThing drop;
        public float OutputStrength { get; set; }

        public bool IsFree
        {
            get
            {
                var a = getActor(drop);
                if (a == null) return drop == null;
                return Vector3.Dot(moveDirection, getActor(drop).GlobalPosition.dx() - spawnPosition) > Thing.GetRadius() * 2.01f;
            }
        }

        public DroppedThingOutputPipe(Vector3 spawnPosition, Vector3 moveDirection)
        {
            this.spawnPosition = spawnPosition;
            this.moveDirection = moveDirection;

            OutputStrength = 10;

            var tubeEntity = new Entity();

            var mesh = new RAMMesh();
            var f = 0.3f;
            var radius = Thing.GetRadius() + 0.05f;

            for (float angle = 0; angle < MathHelper.TwoPi; angle += MathHelper.PiOver2)
            {
                mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box() { Dimensions = new Vector3(radius * 2, f, radius * 2).xna(), Orientation = Matrix.Translation(0, -f * 0.5f - radius, 0).xna() * Matrix.RotationAxis(Vector3.UnitX, angle).xna() });
            }
            mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box() { Dimensions = new Vector3(f, radius * 2, radius * 2).xna(), Orientation = Matrix.Translation(-f * 0.5f - radius, 0, 0).xna() });
            //mesh.GetCollisionData().ConvexMeshes.Add(new MeshCollisionData.Convex());
            tubeEntity.Mesh = mesh;
            tubeEntity.Solid = true;
            tubeEntity.Static = false;
            tubeEntity.Kinematic = true;
            tubeEntity.WorldMatrix = Matrix.RotationY(MathHelper.PiOver2) * Matrix.Invert(Matrix.LookAtRH(spawnPosition, spawnPosition + moveDirection, Vector3.UnitY));

        }

        public void Update()
        {
            if (IsFree) drop = null;
            var a = getActor(drop);
            if (a == null) return;
            if (a.LinearVelocity.Length() < 1)
                a.AddForce(moveDirection.xna() * OutputStrength, ForceMode.Impulse);
        }
        private Actor getActor(DroppedThing drop)
        {
            if (drop == null) return null;

            if (drop.get<Entity>() == null) return null;
            if (drop.get<Entity>().get<EntityPhysXUpdater.EntityPhysX>() == null) return null;
            if (drop.get<Entity>().get<EntityPhysXUpdater.EntityPhysX>().getCurrentActor() == null) return null;
            return drop.get<Entity>().get<EntityPhysXUpdater.EntityPhysX>().getCurrentActor();
        }
        public void SpawnItem(Thing thing)
        {
            drop = new DroppedThing() { Thing = thing };
            drop.Physical.WorldMatrix = Matrix.Translation(spawnPosition);
        }
    }
}

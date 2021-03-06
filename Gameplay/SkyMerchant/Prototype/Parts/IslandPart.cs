﻿using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    /// <summary>
    /// IPhysical is here to make the simpleworldlocator work
    /// </summary>
    [ModelObjectChanged]
    public class IslandPart : EngineModelObject,IGameObjectComponent
    {
        #region Injection
        public IPositionComponent Physical { get; set; }
        public IMeshRenderComponent RenderComponent { get; set; }
        public BasicPhysicsPart Physics { get; set; }
        public IslandMeshFactory IslandMeshFactory { get; set; }
        #endregion

        private static float islandCollisionRadius = 20;

        public IslandPart(IPositionComponent physical, IMeshRenderComponent renderComponent, BasicPhysicsPart physics, IslandMeshFactory islandMeshFactory)
        {
            Physical = physical;
            RenderComponent = renderComponent;
            Physics = physics;
            IslandMeshFactory = islandMeshFactory;
        }

        public void UpdatePhysical()
        {

        }


        #region Visualization

        /// <summary>
        /// Returns the max Y position of the islands voxels (absolute coords)
        /// </summary>
        /// <returns></returns>
        public float GetMaxY()
        {
            return Physical.LocalBoundingBox.Transform(Physical.GetWorldMatrix()).Maximum.Y;
        }

        public int Seed { get; set; }

        public float TargetHeight { get; set; }

        public void FixPhysical()
        {
            if (RenderComponent.Mesh != null) return;
            RenderComponent.Mesh = IslandMeshFactory.GetMesh(Seed);
            var boundingBox = TW.Assets.GetBoundingBox(RenderComponent.Mesh);
            var center = (boundingBox.Minimum + boundingBox.Maximum) * 0.5f;
            center.Y = boundingBox.Maximum.Y;
            RenderComponent.ObjectMatrix = Matrix.Translation(-center);
        }

        #endregion

        public void SimulateFloatForce()
        {
            var i = this;
            var diff = i.Physical.Position.Y - i.TargetHeight;
            i.Physics.ApplyForce(-diff * Vector3.UnitY);
            i.Physics.ApplyForce(-0.1f * Vector3.UnitY * Vector3.Dot(Vector3.UnitY, i.Physics.Velocity));

        }

        public void SimulateMovement()
        {
            var i = this;
            // Move and wrap
            var oldPos = i.Physical.Position;
            var newPos = i.Physical.Position + i.Physics.Velocity * TW.Graphics.Elapsed;
            i.Physical.Position = newPos;

            //TW.Graphics.LineManager3D.AddCenteredBox(newPos, islandCollisionRadius, new Color4(0, 0, 0));
        }

        public void SimulateIslandWrapping()
        {
            var i = this;
            // Move and wrap
            var newPos = i.Physical.Position;
            for (int j = 0; j < 3; j++)
            {
                var worldRange = 2000;
                if (newPos[j] > worldRange) newPos[j] = -worldRange;
                if (newPos[j] < -worldRange) newPos[j] = worldRange;
            }


            i.Physical.Position = newPos;
        }

        public void SimulateIslandShieldCollision()
        {
            var coeff = 1f;
            var i = this;

            foreach (var j in TW.Data.Objects.OfType<IslandPart>())
            {
                if (i == j) continue;
                var dir = j.Physical.Position - i.Physical.Position;
                var dist = dir.Length();

                dir.Normalize();

                if (dist > islandCollisionRadius) continue;
                if (Vector3.Dot(i.Physics.Velocity, dir) < 0
                    && Vector3.Dot(j.Physics.Velocity, -dir) < 0) continue;

                var speedAlongDir = Vector3.Dot(i.Physics.Velocity, dir) * dir - Vector3.Dot(j.Physics.Velocity, dir) * dir;
                i.Physics.Velocity -= speedAlongDir;
                j.Physics.Velocity += speedAlongDir;
                //i.ApplyForce(-dir * coeff);
                //j.ApplyForce(dir * coeff);

            }
        }

        /// <summary>
        /// F_D\, =\, \tfrac12\, \rho\, v^2\, C_D\, A
        /// </summary>
        public void SimulateIslandAirDrag()
        {
            var dragCoeff = 0.01f;
            var i = this;
            // Short for -norm(vel) * len(vel) * len(vel) * coeff;
            i.Physics.ApplyForce(-i.Physics.Velocity * i.Physics.Velocity.Length() * dragCoeff);
            // simplified drag equation (using velocity ^2)
        }

        //private void applyBridgePull()
        //{
        //    var i = this;
        //    {
        //        // assume islands equal weight
        //        var deltaX = b.CalculateCurrentLength() - b.InitialLength;

        //        //if (deltaX > 10) continue;
        //        if (deltaX < 0) continue;

        //        //deltaX = 10;
        //        var force = b.GetBridgeDirection();
        //        //force.Y = 0;
        //        var maxForce = 0.1f;
        //        if (force.Length() > maxForce)
        //            force = force / force.Length() * maxForce;
        //        b.Start.ApplyForce(force * deltaX);
        //        b.End.ApplyForce(-force * deltaX);
        //    }
        //}

        //private void drawBridges()
        //{
        //    var offset = new Vector3(0, 5, 0);

        //    foreach (var b in TW.Data.Objects.OfType<Bridge>())
        //    {
        //        var col = new Color4(0, 1, 0);
        //        if (b.CalculateCurrentLength() > b.InitialLength)
        //            col = new Color4(1, 0, 0);

        //        TW.Graphics.LineManager3D.AddLine(b.Start.Physical.Position + offset, b.End.Physical.Position + offset, col);
        //    }

        //    var target = getTargetedIsland();
        //    if (bridgeStart != null && target != null)
        //    {
        //        TW.Graphics.LineManager3D.AddLine(bridgeStart.Physical.Position + offset, target.Physical.Position + offset, new Color4(1, 0, 0));
        //    }
        //}

        //private void driveIsland()
        //{
        //    if (drivingIsland == null) return;

        //    var dir = new Vector3(0, 0, 0);
        //    if (TW.Graphics.Keyboard.IsKeyDown(MinHash.W)) dir += new Vector3(0, 0, -1);
        //    if (TW.Graphics.Keyboard.IsKeyDown(MinHash.A)) dir += new Vector3(-1, 0, 0);
        //    if (TW.Graphics.Keyboard.IsKeyDown(MinHash.S)) dir += new Vector3(0, 0, 1);
        //    if (TW.Graphics.Keyboard.IsKeyDown(MinHash.D)) dir += new Vector3(1, 0, 0);

        //    if (TW.Graphics.Keyboard.IsKeyDown(MinHash.C)) drivingIsland.Velocity = new Vector3();

        //    dir = -Vector3.TransformNormal(dir, Matrix.RotationY(-TW.Graphics.SpectaterCamera.AngleHorizontal));

        //    dir *= 5;

        //    drivingIsland.Velocity += dir * TW.Graphics.TickLength;

        //    TW.Graphics.SpectaterCamera.CameraPosition = drivingIsland.Physical.Position +
        //                                                 new Vector3(0, 10, 0);
        //}

    }
}
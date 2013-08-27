using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant
{
    /// <summary>
    /// Prototype class, not used in production
    /// </summary>
    [ModelObjectChanged]
    public class Island : EngineModelObject, IPhysical
    {
        public Island()
        {
            Physical = new Physical();
        }

        public int Seed { get; set; }

        public Vector3 Velocity { get; set; }

        public float TargetHeight { get; set; }
        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            if (Physical.Mesh == null)
            {
                Physical.Mesh = CreateMesh();
                var boundingBox = TW.Assets.GetBoundingBox(Physical.Mesh);
                var center = (boundingBox.Minimum + boundingBox.Maximum) * 0.5f;

                Physical.ObjectMatrix = Matrix.Translation(-center);

            }
        }

        public IMesh CreateMesh()
        {
            var r = new Random(Seed);
            var gen = new IslandGenerater(r);

            var c = new VoxelMeshBuilder();

            var island = gen.GenerateIsland(r.Next(7, 20));
            var mesh = c.BuildMesh(island);

            return mesh;
        }

        /// <summary>
        /// This method applies given force for this tick
        /// Don't apply elapsed to the force parameter yourself!
        /// </summary>
        /// <param name="force"></param>
        public void ApplyForce(Vector3 force)
        {
            Velocity += force * TW.Graphics.Elapsed; //TODO: incorporate mass here? (f=ma)
        }
    }
}
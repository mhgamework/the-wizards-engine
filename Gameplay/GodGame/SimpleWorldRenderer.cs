using System;
using System.Diagnostics;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class SimpleWorldRenderer : ISimulator
    {
        public const int RenderSize = 10;
        private World world;

        private Array2D<Entity> entities;


        public SimpleWorldRenderer(World world)
        {
            this.world = world;

            Debug.Assert(Math.Abs(world.VoxelSize.X - world.VoxelSize.Y) < 0.001f);

            entities = new Array2D<Entity>(new Point2(RenderSize, RenderSize));
            entities.ForEach((e, p) => entities[p] = new Entity()
                {
                    WorldMatrix = Matrix.Scaling(new Vector3(world.VoxelSize.X)) * Matrix.Translation(world.GetBoundingBox(p).GetCenter())
                });

        }

        public void Simulate()
        {
            entities.ForEach((e, p) =>
                {
                    var v = world.GetVoxel(p);
                    if (v == null)
                    {
                        e.Visible = false;
                        return;
                    }
                    e.Mesh = getMesh(v);
                    e.Visible = e.Mesh != null;


                });
        }

        /// <summary>
        /// TODO: Use memoization pattern
        /// </summary>
        /// <param name="gameVoxel"></param>
        /// <returns></returns>
        private IMesh getMesh(GameVoxel gameVoxel)
        {
            if (gameVoxel.Type == null) return null;
            if (gameVoxel.Type.NoMesh) return null;

            return UtilityMeshes.CreateBoxColored(gameVoxel.Type.Color, new Vector3(0.5f, 0.05f, 0.5f));
        }
    }
}
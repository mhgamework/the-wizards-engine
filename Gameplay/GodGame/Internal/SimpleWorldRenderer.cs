using System;
using System.Diagnostics;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class SimpleWorldRenderer : ISimulator
    {
        public const int RenderSize = 20;
        private World world;

        private Array2D<Entity> entities;
        private SpotLight light;


        public SimpleWorldRenderer(World world)
        {
            this.world = world;

            Debug.Assert(Math.Abs(world.VoxelSize.X - world.VoxelSize.Y) < 0.001f);

            entities = new Array2D<Entity>(new Point2(RenderSize, RenderSize));
            entities.ForEach((e, p) => entities[p] = new Entity());


            light = TW.Graphics.AcquireRenderer().CreateSpotLight();
            light.LightRadius = 300;
            light.LightIntensity = 1;
            light.SpotDirection = Vector3.Normalize(new Vector3(-1, -1, -1));
            light.ShadowsEnabled = true;
            light.Color = Color.White.dx().ToVector3();
        }

        public void Simulate()
        {
            var target = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!target.HasValue) return;

            var offset =
                ((target.Value / world.VoxelSize.X).TakeXZ() - new Point2(RenderSize / 2, RenderSize / 2))
                .Floor();



            var worldTranslation = ((Vector2)offset).ToXZ() * world.VoxelSize.X;
            

            var bb = new BoundingBox(new Vector3() + worldTranslation, (world.VoxelSize * RenderSize).ToXZ(2) + worldTranslation);
            TW.Graphics.LineManager3D.AddBox(bb, new Color4(0, 0, 0));

            var spotTarget = (bb.GetCenter());

            light.LightPosition = spotTarget + new Vector3(10, 200, 10);
            light.SpotDirection = Vector3.Normalize(spotTarget - light.LightPosition);


            entities.ForEach((e, p) =>
                {
                    var v = world.GetVoxel(p + new Point2(offset));
                    if (v == null)
                    {
                        e.Visible = false;
                        return;
                    }
                    e.Mesh = getMesh(v);
                    e.WorldMatrix = Matrix.Scaling(new Vector3(world.VoxelSize.X)) *
                                    Matrix.Translation(world.GetBoundingBox(p).GetCenter() + worldTranslation);
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

            var handle = new IVoxelHandle(world, gameVoxel);

            return gameVoxel.Type.GetMesh(handle);

        }
    }
}
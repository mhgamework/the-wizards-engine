using System;
using System.Collections.Generic;
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
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class SimpleWorldRenderer : ISimulator
    {
        public const int RenderSize = 20;
        private World world;

        private Array2D<Entity> entities;
        private SpotLight light1;
        private SpotLight light2;

        private readonly CustomVoxelsRenderer customVoxelsRenderer;

        public SimpleWorldRenderer(World world)
        {
            this.world = world;

            Debug.Assert(Math.Abs(world.VoxelSize.X - world.VoxelSize.Y) < 0.001f);

            entities = new Array2D<Entity>(new Point2(RenderSize, RenderSize));
            entities.ForEach((e, p) => entities[p] = new Entity());


            light1 = TW.Graphics.AcquireRenderer().CreateSpotLight();
            light2 = TW.Graphics.AcquireRenderer().CreateSpotLight();
            customVoxelsRenderer = new CustomVoxelsRenderer(this, world);
        }

        private void configureLight(SpotLight light, Vector3 spotTarget, Vector3 offset)
        {
            light.LightRadius = 300;
            light.LightIntensity = 1;
            light.ShadowsEnabled = true;
            light.Color = Color.White.dx().ToVector3();
            light.LightPosition = spotTarget + offset;
            light.SpotDirection = Vector3.Normalize(spotTarget - light.LightPosition);
        }

        private static Vector3 dir()
        {
            return new Vector3(-1, -1, -1);
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

            updateLights(bb);




            updateVoxelEntities(offset, worldTranslation);
            customVoxelsRenderer.updateVoxelCustomRenderers(offset, worldTranslation,entities);


        }

        private void updateVoxelEntities(Point2 offset, Vector3 worldTranslation)
        {
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



        private void updateLights(BoundingBox visibleWorldBB)
        {
            var spotTarget = (visibleWorldBB.GetCenter());
            configureLight(light1, spotTarget, new Vector3(40, 200, 40));
            configureLight(light2, spotTarget, new Vector3(-50, 200, -50));
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
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class SimpleWorldRenderer : ISimulator
    {
        public const int RenderSize = 32;
        private Model.World world;
        private readonly IVoxelWorldRenderer voxelWorldRenderer;

        private SpotLight light1;
        private SpotLight light2;

        private readonly CustomVoxelsRenderer customVoxelsRenderer;
        public IEnumerable<IRenderable> VisibleCustomRenderables
        {
            get { return customVoxelsRenderer.VisibleCustomRenderables; }
        }

        public SimpleWorldRenderer(Model.World world)
            : this(world, new PerEntityVoxelWorldRenderer(world, new Point2(RenderSize, RenderSize)))
        {

        }
        public SimpleWorldRenderer(Model.World world, IVoxelWorldRenderer voxelWorldRenderer)
        {
            this.world = world;
            this.voxelWorldRenderer = voxelWorldRenderer;

            Debug.Assert(Math.Abs(world.VoxelSize.X - world.VoxelSize.Y) < 0.001f);




            light1 = TW.Graphics.AcquireRenderer().CreateSpotLight();
            light2 = TW.Graphics.AcquireRenderer().CreateSpotLight();
            customVoxelsRenderer = new CustomVoxelsRenderer(world);
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
            TW.Graphics.SpectaterCamera.FarClip = 2000;


            //TODO: join interfaces and make a single renderer using composite?
            voxelWorldRenderer.UpdateWindow(offset, worldTranslation, new Point2(RenderSize, RenderSize));
            customVoxelsRenderer.updateVoxelCustomRenderers(offset, worldTranslation, new Point2(RenderSize, RenderSize));


        }


        private void updateLights(BoundingBox visibleWorldBB)
        {
            var spotTarget = (visibleWorldBB.GetCenter());
            configureLight(light1, spotTarget, new Vector3(40, 200, 40));
            configureLight(light2, spotTarget, new Vector3(-50, 200, -50));
        }


    }
}
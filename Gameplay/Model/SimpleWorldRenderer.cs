using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.World.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Model
{
    public class SimpleWorldRenderer : ISimulator
    {
        private DeferredRenderer deferred;
        private WorldRenderer renderer;
        private CameraInfo info;

        public SimpleWorldRenderer()
        {
            deferred = new DeferredRenderer(TW.Game);
            renderer = new WorldRenderer(TW.Model, deferred);

            var light = deferred.CreateDirectionalLight();
            light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
            light.ShadowsEnabled = true;


            info = TW.Model.GetSingleton<CameraInfo>();

        }

        public void Simulate()
        {
            TW.Game.Camera = info.ActiveCamera;
            renderer.ProcessWorldChanges();
            deferred.Draw();

        }
    }
}

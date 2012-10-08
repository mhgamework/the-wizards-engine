﻿using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;



namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for rendering the world
    /// </summary>
    public class WorldRenderingSimulator : ISimulator
    {
        private DeferredRenderer deferred;
        private CameraInfo info;
        private TextareaUpdater textareaSimulator;
        private WireframeBoxSimulator wireframeSimulator;
        private EntitySimulator entitySimulator;
        private PointLightSimulator pointLightSimulator;

        public WorldRenderingSimulator()
        {

            deferred = TW.Graphics.AcquireRenderer();

            var data = TW.Data.GetSingleton<Data>();

            if (!data.LightCreated)
            {
                var light = deferred.CreateDirectionalLight();
                light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
                light.ShadowsEnabled = true;
                data.LightCreated = true;
            }



            info = TW.Data.GetSingleton<CameraInfo>();


            textareaSimulator = new TextareaUpdater();

            entitySimulator = new EntitySimulator();
            wireframeSimulator = new WireframeBoxSimulator();
            pointLightSimulator = new PointLightSimulator();



        }

        public void Simulate()
        {
            TW.Graphics.Camera = info.ActiveCamera;
            deferred.Draw();

            entitySimulator.Simulate();
            wireframeSimulator.Simulate();
            pointLightSimulator.Simulate();

            textareaSimulator.Update();
            textareaSimulator.Render();


        }

        public class Data : EngineModelObject
        {
            public bool LightCreated = false;
        }
    }
}

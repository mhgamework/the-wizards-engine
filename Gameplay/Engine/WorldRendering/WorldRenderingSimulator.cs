﻿using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Responsible for rendering the world
    /// NOTE: UI simulation was moved the UISimualtor
    /// </summary>
    public class WorldRenderingSimulator : ISimulator
    {
        private DeferredRenderer deferred;
        private CameraInfo info;
        
        private WireframeBoxSimulator wireframeSimulator;
        private EntitySimulator entitySimulator;
        private PointLightSimulator pointLightSimulator;

        public WorldRenderingSimulator()
        {

            deferred = TW.Graphics.AcquireRenderer();

            deferred.SSAO.MNumDirs = 8;
            deferred.SSAO.MNumSteps = 2;
            deferred.SSAO.MContrast = 2;
            deferred.SSAO.MRadiusMultiplier = 2;

            var data = TW.Data.GetSingleton<Data>();

            //if (!data.LightCreated)
            {
                var light = deferred.CreateDirectionalLight();
                light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
                light.ShadowsEnabled = true;
                data.LightCreated = true;
            }



            info = TW.Data.GetSingleton<CameraInfo>();



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

            

            TW.Graphics.LineManager3D.Render(TW.Graphics.Camera);
            TW.Graphics.SetBackbuffer();

            

        }

        public class Data : EngineModelObject
        {
            public bool LightCreated = false;
        }
    }
}

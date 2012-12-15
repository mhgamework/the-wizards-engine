using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.WorldRendering;
using PointLight = MHGameWork.TheWizards.Rendering.Deferred.PointLight;

namespace MHGameWork.TheWizards.VoxelTerraining
{
    public class FlashlightSimulator : ISimulator
    {
        private PointLight pointLight;
        private CameraInfo cameraInfo;

        public FlashlightSimulator()
        {
            cameraInfo = TW.Data.GetSingleton<CameraInfo>();

            pointLight = TW.Graphics.AcquireRenderer().CreatePointLight();
            pointLight.LightRadius = 20;
        }

        public void Simulate()
        {
            pointLight.LightPosition = cameraInfo.ActiveCamera.ViewInverse.xna().Translation.dx();
        }
    }
}

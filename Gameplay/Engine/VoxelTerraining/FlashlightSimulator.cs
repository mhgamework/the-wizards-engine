using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using PointLight = MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.PointLight;

namespace MHGameWork.TheWizards.Engine.VoxelTerraining
{
    public class FlashlightSimulator : ISimulator
    {
        private PointLight pointLight;
        private CameraInfo cameraInfo;

        public FlashlightSimulator()
        {
            cameraInfo = TW.Data.GetSingleton<CameraInfo>();

            pointLight = TW.Graphics.AcquireRenderer().CreatePointLight();
            //pointLight = TW.Graphics.AcquireRenderer().CreateSpotLight();
            pointLight.LightRadius = 20;
        }

        public void Simulate()
        {
            var pos = cameraInfo.ActiveCamera.ViewInverse.xna().Translation.dx() - cameraInfo.ActiveCamera.ViewInverse.xna().Right.dx()*0.2f;
            //var lookat = cameraInfo.ActiveCamera.ViewInverse.xna().Translation.dx() + cameraInfo.ActiveCamera.ViewInverse.xna().Forward.dx() * 10;
            pointLight.LightPosition = pos;
            //pointLight.SpotDirection = Vector3.Normalize(lookat - pos);
            //pointLight.SpotLightAngle = MathHelper.ToRadians(20);
            ////pointLight.SpotDecayExponent = 100;
        }
    }
}

using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using PointLight = MHGameWork.TheWizards.Rendering.Deferred.PointLight;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Adds a light at the camera position
    /// </summary>
    public class CameraLightSimulator:ISimulator
    {
        private PointLight light;

        public CameraLightSimulator()
        {
            light = TW.Graphics.AcquireRenderer().CreatePointLight();
        }

        public static void addCameraLightSimulator(TWEngine engine)
        {

            engine.AddSimulator(new BasicSimulator(() =>
            {
                
            }));
        }

        public void Simulate()
        {
            light.LightIntensity = 2;
            light.LightRadius = 30;
            light.LightPosition = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.GetTranslation();
        }
    }
}
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using PointLight = MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.PointLight;

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
            Light.LightIntensity = 1;
            Light.LightRadius = 200;
        }

        public PointLight Light
        {
            get { return light; }
        }

        public static void addCameraLightSimulator(TWEngine engine)
        {

            engine.AddSimulator(new BasicSimulator(() =>
            {
                
            }));
        }

        public void Simulate()
        {
         
            Light.LightPosition = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.GetTranslation();
        }
    }
}
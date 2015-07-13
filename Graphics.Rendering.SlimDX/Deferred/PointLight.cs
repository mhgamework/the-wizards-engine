using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling;
using SlimDX;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred
{
    public class PointLight
    {
        public Vector3 LightPosition { get; set; }
        public float LightIntensity { get; set; }
        public float LightRadius { get; set; }
        public Vector3 Color { get; set; }
        public bool ShadowsEnabled { get; set; }
        
        
            
        public FrustumCullerView[] Views { get; private set; }

        public PointLight(FrustumCuller culler)
        {
            LightPosition = new Vector3(0, 6, 0);
            LightRadius = 6;
            LightIntensity = 1;

            Color = new Vector3(1, 1, 0.9f);

            Views = new FrustumCullerView[6];
            for (int i = 0; i < Views.Length; i++)
            {
                Views[i] = culler.CreateView();
            }
        }
    }
}

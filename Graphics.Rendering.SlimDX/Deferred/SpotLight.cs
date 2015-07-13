using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling;
using SlimDX;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred
{
    public class SpotLight
    {
        public Vector3 LightPosition { get; set; }
        public float LightIntensity { get; set; }
        public float LightRadius { get; set; }
        public Vector3 SpotDirection { get; set; }
        public float SpotLightAngle { get; set; }
        public float SpotDecayExponent { get; set; }
        public Vector3 Color { get; set; }
        public bool ShadowsEnabled { get; set; }

        public FrustumCullerView ShadowView { get; set; }

        public SpotLight(FrustumCuller culler)
        {
            LightPosition = new Vector3(0, 6, 0);
            LightRadius = 6;
            LightIntensity = 1;
            SpotDirection = MathHelper.Down;
            SpotLightAngle = MathHelper.ToRadians(30);
            SpotDecayExponent = 1;


            Color = new Vector3(1, 1, 0.9f);
            ShadowView = culler.CreateView();

        }
    }
}

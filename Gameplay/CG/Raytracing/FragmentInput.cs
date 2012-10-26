using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// TODO: convert to abstract factory or normal factory??
    /// </summary>
    public struct FragmentInput
    {
        public bool Clip;
        public Vector3 Position;
        public Vector2 Texcoord;
        public Vector3 Normal;
        public float SpecularPower;
        public float SpecularIntensity;
        public Color4 Diffuse;
        public Color4 SpecularColor;

        public float U;
        public float V;
    }
}
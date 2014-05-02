using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Particles
{
    public class ColoredParticle
    {
        public Vector3 StartPosition;
        public Vector3 StartVelocity = new Vector3(1,0,0);
        public Color4 Color = new Color4(0,1,0);
        public float Size = 1;
        public float Duration = 1;
        public float SpawnTime;
    }
}
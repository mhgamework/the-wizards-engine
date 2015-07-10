using System.Collections.Generic;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;

namespace MHGameWork.TheWizards.Rendering.Particles
{
    public class ParticlesLineRenderer
    {
        private readonly LineManager3D lines;

        public ParticlesLineRenderer(LineManager3D lines)
        {
            this.lines = lines;
        }

        public void RenderEffect(ParticleEffect effect)
        {
            effect.Particles.ForEach(p =>
                {
                    lines.AddCenteredBox(effect.CalculatePosition(p), p.Size, p.Color);
                });
        }
    }
}
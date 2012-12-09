using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Texturing
{
    public class Texture2DGeometrySampler : IGeometrySampler<Color4>
    {
        private Texture2DSampler sampler;
        private Texture2D texture;

        public Texture2DGeometrySampler(Texture2DSampler sampler, Texture2D texture)
        {
            this.sampler = sampler;
            this.texture = texture;
        }

        public override Color4 Sample(TraceResult input)
        {
            return sampler.SampleBilinear(texture, input.Texcoord);
        }
    }
}

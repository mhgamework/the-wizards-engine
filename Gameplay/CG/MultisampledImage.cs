using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Sampling;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class MultisampledImage : IRenderedImage
    {
        private readonly IRenderedImage image;
        private Vector2 cellSize;

        private JitteredSampler sampler;

        public MultisampledImage(IRenderedImage image, Vector2 resolution, JitteredSampler sampler)
        {
            this.image = image;
            this.sampler = sampler;
            cellSize = new Vector2(1f / resolution.X, 1f / resolution.Y);
        }

        public Color4 GetPixel(Vector2 pos)
        {
            var ret = new Vector4();
            foreach (var sample in sampler.GenerateSamples())
            {
                ret += image.GetPixel(Vector2.Modulate(sample, cellSize) + pos).ToVector4();
            }

            return new Color4(ret / (float)sampler.SampleCount);
        }
    }
}

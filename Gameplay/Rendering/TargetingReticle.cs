using System;
using MHGameWork.TheWizards.Rendering;
using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Services
{
    public class TargetingReticle
    {
        public TargetingReticle()
        {
        }

        public void drawReticle()
        {
            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;

            var center = new Vector2(TW.Graphics.Form.Form.ClientSize.Width, TW.Graphics.Form.Form.ClientSize.Height) * 0.5f;
            var reticleSize = new Vector2(32, 32);

            var tex = TW.Assets.LoadTexture("Scattered\\Models\\Reticle.png");

            draw(tex, center - reticleSize * 0.5f, reticleSize);
        }

        private void draw(ITexture tex, Vector2 pos, Vector2 size)
        {
            throw new NotImplementedException();
            //TW.Graphics.TextureRenderer.Draw(TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(tex), new Vector2(pos.X, pos.Y), new Vector2(size.X, size.Y));
        }
    }
}
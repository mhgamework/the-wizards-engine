using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class HudSimulator : ISimulator
    {
        private readonly Level level;
        private Textarea text;
        private ShaderResourceView reticleTextureRv;

        public HudSimulator(Level level)
        {
            this.level = level;
            text = new Textarea();
            text.Position = new Vector2(650, 30);
            text.Size = new Vector2(140, 200);
            var reticleTexture = TW.Assets.LoadTexture("Scattered\\Models\\Reticle.png");
            reticleTextureRv = TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(reticleTexture);



            DI.Get<UISimulator>().SubSimulators.Add(new BasicSimulator(DrawUI));
        }
        public void Simulate()
        {
            text.Text = level.LocalPlayer.Inventory.Items.GroupBy(i => i).Aggregate("Inventory: \n", (acc, el) => acc + el.Count() + " " + el.First().Name + "\n");
        }

        public void DrawUI()
        {
            drawReticle(reticleTextureRv);
        }

        private void drawReticle(ShaderResourceView tex)
        {
            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;
            var center = new Vector2(TW.Graphics.Form.Form.ClientSize.Width, TW.Graphics.Form.Form.ClientSize.Height) * 0.5f;
            var reticleSize = new Vector2(32, 32);
            TW.Graphics.TextureRenderer.Draw(tex, center - reticleSize * 0.5f, reticleSize);
        }
    }
}
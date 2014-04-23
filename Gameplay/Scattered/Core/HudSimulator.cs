using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
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
        private ShaderResourceView grayTextureRv;

        private float right = TW.Graphics.Form.Form.ClientSize.Width;
        private float bottom = TW.Graphics.Form.Form.ClientSize.Height;

        public HudSimulator(Level level)
        {
            this.level = level;
            text = new Textarea();
            text.Position = new Vector2(650, 30);
            text.Size = new Vector2(140, 200);
            var reticleTexture = TW.Assets.LoadTexture("Scattered\\Models\\Reticle.png");
            reticleTextureRv = TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(reticleTexture);

            var grayTexture = TW.Assets.LoadTexture("Scattered\\Models\\Maps\\DarkGrey.png");
            grayTextureRv = TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(grayTexture);


            DI.Get<UISimulator>().SubSimulators.Add(new BasicSimulator(DrawUI));
        }
        public void Simulate()
        {
            text.Text = level.LocalPlayer.Inventory.Items.GroupBy(i => i).Aggregate("Inventory: \n", (acc, el) => acc + el.Count() + " " + el.First().Name + "\n");
        }

        public void DrawUI()
        {
            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;
            drawReticle();
            drawHealthBar(level.LocalPlayer.Health);
        }

        private void drawReticle()
        {

            var center = new Vector2(TW.Graphics.Form.Form.ClientSize.Width, TW.Graphics.Form.Form.ClientSize.Height) * 0.5f;
            var reticleSize = new Vector2(32, 32);

            var tex = TW.Assets.LoadTexture("Scattered\\Models\\Reticle.png");

            draw(tex, center - reticleSize * 0.5f, reticleSize);
        }

        private void drawHealthBar(float health)
        {
            var back = TW.Assets.LoadTexture("Scattered\\HUD\\HealthBarBack.png");
            var front = TW.Assets.LoadTexture("Scattered\\HUD\\HealthBar.png");

            var maxHeight = 150;
            var height = maxHeight * health;

            draw(back, right - 70, 20, 50, maxHeight);
            draw(front, right - 70, 20 + (maxHeight - height), 50, height);

        }

        //private void drawRectangle(Vector2 pos, Vector2 size, int border)
        //{
        //    draw(grayTextureRv, pos.X, pos.Y - border, size.X, border);
        //    draw(grayTextureRv, pos.X, pos.Y + size.Y, size.X, border);

        //    draw(grayTextureRv, pos.X - border, pos.Y, border, size.Y);
        //    draw(grayTextureRv, pos.X + size.X, pos.Y, border, size.Y);

        //    //draw(grayTextureRv, pos.X - border, pos.Y - border, size.X, size.Y);
        //    //draw(grayTextureRv, pos.X, pos.Y - border, size.X, size.Y);
        //    //draw(grayTextureRv, pos.X, pos.Y - border, size.X, size.Y);
        //    //draw(grayTextureRv, pos.X, pos.Y - border, size.X, size.Y);


        //}



        private void draw(ITexture tex, Vector2 pos, Vector2 size)
        {
            draw(TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(tex), pos.X, pos.Y, size.X, size.Y);
        }

        private void draw(ITexture tex, float x, float y, float sizeX, float sizeY)
        {
            draw(TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(tex), x, y, sizeX, sizeY);
        }

        private void draw(ShaderResourceView rv, float x, float y, float sizeX, float sizeY)
        {
            TW.Graphics.TextureRenderer.Draw(rv, new Vector2(x, y), new Vector2(sizeX, sizeY));
        }

    }
}
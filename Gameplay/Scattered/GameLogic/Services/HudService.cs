using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using SlimDX.Direct3D11;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Services
{
    public class HudService 
    {
        private readonly Level level;
        private Textarea text;
        private ShaderResourceView grayTextureRv;

        private float right = TW.Graphics.Form.Form.ClientSize.Width;
        private float bottom = TW.Graphics.Form.Form.ClientSize.Height;

        private Textarea goalText;


        public HudService(Level level)
        {
            this.level = level;
            text = new Textarea();
            text.Position = new Vector2(650, 30);
            text.Size = new Vector2(140, 200);


            goalText = new Textarea(); // FOR SOME UNKNOWN REASON THIS DOESNT WORK???
            goalText.Position = new Vector2(right - 20 - 200, 20 + 200 + 20);
            goalText.Size = new Vector2(120, 150);


            var grayTexture = TW.Assets.LoadTexture("Scattered\\Models\\Maps\\DarkGrey.png");
            grayTextureRv = TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(grayTexture);



            DI.Get<UISimulator>().SubSimulators.Add(new BasicSimulator(DrawUI));
            targetingReticle = new TargetingReticle();
        }
        public void Simulate()
        {
            text.Text = level.LocalPlayer.Inventory.Items.GroupBy(i => i).Aggregate("Inventory: \n", (acc, el) => acc + el.Count() + " " + el.First().Name + "\n");
            text.Text += "\n" + goalText.Text;
        }

        public void DrawUI()
        {
            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;
            targetingReticle.drawReticle();
            drawHealthBar(level.LocalPlayer.Health);
            drawMap();
            updateGoalText();
        }

        private void updateGoalText()
        {

            goalText.Text = "Drones left: " + level.Islands.SelectMany(i => i.Addons.OfType<Enemy>()).Count();

        }

        float mapViewRange = 200f;
        private Vector2 mapPos;
        private Vector2 mapSize = new Vector2(200, 200);
        private readonly TargetingReticle targetingReticle;

        private void drawMap()
        {
            mapPos = new Vector2(right - 20 - mapSize.X, 20);

            drawOnMap(level.LocalPlayer, new Color4(0, 0.5f, 0), 10);

            level.FindInRange<Enemy>(level.LocalPlayer.Node, 200, _ => true)
                .ForEach(e => drawOnMap(e, new Color4(1, 0, 0), 3));

            level.FindInRange<Tower>(level.LocalPlayer.Node, 200, _ => true)
                .ForEach(e => drawOnMap(e, new Color4(0.5f, 0.5f, 0.5f), 8));
        }

        private void drawOnMap(IHasNode hasNode, Color4 color4, float size)
        {
            var localPos = Vector3.TransformCoordinate(hasNode.Node.Position,
                                                       Matrix.Invert(level.LocalPlayer.Node.Absolute));

            var rel = localPos.TakeXZ();
            rel /= mapViewRange;
            rel = (rel + new Vector2(1)) * 0.5f; // 0-1 range
            var pos = mapPos + Vector2.Modulate(mapSize, rel);
            pos -= new Vector2(size * 0.5f);
            TW.Graphics.TextureRenderer.DrawColor(color4, pos, new Vector2(size));
        }


        private void drawHealthBar(float health)
        {
            var back = TW.Assets.LoadTexture("Scattered\\HUD\\HealthBarBack.png");
            var front = TW.Assets.LoadTexture("Scattered\\HUD\\HealthBar.png");

            var x = right - 70;
            var y = 20 + mapPos.Y + mapSize.Y;
            var maxHeight = 150;
            var height = maxHeight * health;

            draw(back, x, y, 50, maxHeight);
            draw(front, x, y + (maxHeight - height), 50, height);

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
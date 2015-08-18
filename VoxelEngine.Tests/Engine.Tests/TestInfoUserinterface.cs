using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.Tests
{
    /// <summary>
    /// Provides information to the user of a test by presenting text in the topright of the screen
    /// </summary>
    public class TestInfoUserinterface
    {
        private readonly DX11Game game;
        public string Text;
        private Textarea area;

        public bool Visible = false;
        public int Width = 300;


        public TestInfoUserinterface(DX11Game game)
        {
            this.game = game;
            area = new Textarea();

            //area.AlignRight = true;

            Text = "No help available";

        }

        public void Update()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F11))
                Visible = !Visible;

            area.Size = new SlimDX.Vector2(Width, 500);
            area.Position = new SlimDX.Vector2(game.Form.Form.ClientSize.Width - 10 - area.Size.X, 10);

            if (Visible)
            {
                area.Text = Text + "\n\nPress F11 to hide help";
                area.AlignRight = false;
            }
            else
            {
                area.AlignRight = true;
                area.Text = "Press F11 for help";
            }
        }
    }
}
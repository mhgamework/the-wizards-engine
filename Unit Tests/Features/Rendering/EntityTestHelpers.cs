using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Default;

namespace MHGameWork.TheWizards.Tests.Rendering
{
    public static class RenderingTestExtensions
    {

        public static void RunTest(this DefaultRenderer renderer)
        {
            XNAGame game = new XNAGame();
            game.AddXNAObject(renderer);

            game.DrawEvent += delegate
                              {
                                  game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
                              };
            game.Run();
        }
    }
}

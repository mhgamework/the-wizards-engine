using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;
using SlimDX;
using TexturePool = MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.TexturePool;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class TexturePoolTest
    {
        [Test]
        public void TestLoadTexture()
        {
            DX11Game game = new DX11Game();
            var pool = new TexturePool(game);

            RAMTexture tex = RenderingTestsHelper.GetTestTexture();

            game.GameLoopEvent += delegate
            {
                //We should do this ACTUALLY in real usage situations, but it proves we cache the data.
                int row = 0;
                int col = 0;
                int width = 10;
                for (int i = 0; i < 100; i++)
                {
                    row = i / width;
                    col = i % width;
                    game.TextureRenderer.Draw(pool.LoadTexture(tex), new Vector2(10 + col * 40, 10 + row * 40), new Vector2(40, 40));
                }


            };

            game.Run();
        }

    }
}
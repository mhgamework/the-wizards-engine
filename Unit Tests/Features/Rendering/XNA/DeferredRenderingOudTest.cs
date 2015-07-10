using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.XNA
{
    [TestFixture]
    public class DeferredRenderingOudTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestDeferredRenderer()
        {
            var game = new XNAGame();

            var renderer = new DeferredRendererOud(DefaultMeshes.CreateMerchantsHouseMeshOLD(), TestFiles.WoodPlanksBareJPG);

            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.Diffuse;

            game.AddXNAObject(renderer);

            game.UpdateEvent += delegate
                                    {
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D1))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.Final;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D2))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.Diffuse;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D3))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.Normal;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D4))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.Depth;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D5))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.LightAccumulation;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D6))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.ShadowMap;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D7))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.AmbientOcclusion;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D8))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.BlurX;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D9))
                                        {
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.BlurY;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.Downsample;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.DownsampleBlurX;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.DownsampleBlurY;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad4))
                                            renderer.OutputMode = DeferredRendererOud.DeferredOutputMode.ToneMapped;
                                    };

            game.Run();
        }
    }
}

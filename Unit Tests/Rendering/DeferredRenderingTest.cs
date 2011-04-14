using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering.Deferred;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Rendering
{
    [TestFixture]
    public class DeferredRenderingTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestDeferredRenderer()
        {
            var game = new XNAGame();

            var renderer = new DeferredRenderer(RenderingTest.CreateMerchantsHouseMeshOLD(), TestFiles.WoodPlanksBareJPG);

            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.Diffuse;

            game.AddXNAObject(renderer);

            game.UpdateEvent += delegate
                                    {
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D1))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.Final;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D2))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.Diffuse;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D3))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.Normal;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D4))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.Depth;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D5))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.LightAccumulation;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D6))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.ShadowMap;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D7))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.AmbientOcclusion;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D8))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.BlurX;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D9))
                                        {
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.BlurY;
                                        }
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad1))
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.Downsample;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.DownsampleBlurX;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad3))
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.DownsampleBlurY;
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.NumPad4))
                                            renderer.OutputMode = DeferredRenderer.DeferredOutputMode.ToneMapped;
                                    };

            game.Run();
        }
    }
}

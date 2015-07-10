﻿using System;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.Deferred;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class GBufferTest
    {
        [Test]
        public void TestGBuffer()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var buffer = new GBuffer(game.Device, 300, 300);

            buffer.Clear();

            game.GameLoopEvent += delegate
                {
                    ( (Action) delegate { } )();
                    DrawGBuffer(game, buffer);
                };

            game.Run();
        }

        public static void DrawGBuffer(DX11Game game, GBuffer buffer)
        {
            game.TextureRenderer.Draw(buffer.DiffuseRV, new Vector2(0, 0),
                                      new Vector2(400, 300));
            game.TextureRenderer.Draw(buffer.NormalRV, new Vector2(400, 00),
                                      new Vector2(400, 300));
            game.TextureRenderer.Draw(buffer.DepthRV, new Vector2(00, 300),
                                      new Vector2(400, 300));
        }

        [Test]
        public void TestRenderToGBuffer()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;


            var filledGBuffer = new DeferredTest.TestFilledGBuffer(game, 600, 600);


            game.GameLoopEvent += delegate
            {
                filledGBuffer.DrawUpdatedGBuffer();

                game.SetBackbuffer();

                DrawGBuffer(game, filledGBuffer.GBuffer);
            };

            game.Run();

        }
    }
}
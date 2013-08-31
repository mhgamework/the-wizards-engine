using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Vegetation.Grass;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using NUnit.Framework;
using Rhino.Mocks;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Vegetation
{
    [TestFixture]
    public class GrassTest
    {
        [Test]
        public void TestSingleGrassMeshDx11()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var device = game.Device;
            var context = device.ImmediateContext;

            var map = MockRepository.GenerateMock<IGrassMap>();
            map.Stub(m => m.getDensity(0, 0)).Return(20);
            map.Stub(m => m.getGrowingHeight(0, 0)).Return(1);
            map.Stub(m => m.getHeight(0, 0)).Return(0);
            map.Stub(m => m.Width).Return(1);
            map.Stub(m => m.Length).Return(1);
            GrassMeshFactory factory = new GrassMeshFactory();
            GrassMeshRenderData renderData = factory.CreateGrassMeshRenderData(game, device, map, 123);



            var GBuffer = new GBuffer(game.Device, 600, 600);


            game.GameLoopEvent += delegate
                {
                    context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                    GBuffer.Clear();
                    GBuffer.SetTargetsToOutputMerger();

                    renderData.Draw(context, game);

                    game.SetBackbuffer();

                    DeferredTest.DrawGBuffer(game, GBuffer);
                };

            game.Run();
        }
    }
}

using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Types;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class RendererTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        //Test broken [Test]
        public void Test()
        {
            var world = createTestWorld();

            engine.AddSimulator(new SimpleWorldRenderer(world));
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private Internal.Model.World createTestWorld()
        {
            var ret = new Internal.Model.World(5, 10);

            var typeRed = new GameVoxelType("Red") { Color = Color.Red };
            var typeGreen = new GameVoxelType("Green") { Color = Color.Green };
            var typeBlue = new GameVoxelType("Blue") { Color = Color.Blue };

            ret.GetVoxel(new Point2(0, 0)).ChangeType(typeRed);
            ret.GetVoxel(new Point2(1, 1)).ChangeType(typeRed);
            ret.GetVoxel(new Point2(2, 1)).ChangeType(typeGreen);
            ret.GetVoxel(new Point2(1, 2)).ChangeType(typeBlue);

            return ret;
        }
    }
}
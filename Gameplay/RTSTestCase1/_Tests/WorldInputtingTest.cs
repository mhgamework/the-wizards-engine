using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class WorldInputtingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private CommandFactory f;
        private WorldInputtingSimulator inputtingSimulator;

        [SetUp]
        public void Setup()
        {
            f = TW.Data.Get<CommandFactory>();
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestShizzle()
        {
            inputtingSimulator = new WorldInputtingSimulator();
            inputtingSimulator.Configuration.Menu.CreateItem("Rivers", enableRivers);
            inputtingSimulator.Configuration.Menu.CreateItem("Walls", enableWalls);
            inputtingSimulator.Configuration.Menu.CreateItem("Tree", enableRocks);
            inputtingSimulator.Configuration.Menu.CreateItem("Rock", enableTrees);

            engine.AddSimulator(inputtingSimulator);
            engine.AddSimulator(new WorldRenderingSimulator());

        }

        [Test]
        public void TestWorldPlacer()
        {
            inputtingSimulator = new WorldInputtingSimulator();
            enableTrees();

            var t = new Tree();
            t.Position = new Vector3(1, 0, 1);
            t = new Tree();
            t.Position = new Vector3(2, 0, 2);

            engine.AddSimulator(inputtingSimulator);
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private void enableTrees()
        {
            var placer = new WorldPlacer
                (
                    getItems: () => TW.Data.Objects.OfType<Tree>(),
                    getPosition: tree => ((Tree)tree).Position,
                    setPosition: (tree, position) => ((Tree)tree).Position = position,
                    getBoundingBox: tree => ((Tree)tree).Physical.GetBoundingBox(),
                    createItem: () => new Tree(),
                    deleteItem: t => TW.Data.RemoveObject((Tree)t)
                );

            inputtingSimulator.Configuration.Placer = placer;


        }

        private void enableRocks()
        {
            throw new System.NotImplementedException();
        }

        private void enableRivers()
        {

        }
        private void enableWalls()
        {

        }


    }
}

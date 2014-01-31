using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
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
    /// <summary>
    /// TODO: test this without the reference to Tree!
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class WorldInputtingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        //private CommandFactory f;
        EditorConfiguration controller = new EditorConfiguration();

        [SetUp]
        public void Setup()
        {
            //f = TW.Data.Get<CommandFactory>();
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestShizzle()
        {
            
            var menu = new EditorMenuConfiguration();
            controller.Menu = menu;

            menu.CreateItem("Rivers", enableRivers);
            menu.CreateItem("Walls", enableWalls);
            menu.CreateItem("Tree", enableRocks);
            menu.CreateItem("Rock", enableTrees);
            
            engine.AddSimulator(new WorldInputtingSimulator(controller));
            engine.AddSimulator(new WorldRenderingSimulator());

        }

        [Test]
        public void TestWorldPlacer()
        {

            DI.Get<TestSceneBuilder>().Setup = delegate
                {
                    var t = new Tree();
                    t.Position = new Vector3(1, 0, 1);
                    t = new Tree();
                    t.Position = new Vector3(2, 0, 2);
                };

            enableTrees();


            engine.AddSimulator(new WorldInputtingSimulator(controller));
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
                    deleteItem: t => TW.Data.RemoveObject((Tree) t)
                );

            controller.Placer = placer;


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


        /// <summary>
        /// TODO: copied from removed rtstestcase and stubbed (not implemented)
        /// 
        /// </summary>
        public class Tree : EngineModelObject
        {
            public Vector3 Position { get; set; }

            public Physical Physical { get; private set; }
        }
    }

}

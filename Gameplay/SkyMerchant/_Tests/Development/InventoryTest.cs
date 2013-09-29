using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using NUnit.Framework;
using Rhino.Mocks;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    [EngineTest]
    [TestFixture]
    public class InventoryTest
    {
        /// <summary>
        /// Tests the basic view using a wireframerenderer
        /// </summary>
        [Test]
        public void TestInventoryView3D()
        {
            var game = EngineFactory.CreateEngine();

            var view = new InventoryView3DTopDown(createRootItem(), new WireframeInventoryNodeRenderer());

            game.AddSimulator(new BasicSimulator(view.Update));
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }

        /// <summary>
        /// Test the FSM in this class, probably split this test?
        /// </summary>
        [Test]
        public void TestInventoryController()
        {
            var game = EngineFactory.CreateEngine();
            var controller = new InventoryController(new HotbarController(null, null), null);

            game.AddSimulator(new BasicSimulator(controller.Update));
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }

        /// <summary>
        /// Shows the actual inventory to use in the quest builder, while rendering meshes
        /// </summary>
        [Test]
        public void TestMeshSpawnerRenderer()
        {
            var game = EngineFactory.CreateEngine();

            var meshRender = new MeshSpawnerInventoryRenderer(new WireframeInventoryNodeRenderer());

            var view = new InventoryView3DTopDown(createInventoryWithMeshSpanwers(), meshRender);

            game.AddSimulator(new BasicSimulator(meshRender.MakeAllInvisible));
            game.AddSimulator(new BasicSimulator(view.Update));
            game.AddSimulator(new PhysicalSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }

        /// <summary>
        /// Shows the actual inventory to use in the quest builder, while rendering meshes
        /// </summary>
        [Test]
        public void TestTextRenderer()
        {
            var game = EngineFactory.CreateEngine();

            var meshRender = new HotBarItemTextInventoryRenderer(new WireframeInventoryNodeRenderer());

            var view = new InventoryView3DTopDown(createInventoryWithHotbaritems(), meshRender);

            game.AddSimulator(new BasicSimulator(meshRender.MakeAllInvisible));
            game.AddSimulator(new BasicSimulator(view.Update));
            game.AddSimulator(new PhysicalSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }

        /// <summary>
        /// Tests the inventory controller (integration test)
        /// </summary>
        [Test]
        public void TestControllerFull()
        {

            var game = EngineFactory.CreateEngine();

            var builder = new DefaultInventoryBuilder(null,null);

            var meshRender = new MeshSpawnerInventoryRenderer(new HotBarItemTextInventoryRenderer(new WireframeInventoryNodeRenderer()));

            var view = new InventoryView3DTopDown(builder.CreateTree(), meshRender);

            var bar = new Hotbar();
            var hotbarController = new HotbarController(bar, new HotbarTextView(bar, new Rendering2DComponentsFactory()));
            var controller = new InventoryController(hotbarController, view);


            game.AddSimulator(new BasicSimulator(meshRender.MakeAllInvisible));
            game.AddSimulator(new BasicSimulator(view.Update));
            game.AddSimulator(new BasicSimulator(controller.Update));
            game.AddSimulator(new BasicSimulator(hotbarController.Update));

            game.AddSimulator(new PhysicalSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }

        private IInventoryNode createRootItem()
        {
            var root = createGroupNode();

            // - Models
            root.AddChild(createGroupNode());
            for (int i = 0; i < 20; i++) ((GroupInventoryNode)root.Children[0]).AddChild(createGroupNode());

            // - Quests
            root.AddChild(createGroupNode());
            for (int i = 0; i < 8; i++) ((GroupInventoryNode)root.Children[1]).AddChild(createGroupNode());
            for (int i = 0; i < 30; i++) ((GroupInventoryNode)root.Children[1].Children[4]).AddChild(createGroupNode());
            for (int i = 0; i < 100; i++) ((GroupInventoryNode)root.Children[1].Children[4].Children[15]).AddChild(createGroupNode());
            for (int i = 0; i < 100; i++) ((GroupInventoryNode)root.Children[1].Children[4].Children[15].Children[45]).AddChild(createGroupNode());



            // - Placeables
            root.AddChild(createGroupNode());
            for (int i = 0; i < 5; i++) ((GroupInventoryNode)root.Children[2]).AddChild(createGroupNode());
            for (int i = 0; i < 5; i++) ((GroupInventoryNode)root.Children[2].Children[4]).AddChild(createGroupNode());

            return root;
        }

        private IInventoryNode createInventoryWithHotbaritems()
        {
            var root = createGroupNode();
            root.AddChild(new HotBarItemInventoryNode(createHotbarItem("Japser")));
            root.AddChild(new HotBarItemInventoryNode(createHotbarItem("Chuptys")));
            root.AddChild(new HotBarItemInventoryNode(createHotbarItem("Bart")));
            root.AddChild(new HotBarItemInventoryNode(createHotbarItem("Michiel")));

            return root;
        }

        private IInventoryNode createInventoryWithMeshSpanwers()
        {
            var root = createGroupNode();
            root.AddChild(new HotBarItemInventoryNode(createMeshSpawner("Core\\Barrel01")));
            root.AddChild(new HotBarItemInventoryNode(createMeshSpawner("Core\\Crate01")));

            return root;
        }

        private IHotbarItem createMeshSpawner(string path)
        {
            return new MeshSpawnerItem(path,null);
        }

        private GroupInventoryNode createGroupNode()
        {
            return new GroupInventoryNode();
        }

        private IHotbarItem createHotbarItem(string name)
        {
            var ret = MockRepository.GenerateStub<IHotbarItem>();
            ret.Stub(o => o.Name).Return(name);
            return ret;
        }
    }
}

﻿using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.Inventory;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    [EngineTest]
    [TestFixture]
    public class InventoryTest
    {
        [Test]
        public void TestInventoryView3D()
        {
            var game = EngineFactory.CreateEngine();

            var view = new InventoryView3D(createRootItem(), new WireframeInventoryNodeRenderer());

            game.AddSimulator(new BasicSimulator(view.Update));
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }

        [Test]
        public void TestInventoryController()
        {
            var game = EngineFactory.CreateEngine();
            var controller = new InventoryController(new HotbarController(null, null));

            game.AddSimulator(new BasicSimulator(controller.Update));
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

        private GroupInventoryNode createGroupNode()
        {
            return new GroupInventoryNode();
        }

    }
}

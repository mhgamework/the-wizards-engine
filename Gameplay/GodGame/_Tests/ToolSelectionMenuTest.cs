using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.ToolSelection;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    public class ToolSelectionMenuTest
    {
        private TWEngine engine;
        private ToolSelectionMenu toolSelectionMenu;


        [SetUp]
        public void Setup()
        {
            engine = EngineFactory.CreateEngine();
            toolSelectionMenu = new ToolSelectionMenu();

            engine.AddSimulator(new BasicSimulator(() =>
            {
                toolSelectionMenu.ProcessUserInput();
            }));

            TW.Graphics.SpectaterCamera.EnableUserInput = false;
        }

        [Test]
        public void TestShowToolSelectionMenu()
        {
            //nothing to do, only initialization...
        }

        [Test]
        public void TestAddItemsToSelectionMenu()
        {
            var itemList = new List<IToolSelectionItem>();
            for (int i = 0; i < 9; i++)
            {
                itemList.Add(new ToolSelectionCategory { DisplayName = "Category " + (i + 1) });
            }

            toolSelectionMenu.Initialize(itemList);
        }

        [Test]
        public void TestSelectCategoryItem()
        {
            var itemList = new List<IToolSelectionItem>();
            for (int i = 0; i < 9; i++)
            {
                var cat = new ToolSelectionCategory { DisplayName = "Category " + (i + 1) };
                for (int j = 0; j < 5; j++)
                {
                    cat.SelectionItems.Add(new ToolSelectionCategory { DisplayName = "Category " + (i + 1) + "." + (j + 1) });
                }
                itemList.Add(cat);
            }
            toolSelectionMenu.Initialize(itemList);
        }

        [Test]
        public void TestToolSelectionMenu()
        {
            //todo: create categories, create tools, add to toolselectionmenu
            throw new NotImplementedException("Simon doesn't know how to make tools!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Rendering.Vegetation.Trees;
using NUnit.Framework;
using SlimDX;
namespace MHGameWork.TheWizards.Tests.Features.Rendering.Vegetation
{
    [TestFixture]
   public  class TreeTest
    {
        [Test]
        public void TestTreeStructureGenerater()
        {
            DX11Game game = new DX11Game();

            TreeStructure treeStruct = new TreeStructure();
            TreeTypeData treeTypeData = TreeTypeData.GetTestTreeType();
            TreeStructureGenerater gen = new TreeStructureGenerater();

                    //treeTypeData = TreeTypeData.LoadFromXML("treeLevel2");
                    treeStruct = gen.GenerateTree(treeTypeData, 1);


            game.GameLoopEvent += delegate
                {
                    treeStruct.Visualize(game, new Vector3(5, 0, 5));
                };
                    game.Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Rendering
{
    [TestFixture]
    public class MeshOptimizerTest
    {
        [Test]
        public void TestOptimizeMesh()
        {
            //TODO: better test

            //var fileName = TWDir.GameData + "\\Core\\TileSet\\ts001sg001.obj";
            var fileName = TWDir.GameData + "\\TileSet01\\GreyBrick_RoofX_01\\GreyBrick_RoofX_01.obj";
            var mesh = MeshLoader.LoadMeshFromObj(new System.IO.FileInfo(fileName));
            var optimizer = new MeshOptimizer();

            var optimized = optimizer.CreateOptimized(mesh);
        }
    }
}

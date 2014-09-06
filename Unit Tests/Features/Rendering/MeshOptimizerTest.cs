using System;
using System.Diagnostics;
using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
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
        [Test]
        public void TestOptimizeMeshManyParts()
        {
            var b = new MeshBuilder();
            b.AddBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            var partMesh = b.CreateMesh();

            var unoptimized = new RAMMesh();
            for (int i = 0; i < 1000; i++)
                MeshBuilder.AppendMeshTo(partMesh, unoptimized, Matrix.Translation(i, 0, 0));

            var optimizer = new MeshOptimizer();

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 10; i++)
            {
                var optimized = optimizer.CreateOptimized(unoptimized);
                Assert.AreEqual(1, optimized.GetCoreData().Parts.Count);

            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalMilliseconds / 10);
        }
    }
}

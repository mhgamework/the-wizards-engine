using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.OBJParser;
using MHGameWork.TheWizards.Voxelization;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Voxelization
{
    [TestFixture]
    public class VoxelizerTest
    {
        [Test]
        public void TestVoxelize()
        {
            // Points layout
            // z
            // ^
            // |
            // 0--> x
            // Resolution= 0.5
            //    43
            //     5
            //
            // 1
            // 02
            //
            //


            var voxelizer = new Voxelizer();
            Vector3[] positions = new Vector3[6];
            positions[0] = new Vector3(1, 1, 1);
            positions[1] = new Vector3(1, 1, 1.5f);
            positions[2] = new Vector3(1.5f, 1, 1);

            positions[3] = new Vector3(3, 1, 3);
            positions[4] = new Vector3(2.5f, 1, 3);
            positions[5] = new Vector3(3, 1, 2.5f);

            int[] indices;
            indices = new[] { 0, 1, 2, 3, 4, 5 };

            var voxels = voxelizer.Voxelize(positions, indices, 1);
            Assert.AreEqual(4, voxels.Length);
            Assert.True(voxels[0, 0, 0]);
            Assert.True(voxels[1, 0, 1]);

            Assert.False(voxels[0, 0, 1]);
            Assert.False(voxels[1, 0, 0]);

        }
        [Test]
        public void TestVoxelizeSingle()
        {
            // Points layout
            // z
            // ^
            // |
            // 0--> x
            // Resolution= 0.5
            // 1
            // 02
            //
            //


            var voxelizer = new Voxelizer();
            Vector3[] positions = new Vector3[3];
            positions[0] = new Vector3(1, 1, 1);
            positions[1] = new Vector3(1, 1, 1.5f);
            positions[2] = new Vector3(1.5f, 1, 1);

            int[] indices;
            indices = new[] { 0, 1, 2 };

            var voxels = voxelizer.Voxelize(positions, indices, 1);
            Assert.AreEqual(1, voxels.Length);
            Assert.True(voxels[0, 0, 0]);

        }

        [Test]
        public void TestVoxelizeMesh()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);

            var light = renderer.CreateDirectionalLight();


            var mesh = OBJParserTest.GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));


            var position = mesh.GetCoreData().Parts[0].MeshPart.GetGeometryData().GetSourceVector3(
             MeshPartGeometryData.Semantic.Position);
            var positions = new Vector3[position.Length];
            for (int i = 0; i < position.Length; i++)
            {
                positions[i] = position[i].dx();

            }

            var builder = new MeshBuilder();

            var voxelizer = new Voxelizer();
            var resolution = 0.01f;
            var ret = voxelizer.Voxelize(mesh, resolution);

            for (int x = 0; x < ret.GetLength(0); x++)
                for (int y = 0; y < ret.GetLength(1); y++)
                    for (int z = 0; z < ret.GetLength(2); z++)
                    {
                        if (!ret[x, y, z]) continue;

                        var scale = 3;

                        builder.AddBox(new Vector3(x, y, z) * resolution * scale, (new Vector3(x, y, z) + MathHelper.One) * resolution * scale);
                    }

            IMesh voxelMesh = builder.CreateMesh();

            var ori = renderer.CreateMeshElement(mesh);
            ori.WorldMatrix = Matrix.Translation(15,0,0);

            var voxel = renderer.CreateMeshElement(voxelMesh);
            voxel.WorldMatrix = Matrix.Translation(0, 0, 0);

            game.GameLoopEvent += delegate
                                  {
                                      renderer.Draw();
                                  };
            game.Run();
        }
    }
}

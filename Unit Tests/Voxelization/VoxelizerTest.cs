using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
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

            var scale = 3;


            for (int x = ret.Min.X; x <= ret.Max.X; x++)
                for (int y = ret.Min.Y; y <= ret.Max.Y; y++)
                    for (int z = ret.Min.Z; z <= ret.Max.Z; z++)
                    {
                        if (!ret[x, y, z]) continue;


                        builder.AddBox(new Vector3(x, y, z) * resolution * scale, (new Vector3(x, y, z) + MathHelper.One) * resolution * scale);
                    }


            IMesh voxelMesh = builder.CreateMesh();

            var ori = renderer.CreateMeshElement(mesh);
            ori.WorldMatrix = Matrix.Scaling(MathHelper.One * scale) * Matrix.Translation(0, 0, 0);

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

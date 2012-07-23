using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tiling;
using MHGameWork.TheWizards.Voxelization;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class TilingTest
    {
        private BoundingBox tileBounding = new BoundingBox(new Vector3(-1.5f, -2, -1.5f), new Vector3(1.5f, 2f, 1.5f));

        [Test]
        public void TestTileEditor()
        {
            var game = new LocalGame();


            game
                .AddSimulator(new TileEditorSimulator())
                .AddSimulator(new RenderingSimulator());


            TW.Model.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.Specator;

            game.Run();
        }

        [Test]
        public void TestTileBoundaryMatchVisual()
        {
            var game = new LocalGame();

            var voxelizer = new Voxelizer();


            var mesh1 = MeshFactory.Load("Core\\TileSet\\ts001sg001");
            var mesh2 = MeshFactory.Load("Core\\TileSet\\ts001icg001");

            var voxels1 = voxelizer.Voxelize(mesh1, TileBoundary.SurfaceResolution);
            var voxels2 = voxelizer.Voxelize(mesh2, TileBoundary.SurfaceResolution);


            var bound1 = TileBoundary.CreateFromMesh(mesh1, tileBounding, TileFace.Front);
            var bound2 = TileBoundary.CreateFromMesh(mesh2, tileBounding, TileFace.Front);


            var builder = new MeshBuilder();

            for (int i = 0; i < bound1.Surface.GetLength(0); i++)
                for (int j = 0; j < bound1.Surface.GetLength(1); j++)
                {
                    if (bound1.Surface[i, j])
                        builder.AddBox(new Vector3(i, j, 0), new Vector3(i + 1, j + 1, 1));
                    if (bound2.Surface[i, j])
                        builder.AddBox(new Vector3(i, j, 2), new Vector3(i + 1, j + 1, 3));
                }

            new WorldRendering.Entity() { Mesh = builder.CreateMesh(), WorldMatrix = Matrix.Identity };

            new WorldRendering.Entity() { Mesh = Voxelizer.CreateVoxelMesh(voxels1), WorldMatrix = Matrix.Scaling(MathHelper.One * TileBoundary.SurfaceResolution) * Matrix.Translation(-10, 0, 0) };
            new WorldRendering.Entity() { Mesh = mesh1, WorldMatrix =Matrix.Translation(-10, 0, 0) };

            new WorldRendering.Entity() { Mesh = Voxelizer.CreateVoxelMesh(voxels2), WorldMatrix = Matrix.Scaling(MathHelper.One * TileBoundary.SurfaceResolution) * Matrix.Translation(-10, 0, 6) };
            new WorldRendering.Entity() { Mesh = mesh2, WorldMatrix = Matrix.Translation(-10, 0, 6) };


            game
              .AddSimulator(new RenderingSimulator());

            game.Run();

        }


        [Test]
        public void TestTileBoundaryMatch()
        {
            var game = new LocalGame();


            var mesh1 = MeshFactory.Load("Core\\TileSet\\ts001sg001");
            var mesh2 = MeshFactory.Load("Core\\TileSet\\ts001icg001");


            var bound1 = TileBoundary.CreateFromMesh(mesh1, tileBounding, TileFace.Back);
            var bound2 = TileBoundary.CreateFromMesh(mesh2, tileBounding, TileFace.Back);

            Assert.True(bound1.Matches(bound2, new TileBoundaryWinding()));

        }



        [Test]
        public void TestTileFaceExtensionsVisual()
        {
            var game = new DX11Game();
            game.InitDirectX();

            game.GameLoopEvent += delegate
                                   {
                                       foreach (var val in Enum.GetValues(typeof(TileFace)))
                                       {
                                           var face = (TileFace)val;
                                           game.LineManager3D.AddLine(face.Normal(), face.Normal() * 2, new Color4(0, 1, 0));
                                           game.LineManager3D.AddLine(face.Normal(), face.Normal() + face.Up(), new Color4(0, 0, 1));
                                           game.LineManager3D.AddLine(face.Normal(), face.Normal() + face.Right(), new Color4(1, 0, 0));
                                       }
                                   };

            game.Run();
        }

    }
}

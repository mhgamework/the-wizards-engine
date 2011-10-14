using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.World;
using MHGameWork.TheWizards.World.Rendering;
using NUnit.Framework;
using SlimDX;
using TexturePool = MHGameWork.TheWizards.Rendering.Deferred.TexturePool;

namespace MHGameWork.TheWizards.Tests
{
    [TestFixture]
    public class WorldTest
    {
        public static string BarrelObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.obj"; } }
        //public static string BarrelMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.mtl"; } }

        [Test]
        public void TestWorldRenderer()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var world = new WorldNoSectors();
            var renderer = new DeferredRenderer(game);
            var worldRenderer = new WorldRenderer(world, renderer);

            var mesh = MeshLoader.LoadMeshFromObj(new FileInfo(BarrelObj));

            world.CreateNewEntity(mesh, Matrix.Translation(new Vector3(5, 0, 2)));

            game.GameLoopEvent += delegate
                                  {
                                      worldRenderer.ProcessWorldChanges();
                                      world.ClearDirty();
                                  };

            game.Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.PhysX;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.VoxelTerraining;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class VoxelTerrainTest
    {
        [Test]
        public void TestSimpleVoxelTerrain()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = createTerrain();

            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }

        private static VoxelTerrain createTerrain()
        {
            var terr = new VoxelTerrain();
            terr.Size = new Point3(10, 10, 10);
            terr.Create();

            var random = new Random();
            for (int i = 0; i < 30; i++)
            {
                terr.GetVoxel(new Point3(random.Next(9), random.Next(9), random.Next(9))).Filled = true;
            }

            terr.GetVoxel(new Point3(1, 1, 1)).Filled = true;
            return terr;
        }

        [Test]
        public void TestBlob()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = createBlob();

            engine.AddSimulator(new VoxelTerrainSimulator());
            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }

        private static VoxelTerrain createBlob()
        {
            var terr = new VoxelTerrain();
            terr.Size = new Point3(20, 20, 20);
            terr.Create();

            for (int x = 1; x < 10; x++)
            {
                for (int y = 1; y < 10; y++)
                {
                    for (int z = 1; z < 10; z++)
                    {
                        terr.GetVoxel(new Point3(x, y, z)).Filled = true;
                    }
                }
            }
            return terr;
        }

        [Test]
        public void TestPlanetSurface()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = new VoxelTerrain();
            int i = 300;
            terr.Size = new Point3(i, 30, i);
            terr.Create();

            for (int x = 1; x < i; x++)
            {
                for (int y = 1; y < 5; y++)
                {
                    for (int z = 1; z < i; z++)
                    {
                        terr.GetVoxel(new Point3(x, y, z)).Filled = true;
                    }
                }
            }

            engine.AddSimulator(new VoxelTerrainSimulator());
            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }

        [Test]
        public void TestTerrainPhysX()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = createBlob();

            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new BarrelShooterSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
            //engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }

        [Test]
        public void TestVoxelEditor()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            createBlob();

            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new BarrelShooterSimulator());
            var playerData = new PlayerData();
            engine.AddSimulator(new LocalPlayerSimulator(playerData));
            engine.AddSimulator(new ThirdPersonCameraSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.ThirdPerson;
            TW.Data.GetSingleton<CameraInfo>().FirstPersonCameraTarget = playerData.Entity;
            

            engine.Run();
        }


    }
}
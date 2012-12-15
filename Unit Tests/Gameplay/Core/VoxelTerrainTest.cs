using System;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.WorldRendering;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;
using NUnit.Framework;
using TreeGenerator.NoiseGenerater;
using TreeGenerator.TerrrainGeneration;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
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

        private static VoxelTerrainChunk createTerrain()
        {
            var terr = new VoxelTerrainChunk();
            terr.Size = new Point3(10, 10, 10);
            terr.Create();

            var random = new Random();
            for (int i = 0; i < 30; i++)
            {
                terr.GetVoxelInternal(new Point3(random.Next(9), random.Next(9), random.Next(9))).Filled = true;
            }

            terr.GetVoxelInternal(new Point3(1, 1, 1)).Filled = true;
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

        private static VoxelTerrainChunk createBlob()
        {
            var terr = new VoxelTerrainChunk();
            terr.Size = new Point3(20, 20, 20);
            terr.Create();

            for (int x = 1; x < 10; x++)
            {
                for (int y = 1; y < 10; y++)
                {
                    for (int z = 1; z < 10; z++)
                    {
                        terr.GetVoxelInternal(new Point3(x, y, z)).Filled = true;
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

            var terr = new VoxelTerrainChunk();
            int i = 300;
            terr.Size = new Point3(i, 30, i);
            terr.Create();

            for (int x = 1; x < i; x++)
            {
                for (int y = 1; y < 5; y++)
                {
                    for (int z = 1; z < i; z++)
                    {
                        terr.GetVoxelInternal(new Point3(x, y, z)).Filled = true;
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
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());

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

        [Test]
        public void TestTranslatedVoxelTerrain()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var terr = createBlob();
            terr.WorldPosition = new Vector3(30, 0, 0);

            terr = createBlob();

            terr.NodeSize = 5;
            terr.WorldPosition = new Vector3(0, 0, 100);

            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();


        }

        [Test]
        public void TestPersistence()
        {
            var engine = new TWEngine();
            engine.Initialize();

            var terr = createBlob();
            terr.WorldPosition = new Vector3(30, 0, 0);

            var file = new FileInfo( TWDir.Test + "\\VoxelTerrainTest_Persistance.txt");
            if (file.Exists) file.Delete();

            TW.Data.GetSingleton<Datastore>().Persist(terr);
            TW.Data.GetSingleton<Datastore>().SaveToFile(file);
            TW.Data.GetSingleton<Datastore>().LoadFromFile(file);

            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }


        [Test]
        public void TestVoxelEditorBig()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            //generateFlat();
            generateTerrain(10, 10);


            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new BarrelShooterSimulator());
            //var playerData = new PlayerData();
            //engine.AddSimulator(new LocalPlayerSimulator(playerData));
            //engine.AddSimulator(new ThirdPersonCameraSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            //engine.AddSimulator(new PhysXDebugRendererSimulator());

            //TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.ThirdPerson;
            //TW.Data.GetSingleton<CameraInfo>().FirstPersonCameraTarget = playerData.Entity;


            engine.Run();
        }

        private static void generateFlat()
        {
            for (int x = -3; x < 3; x++)
            {
                for (int y = -3; y < 3; y++)
                {
                    var terr = new VoxelTerrainChunk();
                    terr.Size = new Point3(16, 30, 16);
                    //terr.Size = new Point3(5, 5, 5);
                    terr.WorldPosition = Vector3.Modulate(terr.Size.ToVector3()*terr.NodeSize, new Vector3(x, 0, y));
                    terr.Create();

                    for (int tx = 0; tx < terr.Size.X; tx++)
                    {
                        for (int ty = 0; ty < terr.Size.Y/2; ty++)
                        {
                            for (int tz = 0; tz < terr.Size.Z; tz++)
                            {
                                terr.GetVoxelInternal(new Point3(tx, ty, tz)).Filled = true;
                            }
                        }
                    }
                }
            }
        }

        public static void generateTerrain(int chunksX, int chunksY)
        {


            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> positionsbase = new List<Vector3>();
            List<Vector3> positionsbaseDifference = new List<Vector3>();

            List<Color> colors = new List<Color>();
            List<Color> colorsbase = new List<Color>();

            int width = 100;
            int height = 100;
            SimpleTerrain terrain;
            SimpleTerrain terrainbase;
            SimpleTerrain terrainbaseDiffernce;


            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float[,] heightDataErrosion = new float[width, height];
            //float[,] heightDataErrosionDiffernce = new float[width, height];



            for (int i = 0; i < (int)(width); i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (noise.GetPerlineNoise(i, j, 8, 0.1f, 0.8f, 0.8f) * 0.8f + noise.GetPerlineNoise(noise.Perturb(i, j, 0.1f, 30).X, noise.Perturb(i, j, 0.1f, 30).Y, 4, 0.2f, 0.5f, 0.5f) * 0.25f) * 70;

                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positionsbase.Add(new Vector3(i, heightData[i, j], j));
                    colorsbase.Add(Color.White);
                }
            }
            //heightDataErrosionDiffernce = heightData
            heightDataErrosion = gen.GenerateHydrolicErrosion(heightData, 50 * width * height, width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightDataErrosion[i, j], j));
                    colors.Add(new Color(new Microsoft.Xna.Framework.Vector3(positions[i * width + j].Y - positionsbase[i * width + j].Y, 50, 50)));
                    //positionsbaseDifference.Add(new Vector3(i, positions[i * width + j].Y - positionsbase[i * width + j].Y, j));(
                }
            }


            for (int x = 0; x < chunksX; x++)
            {
                for (int y = 0; y < chunksY; y++)
                {
                    var terr = new VoxelTerrainChunk();
                    terr.Size = new Point3(16, 64, 16);
                    //terr.Size = new Point3(5, 5, 5);
                    terr.WorldPosition = Vector3.Modulate(terr.Size.ToVector3() * terr.NodeSize, new Vector3(x, 0, y));
                    terr.Create();

                    for (int tx = 0; tx < terr.Size.X; tx++)
                    {
                        for (int ty = 0; ty < terr.Size.Y / 2; ty++)
                        {
                            for (int tz = 0; tz < terr.Size.Z; tz++)
                            {
                                var heightMapX = tx + (int) terr.WorldPosition.X;
                                var heightMapZ = tz + (int) terr.WorldPosition.Z;
                                if (heightMapX >= width || heightMapZ >= height) continue;
                                if (ty < heightDataErrosion[heightMapX, heightMapZ]/2f)
                                    terr.GetVoxelInternal(new Point3(tx, ty, tz)).Filled = true;
                            }
                        }
                    }
                }
            }

        }

    }
}
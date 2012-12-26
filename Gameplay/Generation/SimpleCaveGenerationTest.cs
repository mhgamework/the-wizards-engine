using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;
using BoundingBox = SlimDX.BoundingBox;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.Generation
{
    [TestFixture]
    [EngineTest]
    public class SimpleCaveGenerationTest
    {
        [Test]
        public void TestSimple()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var grid = new VoxelGrid<bool>(new Point3(100, 30, 100));

            var gen = new SimpleCaveGenerator();

            gen.GenerateRandom(grid, 0.35f);
            for (int i = 0; i < 5; i++)
            {
                gen.ProcessCellularAutomata(grid);

            }

            new VoxelTerrainConvertor().SetTerrain(grid.ToArray());

            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }

        [Test]
        public void TestRoom()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var grid = new VoxelGrid<bool>(new Point3(50, 40, 50));

            var room = new BoundingBox(new Vector3(20, 20, 20), new Vector3(30, 30, 30));


            var gen = new SimpleCaveGenerator();
            gen.Rand = new Random();
            gen.GenerateRandom(grid, 0.30f);
            
            gen.CreateRoom(grid,room);

            for (int i = 0; i < 5; i++)
                gen.ProcessCellularAutomata(grid);

            new VoxelTerrainConvertor().SetTerrain(grid.ToArray());

            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }

        [Test]
        public void TestMultipleRooms()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var grid = new VoxelGrid<bool>(new Point3(50, 40, 50));



            var gen = new SimpleCaveGenerator();
            gen.Rand = new Random();

            grid.ForEach((x,y,z)=> grid[x,y,z] = true);
            //gen.GenerateRandom(grid, 0.30f);
            //gen.FillBorders(grid);

            gen.GenerateRooms(grid);
            

            //for (int i = 0; i < 5; i++)
            //    gen.ProcessCellularAutomata(grid);

            new VoxelTerrainConvertor().SetTerrain(grid.ToArray());

            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }

        [Test]
        public void TestMultipleRoomsConnect()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var grid = new VoxelGrid<bool>(new Point3(50, 40, 50));



            var gen = new SimpleCaveGenerator();
            gen.Rand = new Random();

            grid.ForEach((x, y, z) => grid[x, y, z] = true);
            gen.GenerateRandom(grid, 0.10f);
            //gen.FillBorders(grid);

            gen.GenerateRooms(grid);


            for (int i = 0; i < 4; i++)
                gen.ProcessCellularAutomata(grid);

            new VoxelTerrainConvertor().SetTerrain(grid.ToArray());

            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }
    }
}

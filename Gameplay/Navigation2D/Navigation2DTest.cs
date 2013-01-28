using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Debugging;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Navigation2D
{
    [TestFixture]
    [EngineTest]
    public class Navigation2DTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private Random random = new Random(4);

        [Test]
        public void TestNavigableGrid2DRandom()
        {
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            for (int i = 0; i < 20; i++)
            {
                new Engine.WorldRendering.Entity()
                {
                    Solid = true,
                    Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                    WorldMatrix = Matrix.Translation((float)random.NextDouble() * 10, 0, (float)random.NextDouble() * 10)
                };
            }

        }

        [Test]
        public void TestNavigableGrid2DSolid()
        {
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            new Engine.WorldRendering.Entity()
                {
                    Solid = true,
                    Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                    WorldMatrix = Matrix.Translation(2, 0, 2)
                };

            new Engine.WorldRendering.Entity()
            {
                Solid = false,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(4, 0, 4)
            };
        }

        [Test]
        public void TestNavigableGrid2DMove()
        {
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            new Engine.WorldRendering.Entity()
            {
                Solid = true,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(2, 0, 2)
            };

            new Engine.WorldRendering.Entity()
            {
                Solid = false,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(4, 0, 4)
            };
        }


        [Test]
        public void TestPathFinder2D()
        {

            //engine.AddSimulator(new AttachDebuggerOnStartupSimulator());
            engine.AddSimulator(new BarrelPlacerSimulator());
            engine.AddSimulator(new PathRendererSimulator());

            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());
            //engine.AddSimulator(new ProfilerSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            new Engine.WorldRendering.Entity()
            {
                Solid = true,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(2, 0, 2)
            };

            new Engine.WorldRendering.Entity()
            {
                Solid = true,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(4, 0, 4)
            };
        }

        [Test]
        public void TestPathFinder2DReal()
        {

            //engine.AddSimulator(new AttachDebuggerOnStartupSimulator());
            engine.AddSimulator(new BarrelPlacerSimulator());
            engine.AddSimulator(new PathRendererSimulator());

            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            TW.Data.GetSingleton<NavigableGrid2DData>().NodeSize = 0.25f;
            TW.Data.GetSingleton<NavigableGrid2DData>().Size = 100;

            TW.Data.GetSingleton<PathRendererSimulator.MyData>().End = new Vector2(99, 99);

            //engine.AddSimulator(new ProfilerSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            new Engine.WorldRendering.Entity()
            {
                Solid = true,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(2, 0, 2)
            };

            new Engine.WorldRendering.Entity()
            {
                Solid = true,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(4, 0, 4)
            };
        }

        public class MoveBarrelSimulator : ISimulator
        {
            private Data data = TW.Data.GetSingleton<Data>();
            private Vector3 start = new Vector3(4, 0, 1);
            private Vector3 end = new Vector3(4, 0, 9);
            public void Simulate()
            {
                if (data.Entity == null)
                    data.Entity = new Engine.WorldRendering.Entity()
                     {
                         Solid = true,
                         Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                         WorldMatrix = Matrix.Translation(start)
                     };

                data.Time += TW.Graphics.Elapsed;


            }

            [ModelObjectChanged]
            public class Data : EngineModelObject
            {
                public Engine.WorldRendering.Entity Entity { get; set; }
                public float Time { get; set; }
                public int Direction { get; set; }
            }
        }

        /// <summary>
        /// TODO: design flaw ==> dont make simulators reusable, put the reusable featuers in a seperate class!!
        /// </summary>
        public class PathRendererSimulator : ISimulator
        {
            public MyData Data { get; private set; }
            private bool first = true;
            public Vector2 End;


            public PathRendererSimulator()
            {
                Data = TW.Data.GetSingleton<MyData>();
            }
            public void Simulate()
            {
                if (!first) return;
                //first = false;
                var grid = TW.Data.GetSingleton<NavigableGrid2DData>().Grid;
                if (grid == null) return;
                var p = new PathFinder2D<Vertex2D>();
                var gridConnectionProvider = new GridConnectionProvider() { Grid = grid };
                p.ConnectionProvider = gridConnectionProvider;


                var start = gridConnectionProvider.GetVertex(Data.Start / grid.NodeSize);
                var goal = gridConnectionProvider.GetVertex(Data.End / grid.NodeSize);


                TW.Graphics.LineManager3D.AddCenteredBox(new Vector3(start.Position.X+0.5f,0,start.Position.Y+0.5f)*grid.NodeSize, grid.NodeSize * 0.5f, new Color4(0, 1, 0));
                TW.Graphics.LineManager3D.AddCenteredBox(new Vector3(goal.Position.X+0.5f, 0, goal.Position.Y+0.5f) * grid.NodeSize, grid.NodeSize * 0.5f, new Color4(1, 0, 0));


                var path = p.FindPath(start, goal);
                Vertex2D prev = null;
                if (path == null) return;

                foreach (var node in path)
                {
                    if (prev != null)
                        TW.Graphics.LineManager3D.AddLine(
                            new Vector3(prev.Position.X + 0.5f, 0, prev.Position.Y + 0.5f) * grid.NodeSize,
                            new Vector3(node.Position.X + 0.5f, 0, node.Position.Y + 0.5f) * grid.NodeSize,
                            new Color4(1, 0, 0));
                    prev = node;
                }
            }

            [ModelObjectChanged]
            public class MyData : EngineModelObject
            {
                public MyData()
                {
                    Start = new Vector2(1, 1);
                    End = new Vector2(9, 9);
                }
                public Vector2 Start { get; set; }
                public Vector2 End { get; set; }
            }
        }
        /// <summary>
        /// TODO: design flaw ==> dont make simulators reusable, put the reusable featuers in a seperate class!!
        /// </summary>
        [PersistanceScope]
        public class BarrelPlacerSimulator : ISimulator
        {
            public void Simulate()
            {
                var ray = TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay();

                if (TW.Graphics.Keyboard.IsKeyDown(Key.F))
                {
                    var plane = new Plane(new Vector3(0, 1, 0), 0);
                    var dist = ray.xna().Intersects(plane.xna());
                    if (dist.HasValue)
                        new Engine.WorldRendering.Entity
                        {
                            WorldMatrix = Matrix.Translation(ray.Position + ray.Direction * dist.Value),
                            Solid = true,
                            Mesh = TW.Assets.LoadMesh("Core\\Barrel01")
                        };
                }
                if (TW.Graphics.Keyboard.IsKeyDown(Key.G))
                {
                    var result = TW.Data.GetSingleton<Engine.WorldRendering.World>().Raycast(ray);
                    if (result.IsHit) TW.Data.RemoveObject((Engine.WorldRendering.Entity)result.Object);
                }
            }
        }
    }
}

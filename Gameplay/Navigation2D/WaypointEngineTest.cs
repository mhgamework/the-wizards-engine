using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ApplicationLogic.EntityOud;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Debugging;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Navigation2D
{
    [TestFixture]
    [EngineTest]
    public class WaypointEngineTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        private Random random = new Random(5);
        private readonly TestUtilities testUtilities = new TestUtilities();

        [Test]
        public void TestSetup()
        {
            //TestUtilities.CreateGroundPlane();

            setupMaze();

            TW.Data.GetSingleton<Navigation2DTest.PathRendererSimulator.MyData>().Start = new Vector2(16, 7);
            TW.Data.GetSingleton<Navigation2DTest.PathRendererSimulator.MyData>().End = new Vector2(8, 9);

            engine.AddSimulator(new Navigation2DTest.BarrelPlacerSimulator());
            engine.AddSimulator(new Navigation2DTest.PathRendererSimulator());
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());

            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private void setupMaze()
        {
            var bb = new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 1, 50));
            bb.IterateCells(2, delegate(int x, int y, int z)
                {
                    if (!(random.NextDouble() > 0.80)) return;
                    int height = 1;
                    int width = 1;

                    if (random.NextDouble() > 0.5)
                    {
                        width = random.Next(1, 5);
                    }
                    else
                    {
                        height = random.Next(1, 5);
                    }

                    new Entity()
                        {
                            Mesh = TW.Assets.LoadMesh("Core\\Building\\BasicTestBlock"),
                            WorldMatrix = Matrix.Translation(0.5f, 0, 0.5f) * Matrix.Scaling(width, 1, height) * Matrix.Translation((x + 0.5f) * 2, y, (z + 0.5f) * 2),
                            Solid = true
                        };
                });

            new Entity()
            {
                Mesh = TW.Assets.LoadMesh("Core\\Building\\BasicTestBlock"),
                WorldMatrix = Matrix.Translation(0.5f, 0, 0.5f) *
                Matrix.Scaling(1, 1, bb.Maximum.Z) * Matrix.Translation(0, 0, 0),
                Solid = true
            };
            new Entity()
            {
                Mesh = TW.Assets.LoadMesh("Core\\Building\\BasicTestBlock"),
                WorldMatrix = Matrix.Translation(0.5f, 0, 0.5f) *
                Matrix.Scaling(1, 1, bb.Maximum.Z) * Matrix.Translation(bb.Maximum.X, 0, 0),
                Solid = true
            };

            new Entity()
            {
                Mesh = TW.Assets.LoadMesh("Core\\Building\\BasicTestBlock"),
                WorldMatrix = Matrix.Translation(0.5f, 0, 0.5f) *
                Matrix.Scaling(bb.Maximum.X, 1, 1) * Matrix.Translation(0, 0, 0),
                Solid = true
            };
            new Entity()
            {
                Mesh = TW.Assets.LoadMesh("Core\\Building\\BasicTestBlock"),
                WorldMatrix = Matrix.Translation(0.5f, 0, 0.5f) *
                Matrix.Scaling(bb.Maximum.X-5, 1, 1) * Matrix.Translation(0, 0, bb.Maximum.Z),
                Solid = true
            };

            TW.Data.GetSingleton<NavigableGrid2DData>().Size = 100;
            TW.Data.GetSingleton<NavigableGrid2DData>().NodeSize = 1;
        }

        [Test]
        public void TestGoblinExploration()
        {
            //TestUtilities.CreateGroundPlane();

            setupMaze();

            var g = new Goblin() { Goal = new Vector3(9, 0, 9), Position = new Vector3(9, 0, 9) };

            g = new Goblin() { Goal = new Vector3(30, 0, 9), Position = new Vector3(30, 0, 9) };
            g = new Goblin() { Goal = new Vector3(9, 0, 30), Position = new Vector3(9, 0, 30) };

            engine.AddSimulator(new GoblinExplorationSimulator());

            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());

            engine.AddSimulator(new GoblinMovementSimulatorSimple());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WaypointVisualizer());

            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestWaypointEngine()
        {
            //TestUtilities.CreateGroundPlane();

            setupMaze();

            var g = new Goblin() { Goal = new Vector3(9, 0, 9), Position = new Vector3(9, 0, 9) };

            g = new Goblin() { Goal = new Vector3(30, 0, 9), Position = new Vector3(30, 0, 9) };
            g = new Goblin() { Goal = new Vector3(9, 0, 30), Position = new Vector3(9, 0, 30) };

            engine.AddSimulator(new GoblinExplorationSimulator());

            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());

            engine.AddSimulator(new GoblinMovementSimulatorSimple());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WaypointVisualizer());

            //engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }




        public class GoblinExplorationSimulator : ISimulator
        {
            public float WaypointDistance { get; set; }
            private readonly GridConnectionProvider provider;
            private float interval = 1;

            public GoblinExplorationSimulator()
            {
                WaypointDistance = 5;
                provider = new GridConnectionProvider();

            }
            public void Simulate()
            {
                if (initGrid()) return;

                explore();

                processWaypoints();
            }

            private void processWaypoints()
            {



                foreach (Waypoint wp in TW.Data.GetChangesOfType<Waypoint>().Where(c => c.Change == ModelChange.Added).Select(c => c.ModelObject))
                {
                    var start = provider.GetVertex(wp.Position);

                    foreach (Waypoint wpTo in getWaypoints())
                    {
                        if (wpTo == wp) continue;
                        
                        var goal = provider.GetVertex(wpTo.Position);

                        var finder = new PathFinder2D<Vertex2D> { ConnectionProvider = provider };
                        finder.StopCondition = n => finder.GetCurrentCost(n) > WaypointDistance * 2f;
                        var path = finder.FindPath(start, goal);
                        if (path == null) continue;
                        if (path.Last() != goal) continue;

                        var edge = new Waypoint.Edge { Target = wpTo, Distance = finder.GetCurrentCost(path[path.Count - 2]) };
                        wp.Edges.Add(edge);
                    }
                }
            }

            private void explore()
            {
                var goblins = TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().ToArray();
                foreach (Goblin g in goblins)
                    explore(g);
            }

            private bool initGrid()
            {
                var grid = TW.Data.GetSingleton<NavigableGrid2DData>().Grid;
                if (grid == null) return true;
                provider.Grid = grid;
                return false;
            }

            private void explore(Goblin g)
            {
                if (g.IsMoving) return;

                var pos = getGoblinVertex(g);

                if (minWaypointDistance(pos) > WaypointDistance)
                    insertNewWaypoint(pos);
                var nextPos = GetExplorationNeighbour(pos);
                if (nextPos == null) return;
                moveGoblinTo(g, nextPos);
            }

            private void insertNewWaypoint(Vertex2D pos)
            {
                TW.Data.GetSingleton<ExplorationData>().Waypoints.Add(new Waypoint() { Position = pos.Position });
            }

            private Vertex2D getGoblinVertex(Goblin g)
            {
                return provider.GetVertex(new Vector2(g.Position.X, g.Position.Z));
            }

            private void moveGoblinTo(Goblin modelObject, Vertex2D nextPos)
            {
                modelObject.Goal = new Vector3(nextPos.Position.X, 0, nextPos.Position.Y);
            }


            public Vertex2D GetExplorationNeighbour(Vertex2D current)
            {
                var currDist = minWaypointDistance(current);
                var finder = new PathFinder2D<Vertex2D>
                    {
                        ConnectionProvider = new GridConnectionProvider() { Grid = provider.Grid, Heuristic = (a, b) => 0 },
                        StopCondition = n => minWaypointDistance(n) > WaypointDistance || Vector2.Distance(n.Position, current.Position) > 20

                    };

                var path = finder.FindPath(current, new Vertex2D(new Vector2(4856984, 45684984)));
                if (minWaypointDistance(current) > minWaypointDistance(path.Last())) return null;
                if (path == null) return null;
                return path.Last();

                //return provider.GetConnectedNodes(current).OrderBy(o => -minWaypointDistance(o)).FirstOrDefault();
            }

            private float minWaypointDistance(Vertex2D arg)
            {
                //TODO: this should be walk distance, not fly distance
                if (!getWaypoints().Any()) return float.MaxValue;
                return getWaypoints().Min(w => Vector2.Distance(arg.Position, w.Position));
            }

            private IEnumerable<Waypoint> getWaypoints()
            {
                return TW.Data.GetSingleton<ExplorationData>().Waypoints;
            }
        }
    }
}

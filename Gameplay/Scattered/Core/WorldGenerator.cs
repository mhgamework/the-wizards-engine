using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered._Engine;
using ProceduralBuilder.Building;
using SlimDX;
using DirectX11;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class WorldGenerator
    {
        private readonly Level level;
        private readonly Random random;

        public WorldGenerator(Level level, Random random)
        {
            this.level = level;
            this.random = random;

        }

        private float averageIslandSize = 40;
        private int distanceBetweenIslands = 40;
        private int nbIslandsPerCluster = 10;

        public void Generate()
        {
            Console.WriteLine("Generating world...");
            generateClusters(new Vector2());
            //generateClusters(new Vector2(1, 0));

            Console.WriteLine("Generating meshes...");

            generateIslandMeshes();

            Console.WriteLine("Relaxing island positions ...");

            relaxPosition();

            generateResources(5, 20, level.CoalType, 0.2f);
            generateAddons(0.1f, new Vector3(6, 0, 6).CenteredBoundingbox(), (island, pos) =>
                {
                    island.AddAddon(
                        new Tower(level, island.Node.CreateChild()).Alter(k => k.Node.Relative = Matrix.Translation(pos)));
                });
            generateAddons(0.05f, new Vector3(10, 0, 2).CenteredBoundingbox(), (island1, pos1) =>
                {
                    island1.AddAddon(
                        new FlightEngine(level, island1.Node.CreateChild(), level.CoalType).Alter(k => k.Node.Relative = Matrix.Translation(pos1)));
                });

            var rockMesh = TW.Assets.LoadMesh("Scattered\\Models\\Resources\\RockFormation01");
            var rockBB = new Vector3(8, 0, 8).CenteredBoundingbox();
            generateAddons(0.5f, rockBB, (island, pos) => island.AddAddon(
                new MeshAddon(rockMesh, level, island.Node.CreateChild()).Alter(k => k.Node.Relative = Matrix.Translation(pos))));

            generateJumpPads(0.5f);

            /*var treeMesh = TW.Assets.LoadMesh("Scattered\\Models\\Resources\\Tree");
            var treeBB = new Vector3(5, 0, 5).CenteredBoundingbox();
            generateAddons(0.5f, treeBB, (island, pos) => island.AddAddon(
                new MeshAddon(treeMesh, level, island.Node.CreateChild()).Alter(k => k.Node.Relative = Matrix.Translation(pos))));
            generateAddons(0.5f, treeBB, (island, pos) => island.AddAddon(
                new MeshAddon(treeMesh, level, island.Node.CreateChild()).Alter(k => k.Node.Relative = Matrix.Translation(pos))));*/
        }

        private void generateJumpPads(float islandPercentage)
        {
            var allPads = new List<JumpPad>();
            var nbJumpPads = (int)Math.Floor(level.Islands.Count() * islandPercentage);
            var rnd = new Random(0);

            for (int j = 1; j < nbJumpPads; j++)
            {
                var index = rnd.Next(0, level.Islands.Count());
                var isle = level.Islands.ElementAt(index);
                var pos = isle.SpaceManager.GetBuildPosition(JumpPad.GetLocalBoundingBox());

                if (pos == null) continue;

                isle.SpaceManager.TakeBuildingSpot(pos.Value, JumpPad.GetLocalBoundingBox());
                var jumpPad = new JumpPad(level, isle.Node.CreateChild()).Alter(k => k.Node.Relative = Matrix.Translation((Vector3)pos));
                isle.AddAddon(jumpPad);
                allPads.Add(jumpPad);
            }

            #region padTargetting
            foreach (var pad in allPads)
            {
                Vector3 padPos;
                Vector3 s;
                Quaternion r;
                pad.Node.Absolute.Decompose(out s, out r, out padPos);

                for (int i = allPads.IndexOf(pad) + 1; i < allPads.Count; i++)
                {
                    Vector3 tPadPos;
                    allPads[i].Node.Absolute.Decompose(out s, out r, out tPadPos);
                    if (Vector3.Distance(padPos, tPadPos) <= pad.MaxJumpDistance)
                    {
                        pad.TargetJumpPad = allPads[i];
                        break;
                    }
                }

                if (pad.TargetJumpPad != null)
                    continue;

                for (int i = 0; i < allPads.IndexOf(pad); i++)
                {
                    Vector3 tPadPos;
                    allPads[i].Node.Absolute.Decompose(out s, out r, out tPadPos);
                    if (Vector3.Distance(padPos, tPadPos) <= pad.MaxJumpDistance)
                    {
                        pad.TargetJumpPad = allPads[i];
                        break;
                    }
                }
            }
            #endregion padTargetting
        }

        private void generateAddons(float appearanceRatio, BoundingBox bb, Action<Island, Vector3> create)
        {
            var shuffledIslands = level.Islands.OrderBy(_ => random.Next()).ToArray();
            var addonsLeft = appearanceRatio * shuffledIslands.Length;

            foreach (var island in shuffledIslands)
            {
                if (addonsLeft < 1) return;

                var pos = island.SpaceManager.GetBuildPosition(bb);
                if (pos == null) continue;

                addonsLeft--;
                island.SpaceManager.TakeBuildingSpot(pos.Value, bb);

                //Action<Island> addBridge = il => il.AddAddon(new Bridge(level, il.Node.CreateChild()).Alter(b => b.Node.Relative = Matrix.Translation(0, 0, 8)));
                //Action<Island> addBridge2 = il => il.AddAddon(new Bridge(level, il.Node.CreateChild()).Alter(b =>
                //b.Node.Relative = Matrix.RotationY(MathHelper.Pi) * Matrix.Translation(0, 0, -6)));

                create(island, pos.Value);
                //isl.AddAddon(new FlightEngine(level, isl.Node.CreateChild(), level.CoalType));

                //addBridge(isl);
                //addBridge2(isl);

                //}
                //else if (random.NextDouble() < (1 / 10f))
                //{
                //isl.AddAddon(new Storage(level, isl.Node.CreateChild()));
                //}


            }
        }

        private void generateResources(int minAmount, int maxAmount, ItemType type, float ratioResourcesOverIslands)
        {
            var islands = level.Islands.ToArray();
            for (int i = 0; i < islands.Count() * ratioResourcesOverIslands; i++)
            {
                var island = islands[random.Next(0, islands.Length)];

                var boundingBox = new Vector3(3, 0, 3).CenteredBoundingbox();
                var pos = island.SpaceManager.GetBuildPosition(boundingBox);

                if (pos == null) continue;

                island.SpaceManager.TakeBuildingSpot(pos.Value, boundingBox);

                var r = new Resource(level, island.Node.CreateChild(), type);
                r.Node.Relative = Matrix.Translation(pos.Value);
                r.Amount = random.Next(minAmount, maxAmount);

                island.AddAddon(r);
            }
        }

        private void generateClusters(Vector2 offset)
        {
            int numCells = 8;

            var cellSize = 2 * averageIslandSize;
            offset *= numCells * cellSize;

            var sampler = new StratifiedSampler(random, numCells);
            var nbClusters = 6;
            for (int i = 0; i < nbClusters; i++)
            {
                var pos = offset + (sampler.Sample() * cellSize);
                GenerateCluster(pos);
            }
        }

        private void generateIslandMeshes()
        {
            var islandGenerator = new CachedIslandGenerator(new IslandGenerator(), new OBJExporter());
            var realtimeIslandGenerator = new IslandGenerator();

            Console.WriteLine("Generating island bases");
            level.Islands.ForEach(i =>
                                      {
                                          var desc = i.Descriptor;
                                          desc.BaseElements = islandGenerator.GetIslandBase(i.Descriptor.seed);

                                          IMesh temp;
                                          List<IBuildingElement> navMesh;
                                          List<IBuildingElement> buildmesh;
                                          List<IBuildingElement> bordermesh;
                                          realtimeIslandGenerator.GetIslandParts(desc.BaseElements, desc.seed, false, out temp, out navMesh, out buildmesh, out bordermesh);

                                          i.SpaceManager.BuildAreaMeshes = buildmesh;
                                          desc.BuildMesh = buildmesh;
                                          desc.NavMesh = navMesh;

                                          i.Descriptor = desc;

                                      });

            Console.WriteLine("Generating island meshes");
            level.Islands.ForEach(i =>
                            {
                                var desc = i.Descriptor;
                                var mesh = islandGenerator.GetIslandMesh(desc.BaseElements, desc.seed);
                                i.Mesh = mesh;
                                i.Descriptor = desc;
                            });
        }

        private void relaxPosition()
        {

            for (int i = 0; i < 10; i++)
            {
                foreach (var a in level.Islands)
                    foreach (var b in level.Islands)
                    {
                        if (a == b) continue;
                        if (Vector3.Distance(a.Position, b.Position) > averageIslandSize * 1.2f) continue;
                        var dir = a.Position - b.Position;
                        dir.Normalize();
                        a.Position += dir;
                        b.Position -= dir;

                    }
            }
        }

        public void GenerateCluster(Vector2 center)
        {
            var sampler = new StratifiedSampler(random, 6);


            for (int i = 0; i < nbIslandsPerCluster; i++)
            {
                var pos = (center + (sampler.Sample() * distanceBetweenIslands)).ToXZ();
                pos.Y = random.Next(-2, 3) * 5;
                var isl = level.CreateNewIsland(pos);
                var desc = new IslandDescriptor();
                desc.seed = random.Next(0, 30);
                isl.Descriptor = desc;

                isl.Node.Relative = Matrix.RotationY((float)random.Next(8) * MathHelper.PiOver4) * isl.Node.Relative;


            }
        }
        public struct IslandDescriptor
        {
            public int seed;
            public List<IBuildingElement> BaseElements;
            public List<IBuildingElement> NavMesh;
            public List<IBuildingElement> BuildMesh;
        }


    }

    public class StratifiedSampler
    {
        private readonly Random random;

        public StratifiedSampler(Random random, int size)
        {
            this.random = random;
            stratifiedSize = size;
            samples = generateSamples();
        }
        private int[] generateSamples()
        {
            var ret = Enumerable.Range(0, stratifiedSize * stratifiedSize - 1).ToArray();
            for (int i = 0; i < ret.Length * 10; i++)
            {
                var a = random.Next(0, ret.Length - 1);
                var b = random.Next(0, ret.Length - 1);
                var swap = ret[a];
                ret[a] = ret[b];
                ret[b] = swap;
            }
            return ret;
        }
        private int[] samples;

        private int samplePos = 0;
        private int stratifiedSize = 32;

        /// <summary>
        /// Returns between 0 and stratifiedsize;
        /// </summary>
        public Vector2 Sample()
        {
            var sample = samples[samplePos];
            float x = sample / stratifiedSize;
            float y = sample % stratifiedSize;
            samplePos = (samplePos + 1) % samples.Length;

            x += (float)random.NextDouble();
            y += (float)random.NextDouble();
            return new Vector2(x, y);
        }
    }


}

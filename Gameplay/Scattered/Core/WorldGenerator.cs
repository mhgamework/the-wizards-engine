using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private float averageIslandSize = 35;
        private int distanceBetweenIslands = 35;
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
        }

        private void generateClusters(Vector2 offset)
        {
            int numCells = 16;

            var cellSize = 2 * averageIslandSize;
            offset *= numCells * cellSize;

            var sampler = new StratifiedSampler(random, numCells);
            var nbClusters = 1;
            for (int i = 0; i < nbClusters; i++)
            {
                var pos = offset + (sampler.Sample() * cellSize);
                GenerateCluster(pos);
            }
        }

        private void generateIslandMeshes()
        {
            var islandGenerator = new CachedIslandGenerator(new IslandGenerator(), new OBJExporter());

            Console.WriteLine("Generating island bases");
            level.Islands.ForEach(i =>
                                      {
                                          var desc = i.Descriptor;
                                          desc.BaseElements = islandGenerator.GetIslandBase(i.Descriptor.seed);

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

            for (int i = 0; i < 20; i++)
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
                var isl = level.CreateNewIsland(pos);
                var desc = new IslandDescriptor();
                desc.seed = random.Next(0, 5);
                isl.Descriptor = desc;

                isl.Node.Relative = Matrix.RotationY((float)random.NextDouble() * 10) * isl.Node.Relative;

                if (random.NextDouble() < (1 / 20f))
                {
                    Action<Island> addBridge = il => il.AddAddon(new Bridge(level, il.Node.CreateChild()).Alter(b => b.Node.Relative = Matrix.Translation(0, 0, 8)));
                    Action<Island> addBridge2 = il => il.AddAddon(new Bridge(level, il.Node.CreateChild()).Alter(b =>
                        b.Node.Relative = Matrix.RotationY(MathHelper.Pi) * Matrix.Translation(0, 0, -6)));

                    isl.AddAddon(new Tower(level, isl.Node.CreateChild()));
                    isl.AddAddon(new FlightEngine(level, isl.Node.CreateChild()));

                    addBridge(isl);
                    addBridge2(isl);

                }
                else if (random.NextDouble() < (1 / 10f))
                {
                    isl.AddAddon(new Storage(level, isl.Node.CreateChild()));
                }
            }
        }
        public struct IslandDescriptor
        {
            public int seed;
            public List<IBuildingElement> BaseElements;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using ProceduralBuilder.Building;
using ProceduralBuilder.Rendering;
using ProceduralBuilder.RulebaseModules.RulebaseGenerators;
using ProceduralBuilder.Scattered;
using ProceduralBuilder.Shapes;
using ProceduralBuilder.Tools;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.ProcBuilder
{
    public interface IIslandGenerator
    {
        List<IBuildingElement> GetIslandBase(int seed);
        IMesh GetIslandMesh(List<IBuildingElement> islandBase, int seed);
    }

    public class CachedIslandGenerator : IIslandGenerator
    {
        private readonly IIslandGenerator decorated;

        public CachedIslandGenerator(IIslandGenerator decorated)
        {
            this.decorated = decorated;
        }

        public List<IBuildingElement> GetIslandBase(int seed)
        {
            return decorated.GetIslandBase(seed);
        }

        public IMesh GetIslandMesh(List<IBuildingElement> islandBase, int seed)
        {
            var hash = GetIslandBaseHash(islandBase) - seed;
            Trace.WriteLine(hash);

            return decorated.GetIslandMesh(islandBase, seed);
        }

        public int GetIslandBaseHash(List<IBuildingElement> elements)
        {
            var ret = 0;
            foreach (var el in elements)
            {
                if (!(el is Face)) throw new InvalidOperationException();
                var f = (Face)el;

                ret = (ret + f.WorldMatrix.GetHashCode()) % int.MaxValue / 2;
                ret = (ret + f.Size.GetHashCode()) % int.MaxValue / 2;
            }
            return ret;
        }
    }

    public class IslandGenerator : IIslandGenerator
    {
        private const string startSemId = "IslandFace";

        public List<IBuildingElement> GetIslandBase(int seed)
        {
            var islandTiler = new IslandTiler { IslandSemId = startSemId, IslandSizes = new[] { new Vector2(10, 10), new Vector2(7, 7), new Vector2(5, 10), new Vector2(10, 5) }.ToList(), MaxClusterSize = new Vector2(10, 10) };
            var startShapes = islandTiler.GetIslandTiles(seed);

            return startShapes;
        }

        public IMesh GetIslandMesh(List<IBuildingElement> islandBase, int seed)
        {
            var dummyRenderer = new DummyRenderer();
            var builder = new Builder(dummyRenderer);
            var baseGen = new BaseGenerator { RandomSeed = seed };
            baseGen.Initialize();
            baseGen.GetProperty("startSemId").SetValue(startSemId);
            var islandGen = new IslandGenerator00401 { ParentGenerator = baseGen, Builder = builder };
            islandGen.Initialize();
            baseGen.GetProperty("generator").SetValue(islandGen);

            var levelVisualizer = new BuilderNodeLevelViewer(builder);
            builder.Build(islandBase, baseGen.Generate(), seed);
            levelVisualizer.ChangeCurrentLevel(100);

            return dummyRenderer.GetBatchedMesh();
        }


    }
}

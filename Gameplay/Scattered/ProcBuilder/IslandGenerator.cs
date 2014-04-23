using System.Collections.Generic;
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
    public class IslandGenerator : IIslandGenerator
    {
        private const string startSemId = "IslandFace";

        public List<IBuildingElement> GetIslandBase(int seed)
        {
            var islandSizes = new[] { new Vector2(10, 10), new Vector2(15, 7), new Vector2(10, 15), new Vector2(10, 5) }.ToList();
            var islandTiler = new IslandTiler { IslandSemId = startSemId, IslandSizes = islandSizes, MaxClusterSize = new Vector2(20, 20) };
            var startShapes = islandTiler.GetIslandTiles(seed);

            return startShapes;
        }

        public IMesh GetIslandMesh(List<IBuildingElement> islandBase, int seed)
        {
            IMesh ret;
            List<IBuildingElement> temp01;
            List<IBuildingElement> temp02;
            List<IBuildingElement> temp03;
            GetIslandParts(islandBase, seed, true, out ret, out temp01, out temp02, out temp03);
            return ret;
        }

        public List<IBuildingElement> GetNavMesh(List<IBuildingElement> islandBase, int seed)
        {
            IMesh temp01;
            List<IBuildingElement> ret;
            List<IBuildingElement> temp02;
            List<IBuildingElement> temp03;
            GetIslandParts(islandBase, seed, false, out temp01, out ret, out temp02, out temp03);
            return ret;
        }

        public void GetIslandParts(List<IBuildingElement> islandBase, int seed, bool generateIslandMesh, out IMesh islandMesh, out List<IBuildingElement> navMesh, out List<IBuildingElement> buildMesh, out List<IBuildingElement> borderMesh)
        {
            const string grassMeshSemId = "GrassMesh";
            const string dirtMeshSemId = "DirtMesh";
            const string walkableSemId = "Walkable";
            const string buildableSemId = "Buildable";
            const string borderSemId = "Border";
            var structureBuilder = new Builder(new DummyRenderer());
            structureBuilder.SuppressWarnings = true;
            var meshDummyRenderer = new DummyRenderer();
            var meshBuilder = new Builder(meshDummyRenderer);
            meshBuilder.SuppressWarnings = true;

            var structureGen = new IslandStructureGenerator { RandomSeed = seed};
            structureGen.Initialize();
            structureGen.GetProperty("startSemId").SetValue(startSemId);
            structureGen.GetProperty("topMeshSemId").SetValue(grassMeshSemId);
            structureGen.GetProperty("dirtMeshSemId").SetValue(dirtMeshSemId);
            structureGen.GetProperty("walkableTopSemId").SetValue(walkableSemId);
            structureGen.GetProperty("borderTopSemId").SetValue(borderSemId);
            structureGen.GetProperty("buildSemId").SetValue(buildableSemId);
            structureBuilder.Build(islandBase, structureGen.Generate(), seed);
            var allStructureShapes = structureBuilder.GetTerminalShapes();

            if(generateIslandMesh)
            {
                var meshGen = new IslandMeshPlacer { RandomSeed = seed };
                meshGen.Initialize();
                meshGen.GetProperty("topMeshSemId").SetValue(grassMeshSemId);
                meshGen.GetProperty("dirtMeshSemId").SetValue(dirtMeshSemId);
                var shapesToMesh = allStructureShapes.Where(e => ((Face)e).GetSemanticId() == grassMeshSemId || ((Face)e).GetSemanticId() == dirtMeshSemId).ToList();
                meshBuilder.Build(shapesToMesh, meshGen.Generate(), seed);
            }

            islandMesh = meshDummyRenderer.GetBatchedMesh();
            navMesh = allStructureShapes.Where(e => ((Face)e).GetSemanticId() == walkableSemId).ToList();
            buildMesh = allStructureShapes.Where(e => ((Face)e).GetSemanticId() == buildableSemId).ToList();
            borderMesh = allStructureShapes.Where(e => ((Face)e).GetSemanticId() == borderSemId).ToList();
        }


    }
}

using System.Collections.Generic;
using MHGameWork.TheWizards.Rendering;
using ProceduralBuilder.Building;

namespace MHGameWork.TheWizards.Scattered.ProcBuilder
{
    public interface IIslandGenerator
    {
        List<IBuildingElement> GetIslandBase(int seed);
        IMesh GetIslandMesh(List<IBuildingElement> islandBase, int seed);
    }
}
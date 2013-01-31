using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public interface IConnectionProvider<T> where T:class 
    {
        IEnumerable<T> GetConnectedNodes(PathFinder2D<T> finder, T current);

        float GetCost(T current, T neighbor);

        float GetHeuristicCostEstimate(T start, T goal);



    }
}
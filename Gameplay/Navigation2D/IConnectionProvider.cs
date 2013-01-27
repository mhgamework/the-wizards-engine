using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public interface IConnectionProvider<T>
    {
        IEnumerable<T> GetConnectedNodes(T current);

        float GetCost(T current, T neighbor);

        float GetHeuristicCostEstimate(T start, T goal);



    }
}
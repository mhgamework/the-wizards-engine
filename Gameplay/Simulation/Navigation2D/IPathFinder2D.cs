using System.Collections.Generic;

namespace MHGameWork.TheWizards.Navigation2D
{
    public interface IPathFinder2D<T> where T : class
    {
        List<T> FindPath(T start, T goal);
    }
}
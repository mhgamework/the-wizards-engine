using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Common
{
    public interface IWorldLocator
    {
        IEnumerable<object> AtPosition(Vector3 point, float radius);
    }
    public static class WorldLocatorExtensions
    {
        public static IEnumerable<object> AtObject(this IWorldLocator l, IPhysical ph, float radius)
        {
            return l.AtPosition(ph.Physical.GetPosition(), radius);
        }
    }
}
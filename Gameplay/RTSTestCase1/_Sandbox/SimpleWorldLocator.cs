using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;

using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    public class SimpleWorldLocator : IWorldLocator
    {
        public IEnumerable<object> AtPosition(Vector3 point, float radius)
        {
            return
                TW.Data.Objects.OfType<IPhysical>()
                  .Where(p => Vector3.Distance(p.Physical.GetPosition(), point) < radius);
        }
    }
}
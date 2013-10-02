using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    public class SimpleWorldLocator : IWorldLocator
    {
        public IEnumerable<IWorldObject> AtPosition(Vector3 point, float radius)
        {
            throw new System.NotImplementedException();
            //return
            //    TW.Data.Objects.OfType<IPhysical>()
            //      .Where(p => Vector3.Distance(p.Physical.GetPosition(), point) < radius);
        }

        public IEnumerable<IWorldObject> Raycast(Ray ray)
        {
            throw new System.NotImplementedException();
        }

    }
}
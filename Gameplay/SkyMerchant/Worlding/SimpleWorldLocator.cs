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
            return from obj in TW.Data.Objects.OfType<IWorldObject>()
                   let dist = Vector3.Distance(obj.Position, point)
                   where dist < radius
                   orderby dist
                   select obj;
        }

        public IEnumerable<IWorldObject> Raycast(Ray ray)
        {
            var ret = from obj in TW.Data.Objects.OfType<IWorldObject>()
                      let localRay = ray.Transform(Matrix.Invert(obj.GetWorldMatrix()))
                      let intersect = localRay.xna().Intersects(obj.LocalBoundingBox.xna())
                      where intersect.HasValue
                      orderby intersect.Value
                      select obj;
            return ret;
        }

    }
}
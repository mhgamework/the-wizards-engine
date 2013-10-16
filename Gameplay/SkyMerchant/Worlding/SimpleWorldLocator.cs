using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    public class SimpleWorldLocator : IWorldLocator
    {
        public IEnumerable<IPositionComponent> AtPosition(Vector3 point, float radius)
        {
            return from obj in TW.Data.Objects.OfType<IPositionComponent>()
                   let dist = Vector3.Distance(obj.Position, point)
                   where dist < radius
                   orderby dist
                   select obj;
        }

        public IEnumerable<IPositionComponent> Raycast(Ray ray)
        {
            var ret = from obj in TW.Data.Objects.OfType<IPositionComponent>()
                      let localRay = ray.Transform(Matrix.Invert(obj.GetWorldMatrix()))
                      let intersect = localRay.xna().Intersects(obj.LocalBoundingBox.xna())
                      where intersect.HasValue
                      orderby intersect.Value
                      select obj;

            return ret;
        }

    }
}
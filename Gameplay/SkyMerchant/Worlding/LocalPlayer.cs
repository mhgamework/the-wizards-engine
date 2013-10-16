using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;
using MHGameWork.TheWizards.SkyMerchant._Engine;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    /// <summary>
    /// SkyMerchant implementation of the ILocalPlayer interface
    /// </summary>
    public class LocalPlayer : ILocalPlayer
    {
        private IWorldLocator worldlocator;

        public LocalPlayer(IWorldLocator worldlocator)
        {
            this.worldlocator = worldlocator;
        }

        public IEnumerable<IPositionComponent> TargetedObjects
        {
            get
            {
                var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
                return worldlocator.Raycast(ray);
            }
        }

        public Vector3? GetPointTargetedOnObject(IPositionComponent obj)
        {
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            var localRay = ray.Transform(Matrix.Invert(obj.GetWorldMatrix()));
            var dist = localRay.xna().Intersects(obj.LocalBoundingBox.xna());

            return dist.With(d => Vector3.TransformCoordinate(localRay.GetPoint(d), obj.GetWorldMatrix()));
        }
    }
}
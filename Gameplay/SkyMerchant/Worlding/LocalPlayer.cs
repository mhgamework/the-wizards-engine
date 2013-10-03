using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

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

        public IEnumerable<IWorldObject> TargetedObjects
        {
            get
            {
                var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
                return worldlocator.Raycast(ray);
            }
        }
    }
}
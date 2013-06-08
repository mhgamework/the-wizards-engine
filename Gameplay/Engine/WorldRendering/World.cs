using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.Engine.Synchronization;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Represents the World, should be singleton
    /// </summary>
    [NoSync]
    [ModelObjectChanged]
    public class World : EngineModelObject
    {
        private WorldRaycaster raycaster = new WorldRaycaster();
        public World()
        {

        }


        /// <summary>
        /// Could be seen as a getter
        /// </summary>
        /// <returns></returns>
        public RaycastResult Raycast(Ray ray)
        {
            return raycaster.Raycast(ray);
        }
        /// <summary>
        /// Could be seen as a getter
        /// </summary>
        /// <returns></returns>
        public RaycastResult Raycast(Ray ray, Func<IPhysical, bool> filter)
        {
            return raycaster.Raycast(ray,filter);
        }

    }
}


using System;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Represents the World, should be singleton
    /// </summary>
    [NoSync]
    [ModelObjectChanged]
    public class World : BaseModelObject
    {
        private WorldRaycaster raycaster = new WorldRaycaster();
        public World()
        {

        }


        /// <summary>
        /// Could be seen as a getter
        /// </summary>
        /// <returns></returns>
        public RaycastResult Raycast(Ray ray, bool raycastInvisible = false)
        {
            raycaster.RaycastInvisible = raycastInvisible;
            return raycaster.Raycast(ray);
        }

    }
}


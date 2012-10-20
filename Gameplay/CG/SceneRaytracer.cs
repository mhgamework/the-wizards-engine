using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace ComputerGraphics
{
    public class SceneRaytracer
    {
        private List<Entity> entities = new List<Entity>();

        private WorldRaycaster worldCaster = new WorldRaycaster();

        public void AddEntity(Entity ent)
        {
            
        }

        public RaycastResult Raycast (Ray ray)
        {
            var result = new RaycastResult();
            var temp = new RaycastResult();
            foreach (var ent in entities)
            {
                worldCaster.RaycastEntity(ent, ray, temp);
                if (temp.IsCloser(result)) temp.CopyTo(result);
            }
            return result;
        }
    }
}

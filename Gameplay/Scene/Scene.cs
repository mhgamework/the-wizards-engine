using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Scene
{
    /// <summary>
    /// This is NOT a data class, 
    /// this is the scene frontent for the gameplay
    /// </summary>
    public class Scene
    {
        private readonly MeshRenderer renderer;
        private readonly MeshPhysicsElementFactory physicsElementFactory;
        private List<Entity> entities = new List<Entity>();

        public Scene(MeshRenderer renderer,MeshPhysicsElementFactory physicsElementFactory)
        {
            this.renderer = renderer;
            this.physicsElementFactory = physicsElementFactory;
        }

        public void AddEntity(Entity ent)
        {
            entities.Add(ent);
            ent.UpdateRenderElement(renderer);
            ent.UpdatePhysicsElement(physicsElementFactory);
        }

        public void EnableSimulation()
        {
            
        }
        public void DisableSimulation()
        {
            
        }
    }
}

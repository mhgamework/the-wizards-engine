using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private List<Entity> entities = new List<Entity>();

        public Scene(MeshRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void AddEntity(Entity ent)
        {
            entities.Add(ent);
        }
    }
}

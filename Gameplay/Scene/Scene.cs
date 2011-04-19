using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scene
{
    /// <summary>
    /// This is NOT a data class, 
    /// this is the scene frontent for the gameplay
    /// </summary>
    public class Scene : IXNAObject
    {
        private readonly SimpleMeshRenderer renderer;
        private readonly MeshPhysicsElementFactory physicsElementFactory;
        private List<Entity> entities = new List<Entity>();
        internal List<Entity> UpdateList = new List<Entity>();

        public IXNAGame Game { get; private set; }

        public Scene(SimpleMeshRenderer renderer, MeshPhysicsElementFactory physicsElementFactory)
        {
            this.renderer = renderer;
            this.physicsElementFactory = physicsElementFactory;
        }

        internal SimpleMeshRenderer Renderer
        {
            get { return renderer; }
        }

        internal MeshPhysicsElementFactory PhysicsElementFactory
        {
            get { return physicsElementFactory; }
        }

        public void AddEntity(Entity ent)
        {
            entities.Add(ent);
            ent.UpdateRenderElement();
            ent.UpdatePhysicsElement();
        }

        public void EnableSimulation()
        {

        }
        public void DisableSimulation()
        {

        }

        public Entity RaycastScene(Ray ray)
        {
            float? closest = null;
            Entity ret = null;

            for (int i = 0; i < entities.Count; i++)
            {
                var ent = entities[i];
                var dist = ray.Intersects(ent.BoundingBox);
                if (dist.HasValue && (!closest.HasValue || dist.Value > closest.Value))
                {
                    closest = dist;
                    ret = ent;
                }

            }

            return ret;
        }

        public void Initialize(IXNAGame _game)
        {
            Game = _game;
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
            for (int i = 0; i < UpdateList.Count; i++)
            {
                var ent = UpdateList[i];
                ent.Update();
            }
        }

    }
}

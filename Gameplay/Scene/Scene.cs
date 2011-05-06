using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Ray = Microsoft.Xna.Framework.Ray;

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

        private SceneScriptLoader scriptLoader;

        public IXNAGame Game { get; private set; }

        public Scene(SimpleMeshRenderer renderer, MeshPhysicsElementFactory physicsElementFactory)
        {
            this.renderer = renderer;
            this.physicsElementFactory = physicsElementFactory;
            customRaycastReport = new CustomRaycastReport(this);
            scriptLoader = new SceneScriptLoader(this);
            
        }

        internal SimpleMeshRenderer Renderer
        {
            get { return renderer; }
        }

        internal MeshPhysicsElementFactory PhysicsElementFactory
        {
            get { return physicsElementFactory; }
        }

        public Entity CreateEntity()
        {
            var ent = new Entity(this);
            entities.Add(ent);

            return ent;
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
            _game.AddXNAObject(scriptLoader);
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

        public void AssignScriptToEntity(Entity entity, FileInfo scriptFile)
        {
            scriptLoader.LoadScript(entity, scriptFile);
        }

        private Entity resolveEntityFromPhysx(Actor actor)
        {
            if (actor.UserData is Entity) return (Entity)actor.UserData;
            return null;
        }

        private CustomRaycastReport customRaycastReport;

        public EntityRaycastHit RaycastEntityPhysX(Ray ray, RaycastFilterDelegate filter)
        {
            StillDesign.PhysX.Ray pRay = new StillDesign.PhysX.Ray(ray.Position, ray.Direction);

            customRaycastReport.LastHit = null;
            customRaycastReport.CurrentFilter = filter;

            var count = physicsElementFactory.Engine.Scene.RaycastAllShapes(pRay, customRaycastReport, ShapesType.All);

            customRaycastReport.CurrentFilter = null;

            return customRaycastReport.LastHit;

        }


        public delegate bool RaycastFilterDelegate(EntityRaycastHit hit);

        private class CustomRaycastReport : UserRaycastReport
        {
            private readonly Scene scene;
            public RaycastFilterDelegate CurrentFilter;
            public EntityRaycastHit LastHit;


            public CustomRaycastReport(Scene scene)
            {
                this.scene = scene;
            }

            public override bool OnHit(RaycastHit hits)
            {
                //WARNING: MAY CAUSE OVERHEAD (class constructor)
                var entityFromPhysx = scene.resolveEntityFromPhysx(hits.Shape.Actor);
                if (entityFromPhysx == null) return false;
                var h = new EntityRaycastHit(hits, entityFromPhysx);
                var ret = CurrentFilter(h);

                if (ret == false) return false;

                LastHit = h;

                return true;

            }
        }

        public class EntityRaycastHit
        {
            public float Distance;
            public Vector3 WorldImpact;
            public Vector3 WorldNormal;

            public Entity Entity;

            public EntityRaycastHit(RaycastHit hit, Entity entity)
            {
                Entity = entity;
                Distance = hit.Distance;
                WorldImpact = hit.WorldImpact;
                WorldNormal = hit.WorldNormal;


            }
        }
    }
}

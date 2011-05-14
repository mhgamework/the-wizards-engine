using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public Input Input { get; private set; }

        private SceneScriptLoader scriptLoader;

        public IXNAGame Game { get; private set; }

        public Scene(SimpleMeshRenderer renderer, MeshPhysicsElementFactory physicsElementFactory)
        {
            this.renderer = renderer;
            this.physicsElementFactory = physicsElementFactory;
            customRaycastReport = new CustomRaycastReport(this);
            scriptLoader = new SceneScriptLoader(this);
            sceneComponents = new List<object>();

            registerPhysXContactNotification(physicsElementFactory);


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
            Input = new Input(Game);
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
            processPhysXContacts();
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

        public EntityRaycastHit RaycastEntityPhysX(Ray ray, Predicate<EntityRaycastHit> filter)
        {
            Game.LineManager3D.AddRay(ray, Color.Orange);
            StillDesign.PhysX.Ray pRay = new StillDesign.PhysX.Ray(ray.Position, ray.Direction);

            customRaycastReport.LastHit = null;
            customRaycastReport.CurrentFilter = filter;

            var count = physicsElementFactory.Engine.Scene.RaycastAllShapes(pRay, customRaycastReport, ShapesType.All);

            customRaycastReport.CurrentFilter = null;

            return customRaycastReport.LastHit;

        }

        private List<PhysXContact> contacts = new List<PhysXContact>();

        private void registerPhysXContactNotification(MeshPhysicsElementFactory physicsElementFactory)
        {
            physicsElementFactory.Engine.AddContactNotification(onPhysXContact);
        }

        private void onPhysXContact(ContactPair contactinformation, ContactPairFlag events)
        {
            contacts.Add(new PhysXContact(contactinformation, events));

        }

        private void processPhysXContacts()
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                var contact = contacts[i];

                var entA = resolveEntityFromPhysx(contact.Contactinformation.ActorA);
                var entB = resolveEntityFromPhysx(contact.Contactinformation.ActorB);
                if (entA == null || entB == null) continue;
                if (entA.ContactHandler != null)
                    ExecuteInScriptScope(entA.ContactEntityHandle,
                                         () => entA.ContactHandler(contact.ToContactInformationHelper(entB.APIEntity)));

                if (entB.ContactHandler != null)
                    ExecuteInScriptScope(entB.ContactEntityHandle,
                                         () => entB.ContactHandler(contact.ToContactInformationHelper(entA.APIEntity)));

            }
            contacts.Clear();
        }

        private struct PhysXContact
        {
            public ContactPair Contactinformation { get; set; }
            public ContactPairFlag Events { get; set; }

            public PhysXContact(ContactPair contactinformation, ContactPairFlag events)
                : this()
            {
                Contactinformation = contactinformation;
                Events = events;
            }

            public ContactInformation ToContactInformationHelper(IEntity otherEntity)
            {
                var info = new ContactInformation
                               {
                                   Flags = Events,
                                   FrictionForce = Contactinformation.FrictionForce,
                                   NormalForce = Contactinformation.NormalForce,
                                   OtherEntity = otherEntity
                               };

                return info;
            }

        }
        private List<object> sceneComponents;


        public EntityScriptHandle CurrentRunningScriptHandle { get; private set; }


        /// <summary>
        /// This function ensures full seperation of the scripts from the rest of the engine
        /// 
        /// All code executed in scripts should run inside this function. This function ensures all the script scope variable are set.
        /// It should catch exceptions.
        /// </summary>
        public void ExecuteInScriptScope(EntityScriptHandle handle, Action func)
        {

            CurrentRunningScriptHandle = handle;

            var catchall = false;

            if (catchall)
            {
                try
                {
                    func();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                func();
            }

            CurrentRunningScriptHandle = null;
        }

        /// <summary>
        /// Scene components are objects that share more advanced engine functionality to scripts.
        /// These are to be used cautiously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSceneComponent<T>() where T : class
        {
            for (int index = 0; index < sceneComponents.Count; index++)
            {
                var o = sceneComponents[index];
                if (o is T) return (T)o;
            }
            return null;
        }

        public void AddSceneComponent<T>(T component) where T : class
        {
            if (GetSceneComponent<T>() != null) throw new InvalidOperationException("Component already added!");
            sceneComponents.Add(component);
        }

        private class CustomRaycastReport : UserRaycastReport
        {
            private readonly Scene scene;
            public Predicate<EntityRaycastHit> CurrentFilter;
            public EntityRaycastHit LastHit;


            public CustomRaycastReport(Scene scene)
            {
                this.scene = scene;
            }

            public override bool OnHit(RaycastHit hits)
            {
                //WARNING: MAY CAUSE OVERHEAD (class constructor)
                var entityFromPhysx = scene.resolveEntityFromPhysx(hits.Shape.Actor);
                if (entityFromPhysx == null) return true;
                var h = new EntityRaycastHit(hits, entityFromPhysx);
                var ret = CurrentFilter(h);

                if (ret == false) return true;

                LastHit = h;

                return false;

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

            public Scripting.API.EntityRaycastHit ToAPIRaycastHit()
            {
                return new Scripting.API.EntityRaycastHit
                {
                    Distance = Distance,
                    Entity = Entity.APIEntity,
                    WorldImpact = WorldImpact,
                    WorldNormal = WorldNormal,
                    IsHit = true
                };
            }
        }

        public ISceneMeshProvider MeshProvider { get; set; }

        public IMesh GetMesh(string path)
        {
            if (MeshProvider == null) throw new InvalidOperationException("No MeshProvider set on the scene!");
            return MeshProvider.GetMesh(path);
        }
    }
}

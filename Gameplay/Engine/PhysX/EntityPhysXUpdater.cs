using System.Collections.Generic;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using StillDesign.PhysX;
using IDisposable = System.IDisposable;

namespace MHGameWork.TheWizards.Engine.PhysX
{
    /// <summary>
    /// Applies changes to all changed Entity's PhysX properties, and pulls changes from the physX back into dynamic objects
    /// </summary>
    public class EntityPhysXUpdater
    {
        private MeshPhysicsElementFactory factory;
        private ClientPhysicsQuadTreeNode root;

        private List<EntityPhysX> dynamicEntities = new List<EntityPhysX>();

        public EntityPhysXUpdater(MeshPhysicsElementFactory factory, ClientPhysicsQuadTreeNode root)
        {
            this.factory = factory;
            this.root = root;
        }

        public void Update()
        {


            foreach (var change in TW.Data.GetChangesOfType<WorldRendering.Entity>())
            {
                var ent = change.ModelObject as WorldRendering.Entity;
                var data = ent.get<EntityPhysX>();
                if (change.Change == Data.ModelChange.Removed)
                {
                    if (data != null)
                        data.Dispose();
                    continue;
                }

                if (change.Change == Data.ModelChange.Added)
                {

                }

                if (!ent.Solid)
                {
                    //disable physx
                    if (data != null) data.Dispose();
                    ent.set<EntityPhysX>(data);
                    continue;
                }



                if (data == null)
                {
                    data = new EntityPhysX(ent, factory);
                    ent.set(data);
                }
                data.OnEntityChanged(); //TODO: should in fact only be called on worldmatrix changes!!

                if (ent.Static == false)
                {
                    if (!dynamicEntities.Contains(data))
                        dynamicEntities.Add(data);
                }
                else
                {
                    dynamicEntities.Remove(data);
                }
            }

            foreach (var dyn in dynamicEntities)
            {
                dyn.FetchUpdatesFromPhysX();
            }
        }

        private void deletePhysicsElement(MeshPhysicsElementFactory factory, WorldRendering.Entity ent)
        {
            var el = ent.get<MeshStaticPhysicsElement>();
            if (el != null)
            {
                factory.DeleteStaticElement(el);
            }
            ent.set<MeshStaticPhysicsElement>(null);
        }


        public class EntityPhysX : IDisposable
        {


            private MeshStaticPhysicsElement staticPhysicsElement;
            private MeshDynamicPhysicsElement dynamicPhysicsElement;
            private MeshPhysicsElementFactory factory;

            public WorldRendering.Entity Entity { get; private set; }

            public bool Solid { get { return Entity.Solid; } }
            public bool Kinematic { get { return Entity.Kinematic; } }
            public bool Static { get { return Entity.Static; } }
            public IMesh Mesh { get { return Entity.Mesh; } }

            public EntityPhysX(WorldRendering.Entity ent, MeshPhysicsElementFactory factory)
            {
                Entity = ent;
                this.factory = factory;
            }


            private void UpdatePhysicsElement()
            {
                if (!Solid)
                {
                    deletePhysicsElement();
                    return;
                }
                if (Static)
                {
                    if (dynamicPhysicsElement != null)
                        deletePhysicsElement();
                    else if (staticPhysicsElement != null && staticPhysicsElement.Mesh != Mesh)
                        deletePhysicsElement();
                }
                else
                {
                    if (staticPhysicsElement != null)
                        deletePhysicsElement();
                    else if (dynamicPhysicsElement != null && dynamicPhysicsElement.Mesh != Mesh)
                        deletePhysicsElement();
                }

                if (Mesh == null) return;

                if (Static)
                {
                    if (staticPhysicsElement == null)
                        staticPhysicsElement = factory.CreateStaticElement(Mesh, Entity.WorldMatrix.xna());
                    staticPhysicsElement.ActorUserData = this;

                }
                else
                {
                    if (dynamicPhysicsElement == null)
                        dynamicPhysicsElement = factory.CreateDynamicElement(Mesh, Entity.WorldMatrix.xna());
                    dynamicPhysicsElement.ActorUserData = this;
                }


            }

            public Actor getCurrentActor()
            {
                if (staticPhysicsElement != null && staticPhysicsElement.Actor != null)
                    return staticPhysicsElement.Actor;
                if (dynamicPhysicsElement != null && dynamicPhysicsElement.Actor != null)
                    return dynamicPhysicsElement.Actor;
                return null;

            }

            private void deletePhysicsElement()
            {

                if (staticPhysicsElement != null)
                {
                    factory.DeleteStaticElement(staticPhysicsElement);
                    staticPhysicsElement = null;
                }
                if (dynamicPhysicsElement != null)
                {
                    factory.DeleteDynamicElement(dynamicPhysicsElement);
                    dynamicPhysicsElement = null;
                }
            }


            /// <summary>
            /// Applies simulation info to this object
            /// </summary>
            public void FetchUpdatesFromPhysX()
            {
                if (Mesh == null) return;
                if (Static) return;
                if (dynamicPhysicsElement != null)
                    Entity.WorldMatrix = dynamicPhysicsElement.Actor.GlobalPose.dx();
            }

            /// <summary>
            /// Call when entity has changed
            /// </summary>
            public void OnEntityChanged()
            {
                UpdatePhysicsElement();

                if (staticPhysicsElement != null)
                {
                    staticPhysicsElement.WorldMatrix = Entity.WorldMatrix.xna();

                }
                if (dynamicPhysicsElement != null)
                {
                    dynamicPhysicsElement.Kinematic = Kinematic;
                    dynamicPhysicsElement.World = Entity.WorldMatrix.xna();
                }

            }

            public void Dispose()
            {
                if (dynamicPhysicsElement != null)
                    factory.DeleteDynamicElement(dynamicPhysicsElement);
                if (staticPhysicsElement != null)
                    factory.DeleteStaticElement(staticPhysicsElement);

                dynamicPhysicsElement = null;


            }
        }



    }
}

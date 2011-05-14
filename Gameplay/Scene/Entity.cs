using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting.API;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Ray = Microsoft.Xna.Framework.Ray;

namespace MHGameWork.TheWizards.Scene
{

    /// <summary>
    /// This is NOT a data class
    /// This represents a GamePlay-level entity.
    /// </summary>
    public class Entity
    {

        public Scene Scene { get; private set; }

        private Transformation transformation;
        public Transformation Transformation
        {
            get { return transformation; }
            set
            {
                transformation = value;
                onChange();
                updateBoundingBox();
            }
        }

        private bool visible;
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible == value) return;
                visible = value;
                onChange();
            }
        }

        private bool solid;
        public bool Solid
        {
            get { return solid; }
            set
            {
                if (solid == value) return;
                solid = value;
                onChange();
            }
        }

        private bool _static;
        public bool Static
        {
            get { return _static; }
            set
            {
                if (_static == value) return;
                _static = value;
                onChange();
            }
        }

        private bool kinematic;
        public bool Kinematic
        {
            get { return kinematic; }
            set
            {
                if (kinematic == value) return;
                kinematic = value;
                onChange();
            }
        }

        private IMesh mesh;
        public IMesh Mesh
        {
            get { return mesh; }
            set
            {
                if (mesh == value) return;
                mesh = value;
                calculateLocalBoundingBox();
                onChange();
            }
        }

        public EntityData Data { get; private set; }

        public BoundingBox BoundingBox { get; private set; }
        public BoundingBox LocalBoundingBox { get; private set; }

        public APIEntity APIEntity { get; private set; }

        public float? Raycast(Ray ray)
        {
            throw new NotImplementedException();
        }

        public Entity(Scene scene)
        {
            Scene = scene;
            _static = true;
            solid = true;
            transformation = Transformation.Identity;

            Data = new EntityData();

            APIEntity = new APIEntity(this);

        }


        private SimpleMeshRenderElement renderElement;
        private MeshStaticPhysicsElement staticPhysicsElement;
        private MeshDynamicPhysicsElement dynamicPhysicsElement;

        internal void UpdateRenderElement()
        {
            var renderer = Scene.Renderer;

            if (renderElement != null && renderElement.Mesh != Mesh)
                deleteRenderElement();

            if (mesh == null) return;

            if (renderElement == null)
                renderElement = renderer.AddMesh(Mesh);


            renderElement.WorldMatrix = Transformation.CreateMatrix();
        }

        private void deleteRenderElement()
        {
            if (renderElement == null) return;
            renderElement.Delete();
            renderElement = null;
        }

        internal void UpdatePhysicsElement()
        {
            var factory = Scene.PhysicsElementFactory;
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
                    staticPhysicsElement = factory.CreateStaticElement(Mesh, Transformation.CreateMatrix());
                staticPhysicsElement.ActorUserData = this;

                staticPhysicsElement.ActorCreated += physicsElement_ActorCreated;
            }
            else
            {
                if (dynamicPhysicsElement == null)
                    dynamicPhysicsElement = factory.CreateDynamicElement(Mesh, Transformation.CreateMatrix());
                dynamicPhysicsElement.ActorUserData = this;
                dynamicPhysicsElement.ActorCreated += physicsElement_ActorCreated;
            }


        }

        private Actor getCurrentActor()
        {
            if (staticPhysicsElement != null && staticPhysicsElement.Actor != null)
                return staticPhysicsElement.Actor;
            if (dynamicPhysicsElement != null && dynamicPhysicsElement.Actor != null)
                return dynamicPhysicsElement.Actor;
            return null;

        }

        void physicsElement_ActorCreated(Actor actor)
        {
            updateActorContactFlags(actor);
        }

        private void updateActorContactFlags(Actor actor)
        {
            if (actor == null) return;
            if (ContactHandler != null)
                actor.ContactReportFlags = ContactPairFlag.All;
            else
                actor.ContactReportFlags = ContactPairFlag.IgnorePair;
        }

        private void deletePhysicsElement()
        {
            var factory = Scene.PhysicsElementFactory;

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



        private void calculateLocalBoundingBox()
        {
            if (mesh == null)
            {
                BoundingBox = new BoundingBox();
                return;
            }
            var data = mesh.GetCoreData();

            var bb = new BoundingBox();


            for (int i = 0; i < data.Parts.Count; i++)
            {
                var part = data.Parts[i];
                var geom = part.MeshPart.GetGeometryData();

                bb = bb.MergeWith(BoundingBox.CreateFromPoints(geom.GetSourceVector3(MeshPartGeometryData.Semantic.Position)));
            }
            LocalBoundingBox = bb;
            updateBoundingBox();

        }

        private void updateBoundingBox()
        {
            BoundingBox = LocalBoundingBox.Transform(Transformation.CreateMatrix());
        }


        public void Update()
        {
            if (mesh != null)
            {
                if (!Static)
                {
                    var t = Transformation;
                    t.Translation = dynamicPhysicsElement.Actor.GlobalPosition;
                    t.Rotation = dynamicPhysicsElement.Actor.GlobalOrientationQuat;
                    transformation = t;
                    updateBoundingBox();
                    if (renderElement != null)
                        renderElement.WorldMatrix = t.CreateMatrix();
                }  
            }
           
            for (int i = 0; i < entityHandles.Count; i++)
            {
                var h = entityHandles[i];
                if (!h.UpdateRegistered) continue;
                Scene.ExecuteInScriptScope(h, ((IUpdateHandler)h.Script).Update);
            }

        }

        /// <summary>
        /// If the changes are not cause by external calls, this function is not called
        /// </summary>
        private void onChange()
        {
            UpdateRenderElement();
            UpdatePhysicsElement();

            if (staticPhysicsElement != null)
            {
                staticPhysicsElement.WorldMatrix = transformation.CreateMatrix();
                
            }
            if (dynamicPhysicsElement != null)
            {
                dynamicPhysicsElement.Kinematic = Kinematic;
                dynamicPhysicsElement.World = Transformation.CreateMatrix();
            }
            initContactHandler();
            var enableUpdate = false;
            enableUpdate |= !Static;
            for (int i = 0; i < entityHandles.Count; i++)
            {
                if (entityHandles[i].UpdateRegistered)
                    enableUpdate = true;
            }

            if (enableUpdate)
            {
                if (!Scene.UpdateList.Contains(this))
                    Scene.UpdateList.Add(this);
            }
            else
            {
                Scene.UpdateList.Remove(this);
            }
        }

        private void initContactHandler()
        {
            if (ContactHandler == null) return;
            updateActorContactFlags(getCurrentActor());

        }




        // Scripting functions


        private List<EntityScriptHandle> entityHandles = new List<EntityScriptHandle>();


        public EntityScriptHandle CreateEntityHandle(IScript script)
        {
            var ret = new EntityScriptHandle(this, script);

            entityHandles.Add(ret);
            onChange();

            return ret;
        }

        public void DestroyEntityHandle(EntityScriptHandle handle)
        {
            handle.Destroy();

            entityHandles.Remove(handle);
            onChange();

        }

        internal void OnEntityHandlerStateChanged()
        {
            onChange();
        }


        public EntityScriptHandle GetAttachedScriptHandle<T>() where T : IScript
        {
            for (int i = 0; i < entityHandles.Count; i++)
            {
                var h = entityHandles[i];
                if (h.Script is T)
                    return h;
            }
            return null;
        }

        public Action<IPlayer> PlayerUseHandler;
        public Action<ContactInformation> ContactHandler { get; private set; }
        public EntityScriptHandle ContactEntityHandle { get; private set; }
        public void SetContactHandler(EntityScriptHandle handle, Action<ContactInformation> handler)
        {
            ContactHandler = handler;
            ContactEntityHandle = handle;
            initContactHandler();
        }


        public void RaisePlayerUse(IPlayer player)
        {
            //TODO: execute in script scope
            if (PlayerUseHandler != null)
                PlayerUseHandler(player);
        }

    }
}

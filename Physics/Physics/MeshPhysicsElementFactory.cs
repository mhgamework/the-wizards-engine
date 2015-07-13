using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public class MeshPhysicsElementFactory 
    {
        public PhysicsEngine Engine { get; private set; }
        public ClientPhysicsQuadTreeNode Root { get; private set; }


        private List<MeshStaticPhysicsElement> unitializedElementsStatic = new List<MeshStaticPhysicsElement>();
        private List<MeshDynamicPhysicsElement> unitializedElementsDynamic = new List<MeshDynamicPhysicsElement>();
        private List<MeshDynamicPhysicsElement> dynamicElements = new List<MeshDynamicPhysicsElement>();

        private MeshPhysicsActorBuilder actorBuilder;

        public MeshPhysicsPool MeshPhysicsPool { get; private set; }

        public MeshPhysicsElementFactory(PhysicsEngine engine, ClientPhysicsQuadTreeNode root)
        {
            Engine = engine;
            Root = root;
            
            MeshPhysicsPool = new MeshPhysicsPool();
            actorBuilder = new MeshPhysicsActorBuilder(MeshPhysicsPool);
        }

        public MeshStaticPhysicsElement CreateStaticElement(IMesh mesh, Matrix world)
        {
            if (mesh == null) throw new ArgumentNullException("mesh");

            var el = new MeshStaticPhysicsElement(mesh, world, actorBuilder);
            /*if (game == null)
            {
                unitializedElementsStatic.Add(el);
            }
            else
            {*/
                initStaticMesh(el);
            //}
            return el;
        }

        public MeshDynamicPhysicsElement CreateDynamicElement(IMesh mesh, Matrix world)
        {
            if (mesh == null) throw new ArgumentNullException("mesh");
            var el = new MeshDynamicPhysicsElement(mesh, world, actorBuilder);
            /*if (game == null)
            {
                unitializedElementsDynamic.Add(el);
            }
            else
            {*/
                initDynamicMesh(el);
            //}

            return el;
        }

        public void DeleteStaticElement(MeshStaticPhysicsElement el)
        {
            unitializedElementsStatic.Remove(el);
            el.disposeInternal();

        }
        public void DeleteDynamicElement(MeshDynamicPhysicsElement el)
        {
            unitializedElementsDynamic.Remove(el);
            dynamicElements.Remove(el);
            el.disposeInternal();

        }


        public void Initialize()
        {
            for (int i = 0; i < unitializedElementsStatic.Count; i++)
            {
                var el = unitializedElementsStatic[i];
                initStaticMesh(el);
            }
            for (int i = 0; i < unitializedElementsDynamic.Count; i++)
            {
                var el = unitializedElementsDynamic[i];
                initDynamicMesh(el);
            }
            unitializedElementsStatic.Clear();
            unitializedElementsDynamic.Clear();
        }

        private void initStaticMesh(MeshStaticPhysicsElement el)
        {
            el.LoadInClientPhysics(Engine.Scene, Root);
        }

        private void initDynamicMesh(MeshDynamicPhysicsElement el)
        {
            el.InitDynamic(Engine.Scene);
            el.Move(Root, el.World);

            dynamicElements.Add(el);
        }


        public void Update()
        {
            MeshPhysicsPool.Update(Engine.Scene);
            for (int i = 0; i < dynamicElements.Count; i++)
            {
                var el = dynamicElements[i];
                el.Update(Root);
            }
        }
    }
}

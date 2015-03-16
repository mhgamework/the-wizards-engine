using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.LogicRTS.Framework;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.LogicRTS.EngineGameObjects
{
    public class StaticMeshComponent : BaseGameComponent
    {
        private readonly SceneGraphComponent sceneGraph;
        private Entity ent;

        public StaticMeshComponent(SceneGraphComponent sceneGraph)
        {
            this.sceneGraph = sceneGraph;
            ent = new Entity();

            sceneGraph.ObservableWorldTransform.Subscribe(_ => updateEntityMatrix());
        }

        private Matrix updateEntityMatrix()
        {
            return ent.WorldMatrix = MeshTransform * sceneGraph.WorldTransform;
        }

        public IMesh Mesh
        {
            get { return ent.Mesh; }
            set { ent.Mesh = value; }
        }

        public Matrix MeshTransform
        {
            get { return ent.WorldMatrix; }
            set
            {
                ent.WorldMatrix = value;
                updateEntityMatrix();
            }
        }

        public bool Visible
        {
            get { return ent.Visible; }
            set { ent.Visible = value; }
        }

        public bool CastsShadows
        {
            get { return ent.CastsShadows; }
            set { ent.CastsShadows = value; }
        }
    }
}
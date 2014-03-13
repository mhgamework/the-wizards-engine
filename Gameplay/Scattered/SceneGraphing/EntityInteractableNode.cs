using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using DirectX11;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    /// <summary>
    /// Currently this interacts with the boundingbox around the entity, not the exact entity.
    /// </summary>
    public class EntityInteractableNode
    {
        private readonly Entity ent;
        public SceneGraphNode Node { get; private set; }

        public EntityInteractableNode(Entity ent, SceneGraphNode node)
        {
            this.ent = ent;
            Node = node;
        }

        public float? Intersects(Ray ray)
        {
            if (ent.Visible == false) return null;
            if (ent.Mesh == null) return null;

            var newRay = ray.Transform(Matrix.Invert(Node.Absolute));

            //TODO: WARNING: NO SCALING SUPPORT!!!
            if (Math.Abs(Node.Absolute.M44 - 1) > 0.001) throw new InvalidOperationException("Scaling matrix detected!");

            return newRay.xna().Intersects(TW.Assets.GetBoundingBox(ent.Mesh).xna());
        }
    }
}
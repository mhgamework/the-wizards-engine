using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using DirectX11;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Objects
{
    public class Bridge : IIslandAddon
    {
        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {
            
        }

        public Bridge(Level level, SceneGraphNode node)
        {
            Node = node;
            node.AssociatedObject = this;
            var ent = level.CreateEntityNode(node.CreateChild());
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\BridgeConnector");
        }

        public Island Island { get { return (Island)Node.Parent.AssociatedObject; } }

        //public Vector3 RelativePosition;
        //public Vector3 Direction;
        public Bridge Connection;

        public Vector3 GetAbsolutePosition()
        {
            return Node.Absolute.GetTranslation();
        }

        public Vector3 GetAbsoluteDirection()
        {
            return Node.Absolute.xna().Forward.dx();
        }
    }
}
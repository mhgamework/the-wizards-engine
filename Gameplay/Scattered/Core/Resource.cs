using System.Linq;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Resource : IIslandAddon
    {
        private readonly Level level;
        public ItemType Type { get; private set; }
        public int Amount { get; set; }
        private TextPanelNode panelNode;
        private EntityNode entityNode;

        public Resource(Level level, SceneGraphNode node, ItemType type)
        {
            this.level = level;
            this.Type = type;
            Node = node;
            node.AssociatedObject = this;


            var renderNode = node.CreateChild();

            entityNode = level.CreateEntityNode(renderNode.CreateChild())
                 .Alter(k => k.Node.Relative = Matrix.Identity)
                 .Alter(k => k.CreateInteractable(onInteract));

            panelNode = level.CreateTextPanelNode(renderNode.CreateChild());
            panelNode.Node.Relative = Matrix.Translation(0, 2.5f, 0);
            panelNode.TextRectangle.IsBillboard = true;
            panelNode.TextRectangle.Radius = 0.35f;

            Amount = 1;
        }

        private void onInteract()
        {
            level.LocalPlayer.TryPickupResource(this);
        }

        public SceneGraphNode Node { get; private set; }


        public void PrepareForRendering()
        {
            if (Amount == 0) return;

            var textViewDist = 15;
            panelNode.TextRectangle.Entity.Visible = Vector3.Distance(Node.Position, level.LocalPlayer.Position) < textViewDist;

            panelNode.TextRectangle.Text = Amount.ToString();
            if (Amount < 10)
                entityNode.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\items\\ItemCrates1");
            else if (Amount < 20)
                entityNode.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\items\\ItemCrates2");
            else if (Amount < 50)
                entityNode.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\items\\ItemCrates3");
            else
                entityNode.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\items\\ItemCrates4");
        }

        public void AttemptMerge()
        {
            if (Amount == 0) return;
            foreach (var r in level.Islands.SelectMany(i => i.Addons.OfType<Resource>()).ToArray())
            {
                if (r.Amount == 0) continue;
                if (r == this) continue;
                if (r.Type != Type) continue;
                var mergeDist = 5;
                if (Vector3.Distance(r.Node.Position, Node.Position) > mergeDist) continue;
                Amount += r.Amount;
                r.Amount = 0;
                level.DestroyNode(r.Node);
            }
        }
    }
}
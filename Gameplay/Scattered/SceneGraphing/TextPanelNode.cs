using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    public class TextPanelNode
    {
        private readonly Level level;
        public SceneGraphNode Node { get; private set; }
        public TextRectangle TextRectangle { get; private set; }

        public TextPanelNode(Level level, SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            TextRectangle = new TextRectangle();
            TextRectangle.IsBillboard = true;

        }

        public void UpdateForRendering()
        {
            TextRectangle.Position = Node.Position;
            TextRectangle.Normal = Node.Forward;
        }

       
        public void Dispose()
        {
            TextRectangle.Dispose();
            TextRectangle = null;
        }
    }
}
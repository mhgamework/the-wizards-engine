using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.help;
using TreeGenerator.Editor;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator
{
    public class TreeLineRenderer
    {
        IXNAGame game;
        Seeder seeder;
        public int HighlightLevelIndex = -1;

        public TreeLineRenderer(IXNAGame _game)
        {
            game = _game;
        }

        public void RenderTree(TreeStructure tree,Vector3 position)
        {
            seeder = new Seeder( 148 );
            RenderTreeSegment(tree.Base, position);
        }
        private void RenderTreeSegment(TreeStructureSegment Segment, Vector3 parentPosition)
        {
            
            Vector3 position = parentPosition + Segment.Direction.Heading * Segment.Length;
            game.LineManager3D.AddLine(parentPosition, position,seeder.NextColor());
            for (int i = 0; i < Segment.Children.Count; i++)
            {
                RenderTreeSegment(Segment.Children[i], position);
            }

        }
        //public void RenderTree(engine renderdata)
        //{
        //    for (int i = 0; i < renderdata.Vertices.Count; i += 3)
        //    {
        //        game.LineManager3D.AddTriangle(
        //            renderdata.Vertices[i + 0].pos,
        //            renderdata.Vertices[i + 1].pos,
        //            renderdata.Vertices[i + 2].pos,
        //            Color.Black);
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class QuadTreeVisualizer
    {

        private readonly Color4[] levelColor4s;

        public QuadTreeVisualizer()
        {

            levelColor4s = new Color4[8];
            levelColor4s[0] = new Color4(1, 0, 0);
            levelColor4s[1] = new Color4(1, 165 / 255f, 0);
            levelColor4s[2] = new Color4(1, 1, 0);
            levelColor4s[3] = new Color4(0.5f, 0, 0.5f);
            levelColor4s[4] = new Color4(144 / 255f, 238 / 255f, 144 / 255f);
            levelColor4s[5] = new Color4(0, 0.5f, 0);
            levelColor4s[6] = new Color4(165 / 255f, 42 / 255f, 42 / 255f);
            levelColor4s[7] = new Color4(184 / 255f, 134 / 255f, 11 / 255f);

        }


        private void RenderBoundingBox(DX11Game game, BoundingBox box, Color4 col)
        {
            Vector3 radius = box.Maximum - box.Minimum;
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = box.Minimum;



            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;
            Vector3 tll = min + radY;
            Vector3 tlr = min + radY + radX;
            Vector3 tul = min + radY + radZ;
            Vector3 tur = min + radY + radX + radZ; //= max



            //grondvlak
            game.LineManager3D.AddLine(fll, flr, col);
            game.LineManager3D.AddLine(flr, fur, col);
            game.LineManager3D.AddLine(fur, ful, col);
            game.LineManager3D.AddLine(ful, fll, col);

            //opstaande ribben
            game.LineManager3D.AddLine(fll, tll, col);
            game.LineManager3D.AddLine(flr, tlr, col);
            game.LineManager3D.AddLine(fur, tur, col);
            game.LineManager3D.AddLine(ful, tul, col);

            //bovenvlak
            game.LineManager3D.AddLine(tll, tlr, col);
            game.LineManager3D.AddLine(tlr, tur, col);
            game.LineManager3D.AddLine(tur, tul, col);
            game.LineManager3D.AddLine(tul, tll, col);


            //diagonalen
            game.LineManager3D.AddLine(tll, flr, col);
            game.LineManager3D.AddLine(fll, tlr, col);

            game.LineManager3D.AddLine(tlr, fur, col);
            game.LineManager3D.AddLine(flr, tur, col);

            game.LineManager3D.AddLine(tur, ful, col);
            game.LineManager3D.AddLine(fur, tul, col);

            game.LineManager3D.AddLine(tul, fll, col);
            game.LineManager3D.AddLine(ful, tll, col);


        }


        public void RenderNodeBoundingBox<T>(DX11Game game, IQuadTreeNode<T> quadTreeNode) where T : IQuadTreeNode<T>
        {
            if (quadTreeNode == null) return;

            QuadTreeNodeData<T> node = quadTreeNode.NodeData;

            RenderNodeBoundingBox(game, node.UpperLeft);
            RenderNodeBoundingBox(game, node.UpperRight);
            RenderNodeBoundingBox(game, node.LowerLeft);
            RenderNodeBoundingBox(game, node.LowerRight);

            //TODO: Calculate level is quite lame here and slow, since we loop the tree level by level
            int level = QuadTree.CalculateLevel(quadTreeNode);
            Color4 col;
            if (level < levelColor4s.Length)
                col = levelColor4s[level];
            else
                col = levelColor4s[levelColor4s.Length - 1];

            game.LineManager3D.AddBox(quadTreeNode.NodeData.BoundingBox, col);
        }

        public void RenderNodeGroundBoundig<T>(DX11Game game, IQuadTreeNode<T> quadTreeNode) where T : IQuadTreeNode<T>
        {
            if (quadTreeNode == null) return;

            QuadTreeNodeData<T> node = quadTreeNode.NodeData;

            RenderNodeGroundBoundig(game, node.UpperLeft);
            RenderNodeGroundBoundig(game, node.UpperRight);
            RenderNodeGroundBoundig(game, node.LowerLeft);
            RenderNodeGroundBoundig(game, node.LowerRight);

            //if ( node.IsLeaf == false ) return;
            //FloorLowerLeft = min
            //TopUpperRight = max

            //TODO: Calculate level is quite lame here and slow, since we loop the tree level by level
            int level = QuadTree.CalculateLevel(quadTreeNode);
            Color4 col;
            if (level < levelColor4s.Length)
                col = levelColor4s[level];
            else
                col = levelColor4s[levelColor4s.Length - 1];

            Vector3 radius = (node.BoundingBox.Maximum - node.BoundingBox.Minimum);
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = node.BoundingBox.Minimum;
            min.Y = -1 + level;


            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;



            //grondvlak
            game.LineManager3D.AddLine(fll, flr, col);
            game.LineManager3D.AddLine(flr, fur, col);
            game.LineManager3D.AddLine(fur, ful, col);
            game.LineManager3D.AddLine(ful, fll, col);


        }

        /// <summary>
        /// This function should return true when the given node should be rendered in given Color4, otherwise false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quadTreeNode"></param>
        /// <param name="Color4"></param>
        /// <returns></returns>
        public delegate bool RenderNodePredicate<T>(T quadTreeNode, out Color4 Color4) where T : class, IQuadTreeNode<T>;

        /// <summary>
        /// This functions loops through all nodes and renders the node when the given predicate returns true for given node. It uses the out Color4 argument to determine the Color4 of the rendered node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="game"></param>
        /// <param name="quadTreeNode"></param>
        /// <param name="predicate"></param>
        public void RenderNodeGroundBoundig<T>(DX11Game game, T quadTreeNode, RenderNodePredicate<T> predicate) where T : class, IQuadTreeNode<T>
        {
            if (quadTreeNode == null) return;

            QuadTreeNodeData<T> node = quadTreeNode.NodeData;

            RenderNodeGroundBoundig(game, node.UpperLeft, predicate);
            RenderNodeGroundBoundig(game, node.UpperRight, predicate);
            RenderNodeGroundBoundig(game, node.LowerLeft, predicate);
            RenderNodeGroundBoundig(game, node.LowerRight, predicate);


            Color4 col;

            if (!predicate(quadTreeNode, out col)) return;

            //TODO: Calculate level is quite lame here and slow, since we loop the tree level by level
            int level = QuadTree.CalculateLevel(quadTreeNode);


            Vector3 radius = (node.BoundingBox.Maximum - node.BoundingBox.Minimum);
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = node.BoundingBox.Minimum;
            min.Y = -1 + level;


            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;



            //grondvlak
            game.LineManager3D.AddLine(fll, flr, col);
            game.LineManager3D.AddLine(flr, fur, col);
            game.LineManager3D.AddLine(fur, ful, col);
            game.LineManager3D.AddLine(ful, fll, col);


        }
    }
}

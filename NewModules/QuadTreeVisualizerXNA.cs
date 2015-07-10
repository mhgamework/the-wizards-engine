using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards
{
    public class QuadTreeVisualizerXNA
    {

        private readonly Color[] levelColors;

        public QuadTreeVisualizerXNA()
        {

            levelColors = new Color[8];
            levelColors[0] = Color.Red;
            levelColors[1] = Color.Orange;
            levelColors[2] = Color.Yellow;
            levelColors[3] = Color.Purple;
            levelColors[4] = Color.LightGreen;
            levelColors[5] = Color.Green;
            levelColors[6] = Color.Brown;
            levelColors[7] = Color.DarkGoldenrod;

        }


        private void RenderBoundingBox(IXNAGame game, BoundingBox box, Color col)
        {
            Vector3 radius = box.Max - box.Min;
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = box.Min;



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


        public void RenderNodeBoundingBox<T>(IXNAGame game, IQuadTreeNode<T> quadTreeNode) where T : IQuadTreeNode<T>
        {
            if (quadTreeNode == null) return;

            QuadTreeNodeData<T> node = quadTreeNode.NodeData;

            RenderNodeBoundingBox(game, node.UpperLeft);
            RenderNodeBoundingBox(game, node.UpperRight);
            RenderNodeBoundingBox(game, node.LowerLeft);
            RenderNodeBoundingBox(game, node.LowerRight);

          //TODO: Calculate level is quite lame here and slow, since we loop the tree level by level
            int level = QuadTree.CalculateLevel(quadTreeNode);
            Color col;
            if (level < levelColors.Length)
                col = levelColors[level];
            else
                col = levelColors[levelColors.Length - 1];

            game.LineManager3D.AddBox(quadTreeNode.NodeData.BoundingBox.xna(), col);
        }

        public void RenderNodeGroundBoundig<T>(IXNAGame game, IQuadTreeNode<T> quadTreeNode) where T : IQuadTreeNode<T>
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
            Color col;
            if (level < levelColors.Length)
                col = levelColors[level];
            else
                col = levelColors[levelColors.Length - 1];

            Vector3 radius = (node.BoundingBox.Maximum - node.BoundingBox.Minimum).xna();
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = node.BoundingBox.Minimum.xna();
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
        /// This function should return true when the given node should be rendered in given color, otherwise false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quadTreeNode"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public delegate bool RenderNodePredicate<T>(T quadTreeNode, out Color color) where T : class, IQuadTreeNode<T>;

        /// <summary>
        /// This functions loops through all nodes and renders the node when the given predicate returns true for given node. It uses the out color argument to determine the color of the rendered node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="game"></param>
        /// <param name="quadTreeNode"></param>
        /// <param name="predicate"></param>
        public void RenderNodeGroundBoundig<T>(IXNAGame game, T quadTreeNode, RenderNodePredicate<T> predicate) where T : class, IQuadTreeNode<T>
        {
            if (quadTreeNode == null) return;

            QuadTreeNodeData<T> node = quadTreeNode.NodeData;

            RenderNodeGroundBoundig(game, node.UpperLeft, predicate);
            RenderNodeGroundBoundig(game, node.UpperRight, predicate);
            RenderNodeGroundBoundig(game, node.LowerLeft, predicate);
            RenderNodeGroundBoundig(game, node.LowerRight, predicate);


            Color col;

            if (!predicate(quadTreeNode, out col)) return;

            //TODO: Calculate level is quite lame here and slow, since we loop the tree level by level
            int level = QuadTree.CalculateLevel(quadTreeNode);


            Vector3 radius = (node.BoundingBox.Maximum - node.BoundingBox.Minimum).xna();
            Vector3 radX = new Vector3(radius.X, 0, 0);
            Vector3 radY = new Vector3(0, radius.Y, 0);
            Vector3 radZ = new Vector3(0, 0, radius.Z);
            Vector3 min = node.BoundingBox.Minimum.xna();
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

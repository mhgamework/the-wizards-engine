using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Client;
using StillDesign.PhysX;
using MHGameWork.TheWizards.ServerClient.Terrain;
using Microsoft.Xna.Framework;
using Scene = StillDesign.PhysX.Scene;

namespace MHGameWork.TheWizards.Terrain.Client
{
    public class TerrainBlockClientPhysics : IClientPhysicsObject
    {
        private TerrainBlockHeightfieldBuilder builder;
        private StillDesign.PhysX.Scene scene;
        private TerrainFullData terrainData;
        private int blockX;
        private int blockZ;
        private float blockHeight;


        private Actor actor;

        public TerrainBlockClientPhysics(StillDesign.PhysX.Scene scene, TerrainFullData terrainData, int blockX, int blockZ, float blockHeight, TerrainBlockHeightfieldBuilder builder)
        {
            MHGameWork.TheWizards.WorldDatabase.DataRevision rev;

            this.blockHeight = blockHeight;
            this.blockZ = blockZ;
            this.scene = scene;
            this.builder = builder;
            this.terrainData = terrainData;
            this.blockX = blockX;
        }

        #region IClientPhysicsObject Members

        public void EnablePhysics()
        {
            if ( actor != null ) return;
            actor = builder.BuildHeightfieldActor( scene, terrainData, blockX, blockZ, blockHeight );
        }

        public void DisablePhysics()
        {
            if ( actor != null ) actor.Dispose();
            actor = null;
        }

        private ClientPhysicsQuadTreeNode node;
        public ClientPhysicsQuadTreeNode Node
        {
            get
            {
                return node;
            }
            set
            {
                node = value;
            }
        }

        public Microsoft.Xna.Framework.ContainmentType ContainedInNode( ClientPhysicsQuadTreeNode _node )
        {

            /**
             * This comment is correct, but not used right now.
             * 
             * 
            // Since the terrain blocks are supposed to perfectly fit in a quadtreenode, we cannot simply do
            //  a bounding check, since this will cause floating point errors.

            // But since we assume the block perfectly fits a node, we simply decrease the block's size by 0.01 percent
            // (This would cause errors if the quadtreenode wasn't a perfect fit for the terrain, physics being enabled to late)

             * 
             * */


            //Note: if this causes errors, use the trick explained above.
            return _node.NodeData.BoundingBox.xna().Contains( GetBoundingBox() );
        }

        /// <summary>
        /// TODO: y-coords not implemented at this time.
        /// </summary>
        /// <returns></returns>
        public BoundingBox GetBoundingBox()
        {
            //TODO: fix the y coords

            BoundingBox bb = new BoundingBox(
            new Vector3( terrainData.BlockSize * blockX, -5, terrainData.BlockSize * blockZ ) + terrainData.Position
            , new Vector3( terrainData.BlockSize * ( blockX + 1 ), 5, terrainData.BlockSize * ( blockZ + 1 ) ) + terrainData.Position );

            return bb;
        }

        #endregion
    }
}

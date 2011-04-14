using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class TerrainQuadtreeNode
    {
        private TerrainQuadtreeNode parent;
        private BoundingBox boundingBox = new BoundingBox();
        private bool visible = false;

        public TerrainQuadtreeNode()
        {
        }

        ~TerrainQuadtreeNode()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }


        public TerrainQuadtreeNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private TerrainQuadtreeNode upperLeft;
        private TerrainQuadtreeNode upperRight;
        private TerrainQuadtreeNode lowerLeft;
        private TerrainQuadtreeNode lowerRight;

        protected void Dispose( bool disposing )
        {
            lock ( this )
            {
                if ( disposing )
                {
                    if ( upperLeft != null )
                        upperLeft.Dispose();

                    if ( upperRight != null )
                        upperRight.Dispose();

                    if ( lowerLeft != null )
                        lowerLeft.Dispose();

                    if ( lowerRight != null )
                        lowerRight.Dispose();
                }

                upperLeft = null;
                upperRight = null;
                lowerLeft = null;
                lowerRight = null;
                parent = null;
            }

        }

        public TerrainQuadtreeNode UpperLeft
        {
            get { return upperLeft; }
            set { upperLeft = value; }
        }

        public TerrainQuadtreeNode UpperRight
        {
            get { return upperRight; }
            set { upperRight = value; }
        }

        public TerrainQuadtreeNode LowerLeft
        {
            get { return lowerLeft; }
            set { lowerLeft = value; }
        }

        public TerrainQuadtreeNode LowerRight
        {
            get { return lowerRight; }
            set { lowerRight = value; }
        }
    }
}

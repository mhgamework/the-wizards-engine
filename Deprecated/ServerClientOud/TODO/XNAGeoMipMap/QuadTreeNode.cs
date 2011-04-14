using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class QuadTreeNode : TreeNode
    {
        private TreeNode upperLeft;
        private TreeNode upperRight;
        private TreeNode lowerLeft;
        private TreeNode lowerRight;

        public QuadTreeNode()
            : base()
        {
        }

        protected override void Dispose( bool disposing )
        {
            lock( this )
            {
                if( disposing )
                {
                    if( upperLeft != null )
                        upperLeft.Dispose();

                    if( upperRight != null )
                        upperRight.Dispose();

                    if( lowerLeft != null )
                        lowerLeft.Dispose();

                    if( lowerRight != null )
                        lowerRight.Dispose();
                }

                upperLeft = null;
                upperRight = null;
                lowerLeft = null;
                lowerRight = null;
            }

            base.Dispose( disposing );
        }

        public TreeNode UpperLeft
        {
            get { return upperLeft; }
            set { upperLeft = value; }
        }

        public TreeNode UpperRight
        {
            get { return upperRight; }
            set { upperRight = value; }
        }

        public TreeNode LowerLeft
        {
            get { return lowerLeft; }
            set { lowerLeft = value; }
        }

        public TreeNode LowerRight
        {
            get { return lowerRight; }
            set { lowerRight = value; }
        }
    }
}

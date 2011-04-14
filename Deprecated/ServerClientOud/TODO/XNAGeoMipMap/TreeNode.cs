using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class TreeNode : IDisposable
    {
        private TreeNode parent;
        private BoundingBox boundingBox = new BoundingBox();
        private bool visible = false;

        public TreeNode()
        {
        }

        ~TreeNode()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            lock( this )
            {
                parent = null;
            }
        }

        public TreeNode Parent
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



    }
}

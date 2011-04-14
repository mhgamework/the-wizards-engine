using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Entity.Rendering
{
    /// <summary>
    /// At the moment this class uses a internal mechanism that stores the batched render calls, and allows
    /// faster rendering.
    /// </summary>
    public abstract class Material : IDisposable
    {
        /// <summary>
        /// Initialize to one, since i at the moment suspect there will be alot of materials assigned to only one object.
        /// </summary>
        protected IRenderPrimitives[] batchedPrimitives;
        //private List<IRenderPrimitives> batchedPrimitives = new List<IRenderPrimitives>(1);
        /// <summary>
        /// This returns the number of primitives currently batched. rangiing from 0 --> batchedCount -1
        /// </summary>
        protected int batchedCount;


        private Microsoft.Xna.Framework.Matrix worldMatrix;

        public Microsoft.Xna.Framework.Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }


        public Material()
        {

        }

        public void RenderPrimitivesBatched( IRenderPrimitives primitives )
        {
            if ( batchedPrimitives == null )
            {
                batchedPrimitives = new IRenderPrimitives[ 1 ];
            }
            else if ( batchedPrimitives.Length == batchedCount )
            {
                // Double size
                Array.Resize( ref batchedPrimitives, batchedPrimitives.Length << 1 );
            }
            batchedPrimitives[ batchedCount ] = primitives;
            batchedCount++;
        }

        public virtual void Initialize( IXNAGame game )
        {
        }

        public void Render( IXNAGame game )
        {
            RenderInternal( game );
            batchedCount = 0;
        }

        protected virtual void RenderInternal( IXNAGame game )
        {

        }


        public abstract void Dispose();
        

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace MHGameWork.TheWizards.ServerClient.Entity.Rendering
{
    public class EntityRenderData : IDisposable, Database.ISimpleTag<TaggedEntity>
    {
        private TaggedEntity taggedEntity;
        private ObjectRenderData renderData;
        private Matrix worldMatrix;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }



        public void Render( IXNAGame game )
        {
            // Since the objectRenderData is shared among other entities, set the world matrix every time
            renderData.SetWorldMatrix( worldMatrix );

            renderData.Render( game );


        }

        public void Render2( IXNAGame game )
        {
            // TODO: ?Since the objectRenderData is shared among other entities, set the world matrix every time
            renderData.SetWorldMatrix( worldMatrix );

            renderData.Render2( game );


        }

        public void RenderPrimitives( IXNAGame game )
        {
            renderData.RenderPrimitives( game );
        }

        public void RenderSpecialTemp( IXNAGame game, BasicShader shader )
        {
            renderData.SetWorldMatrix( worldMatrix );
            renderData.RenderSpecialTemp( game, shader );
        }

        public void Initialize( IXNAGame game )
        {
            EntityFullData fullData = taggedEntity.GetTag<EntityFullData>();

            worldMatrix = fullData.Transform.CreateMatrix();

            renderData = fullData.TaggedObject.GetTag<ObjectRenderData>();
            renderData.Initialize( game );
        }

        public void Initialize2( IXNAGame game, ColladaShader baseColladaShader )
        {
            EntityFullData fullData = taggedEntity.GetTag<EntityFullData>();

            worldMatrix = fullData.Transform.CreateMatrix();

            renderData = fullData.TaggedObject.GetTag<ObjectRenderData>();
            renderData.Initialize2( game, baseColladaShader );
        }

        #region IDisposable Members

        public void Dispose()
        {
            // EDIT: NOTE: Do not dispose the renderdata since it is shared among entities.
            /*if ( renderData != null )
                renderData.Dispose();*/

            renderData = null;
        }

        #endregion



        #region ISimpleTag<TaggedEntity> Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedEntity>.InitTag( TaggedEntity obj )
        {
            taggedEntity = obj;
            //throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedEntity>.AddReference( TaggedEntity obj )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}

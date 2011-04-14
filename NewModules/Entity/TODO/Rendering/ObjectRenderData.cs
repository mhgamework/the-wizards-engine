using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Entity.Rendering
{
    public class ObjectRenderData : IDisposable, Database.ISimpleTag<TaggedObject>
    {
        private TaggedObject taggedObject;
        private List<ModelRenderData> models;

        public List<ModelRenderData> Models
        {
            get { return models; }
            //set { models = value; }
        }

        public ObjectRenderData()
        {
            models = new List<ModelRenderData>();
        }

        public void SetWorldMatrix( Matrix world )
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].SetParentWorldMatrix( world );
            }

        }

        public void Initialize2( IXNAGame game, ColladaShader baseColladaShader )
        {
            ObjectFullData fullData = taggedObject.GetTag<ObjectFullData>();

            // Dispose all previous data

            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Dispose();
            }

            models.Clear();

            for ( int i = 0; i < fullData.Models.Count; i++ )
            {
                ModelRenderData renderData = new ModelRenderData();
                renderData.Initialize2( game, baseColladaShader, fullData.Models[ i ] );
                models.Add( renderData );
            }
        }

        /// <summary>
        /// TODO: Now this is called by EntityRenderData.Initialize NOT GOOD!!
        /// TODO: Some thing should make this initialize en unload dynamically.
        /// </summary>
        /// <param name="game"></param>
        public void Initialize( IXNAGame game )
        {
            ObjectFullData fullData = taggedObject.GetTag<ObjectFullData>();

            // Dispose all previous data

            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Dispose();
            }

            models.Clear();

            for ( int i = 0; i < fullData.Models.Count; i++ )
            {
                ModelRenderData renderData = new ModelRenderData();
                renderData.Initialize( game, fullData.Models[ i ] );
                models.Add( renderData );
            }

        }

        public void Render2( IXNAGame game )
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Render2( game );
            }
        }

        public void Render( IXNAGame game )
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Render( game );
            }
        }

        public void RenderPrimitives( IXNAGame game )
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].RenderPrimitives();
            }
        }

        public void RenderSpecialTemp( IXNAGame game, BasicShader shader )
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].RenderSpecialTemp( shader );
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Dispose();
            }
        }

        #endregion

        #region ISimpleTag<TaggedObject> Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.InitTag( TaggedObject obj )
        {
            taggedObject = obj;
            //throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.AddReference( TaggedObject obj )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}

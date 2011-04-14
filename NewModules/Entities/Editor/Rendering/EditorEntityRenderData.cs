using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities.Editor;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class EditorEntityRenderData : IDisposable, MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedEntity>
    {
        private EditorObjectRenderData renderData;
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

        public void Initialize( IXNAGame game, EditorObjectRenderData objectRenderData, EditorEntity ent )
        {
            //TODO: this should be a database function

            renderData = objectRenderData;

            worldMatrix = ent.FullData.Transform.CreateMatrix();
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

        public void InitTag( TaggedEntity obj )
        {
        }

        public void AddReference( TaggedEntity obj )
        {
        }

        #endregion
    }
}
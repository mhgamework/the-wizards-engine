using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Entity;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorObjectRenderData : IDisposable
    {
        private List<EditorModelRenderData> models;

        public List<EditorModelRenderData> Models
        {
            get { return models; }
            //set { models = value; }
        }

        public EditorObjectRenderData()
        {
            models = new List<EditorModelRenderData>();
        }

        public void SetWorldMatrix(Matrix world)
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].SetParentWorldMatrix( world );
            }
            
        }

        public void Initialize( IXNAGame game, ObjectFullData fullData )
        {
            // Dispose all previous data

            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Dispose();
            }

            models.Clear();

            for ( int i = 0; i < fullData.Models.Count; i++ )
            {
                EditorModelRenderData renderData = new EditorModelRenderData();
                renderData.Initialize( game, fullData.Models[ i ] );
                models.Add( renderData );
            }

        }

        public void Render( IXNAGame game )
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Render( game );
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
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Entity.Rendering
{
    /// <summary>
    /// TODO: use a quadtree here.
    /// </summary>
    public class EntityRenderer : TWClient.IRenderer
    {
        public TheWizards.Database.Database Database;
        private EntityManagerService ems;
        private List<EntityRenderData> renderDatas = new List<EntityRenderData>();

        public EntityRenderer( TheWizards.Database.Database _database )
        {
            Database = _database;
            ems = _database.FindService<EntityManagerService>();
        }

        public void Render( XNAGame game )
        {
            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
            game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            
            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                renderDatas[ i ].Render( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void Update( XNAGame game )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void Initialize( XNAGame game )
        {
            // WARNING: memory leak
            renderDatas.Clear();
            for ( int i = 0; i < ems.Entities.Count; i++ )
            {
                TaggedEntity entity = ems.Entities[ i ];
                EntityRenderData data = entity.GetTag<EntityRenderData>();
                renderDatas.Add( data );

                //TODO: now the object data is loaded every time
                data.Initialize( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
        }

    }
}

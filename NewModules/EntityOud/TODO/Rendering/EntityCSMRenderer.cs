using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Entity.Rendering
{
    /// <summary>
    /// TODO: use a quadtree here.
    /// </summary>
    public class EntityCSMRenderer : CascadedShadowMaps.ICSMRenderer
    {
        public TheWizards.Database.Database Database;
        private EntityManagerService ems;
        private CascadedShadowMaps.CSMRendererService csmRendererService;
        private List<EntityRenderData> renderDatas = new List<EntityRenderData>();

        private ColladaShader colladaShader;
        private EffectPool effectPool;

        public EntityCSMRenderer( TheWizards.Database.Database _database )
        {
            Database = _database;
            ems = _database.FindService<EntityManagerService>();
            csmRendererService = Database.FindService<CascadedShadowMaps.CSMRendererService>();

        }

        public void Render( XNAGame game )
        {
            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                renderDatas[ i ].Render( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void Update( XNAGame game )
        {
            //throw new Exception( "The method or operation is not implemented." );
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                //Initialize( game );
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.V ) )
            {
                if ( csmRendererService != null )
                    csmRendererService.RenderDebug = !csmRendererService.RenderDebug;
            }
        }

        public void Initialize( XNAGame game )
        {
            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                renderDatas[ i ].Dispose();
            }
            renderDatas.Clear();
            if ( colladaShader != null ) colladaShader.Dispose();
            colladaShader = null;
            if ( effectPool != null ) effectPool.Dispose();
            effectPool = null;
            // WARNING: memory leak
            effectPool = new EffectPool();
            colladaShader = new ColladaShader( game, effectPool );


            for ( int i = 0; i < ems.Entities.Count; i++ )
            {

                TaggedEntity entity = ems.Entities[ i ];
                EntityRenderData data = entity.GetTag<EntityRenderData>();
                renderDatas.Add( data );

                //TODO: now the object data is loaded every time
                data.Initialize2( game, colladaShader );
                //data.Initialize( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
        }


        #region ICSMRenderer Members

        public void RenderDepth( XNAGame game, BasicShader depthShader )
        {
            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                //Matrix worldMatrix = renderDatas[ i ].WorldMatrix;
                //depthShader.SetParameter( "g_matWorld", worldMatrix );

                //Matrix transpose, inverseTranspose;
                //Matrix transform = worldMatrix;
                //Matrix.Transpose( ref transform, out transpose );
                //Matrix.Invert( ref transpose, out inverseTranspose );
                //depthShader.SetParameter( "g_matWorldIT", inverseTranspose );


                renderDatas[ i ].RenderSpecialTemp( game, depthShader );
            }



        }

        public void RenderNormal( XNAGame game, BasicShader normalShader )
        {
            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace;
            colladaShader.ViewProjection = game.Camera.ViewProjection;
            colladaShader.ViewInverse = game.Camera.ViewInverse;
            if ( csmRendererService != null )
                colladaShader.ShadowOcclusionTexture = csmRendererService.GetShadowOcclusionTexture();
            int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            colladaShader.BackbufferSize = new Vector2( width, height );
            /*game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            game.GraphicsDevice.RenderState.ReferenceAlpha = 128;
            game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            game.GraphicsDevice.RenderState.ReferenceAlpha = 0;*/

            if ( csmRendererService != null )
                colladaShader.LightDirection = csmRendererService.light.Direction;
            if ( csmRendererService != null )
                colladaShader.LightColor = csmRendererService.light.Color;
            colladaShader.AmbientColor = new Vector4( 0.3f, 0.3f, 0.3f, 1 );

            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                //Matrix worldMatrix = renderDatas[ i ].WorldMatrix;
                //depthShader.SetParameter( "g_matWorld", worldMatrix );

                //Matrix transpose, inverseTranspose;
                //Matrix transform = worldMatrix;
                //Matrix.Transpose( ref transform, out transpose );
                //Matrix.Invert( ref transpose, out inverseTranspose );
                //depthShader.SetParameter( "g_matWorldIT", inverseTranspose );



                //renderDatas[ i ].RenderSpecialTemp( game, normalShader );
                renderDatas[ i ].Render2( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace;
        }

        public void RenderShadowMap( XNAGame game, BasicShader shadowMapShader )
        {
            //throw new Exception( "The method or operation is not implemented." );
            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                //Matrix worldMatrix = renderDatas[ i ].WorldMatrix;
                //depthShader.SetParameter( "g_matWorld", worldMatrix );

                //Matrix transpose, inverseTranspose;
                //Matrix transform = worldMatrix;
                //Matrix.Transpose( ref transform, out transpose );
                //Matrix.Invert( ref transpose, out inverseTranspose );
                //depthShader.SetParameter( "g_matWorldIT", inverseTranspose );



                renderDatas[ i ].RenderSpecialTemp( game, shadowMapShader );
            }
        }

        #endregion
    }
}

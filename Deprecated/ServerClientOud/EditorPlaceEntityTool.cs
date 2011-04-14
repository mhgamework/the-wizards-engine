using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.ServerClient
{
    public class EditorPlaceEntityTool : IEditorTool
    {
        private TWEditor editor;

        public TWEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }

        private WereldEntity entity;

        public WereldEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        private Vector3 targetPosition;

        public EditorPlaceEntityTool( TWEditor nEditor )
        {
            editor = nEditor;
        }

        BasicEffect effect;
        public void Render()
        {
            editor.Game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            editor.Game.GraphicsDevice.RenderState.SourceBlend = Microsoft.Xna.Framework.Graphics.Blend.BlendFactor;
            editor.Game.GraphicsDevice.RenderState.DestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseBlendFactor;
            editor.Game.GraphicsDevice.RenderState.BlendFactor = new Color( new Vector4( 0.5f, 0.5f, 0.5f, 1 ) );

            editor.Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;



            if ( effect == null ) effect = new BasicEffect( Game.GraphicsDevice, null );

            effect.View = Game.Camera.View;
            effect.Projection = Game.Camera.Projection;

            effect.World = Matrix.CreateFromYawPitchRoll( 0, -MathHelper.PiOver2, 0 );
            effect.World *= Matrix.CreateScale( 0.02f );
            effect.World *= Matrix.CreateTranslation( targetPosition );

            effect.Begin();
            for ( int iPass = 0; iPass < effect.CurrentTechnique.Passes.Count; iPass++ )
            {
                effect.CurrentTechnique.Passes[ iPass ].Begin();

                Game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
                entity.Render();
                Game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                entity.Render();

                effect.CurrentTechnique.Passes[ iPass ].End();

            }
            effect.End();


            Game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;


        }

        public void Update()
        {

            if ( Game.Mouse.CursorEnabled )
            {
                if ( editor.ImgWereldView.CheckOnControl( Game.Mouse.CursorPosition ) )
                {
                    Vector3 pos = editor.RaycastWereld( new Vector2(
                        Game.Mouse.CursorPosition.X - editor.ImgWereldView.Position.X,
                        Game.Mouse.CursorPosition.Y - editor.ImgWereldView.Position.Y ) );
                    targetPosition = pos;
                    //Game.LineManager3D.AddLine( pos, pos + new Vector3( 0, 20, 0 ), Color.Green );

                    if ( Game.Mouse.LeftMouseJustPressed )
                    {
                        WereldEntity ent = entity.Clone( editor.Wereld );
                        ent.Models[ 0 ].WorldMatrix = Matrix.CreateFromYawPitchRoll( 0, -MathHelper.PiOver2, 0 );
                        ent.Models[ 0 ].WorldMatrix *= Matrix.CreateScale( 0.02f );
                        ent.Models[ 0 ].WorldMatrix *= Matrix.CreateTranslation( targetPosition );
                        editor.Wereld.Entities.AddNew( ent );
                    }
                }

            }
        }



        public void OnActivate()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void OnDeactivate()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        public XNAGame Game { get { return editor.Game; } }


        public static void TestTool()
        {

            TestXNAGame game = new TestXNAGame( "EditorPlaceEntityTool.TestTool" );

            TWWereld wereld = null;

            TWEditor editor = null;

            game.initCode =
                delegate
                {
                    wereld = new TWWereld( game );
                    editor = new TWEditor( wereld );



                    editor.Exit +=
                        delegate
                        {
                            game.Exit();
                        };

                    EditorPlaceEntityTool tool = new EditorPlaceEntityTool( editor );

                    WereldEntity entity = new WereldEntity( wereld );
                    WereldModel model = new WereldModel();
                    WereldMesh mesh = new WereldMesh( wereld );
                    mesh.LoadFromXml( TWXmlNode.GetRootNodeFromFile( game.EngineFiles.DirUnitTests + "Mesh_TestSerialize.xml" ) );

                    model.Mesh = mesh;
                    entity.AddNewModel( model );

                    tool.Entity = entity;

                    editor.SetActiveTool( tool );

                    game.Mouse.CursorEnabled = true;



                };

            game.renderCode =
                delegate
                {
                    editor.Render();
                };

            game.updateCode =
                delegate
                {
                    editor.Update();
                };

            game.Run();
        }



    }
}

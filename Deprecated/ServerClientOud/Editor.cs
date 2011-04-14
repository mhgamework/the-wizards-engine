using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient
{
    public class TWEditor
    {
        private TWWereld wereld;

        public TWWereld Wereld
        {
            get { return wereld; }
            set { wereld = value; }
        }

        private Gui.GuiWindow rootWindow;

        private Gui.GuiButton knpExit;

        private Gui.GuiImage imgWereldView;

        public Gui.GuiImage ImgWereldView
        {
            get { return imgWereldView; }
            set { imgWereldView = value; }
        }


        private SpriteFont font;

        public event EventHandler Exit;
        //public event EventHandler<object> ToolActivated;
        private RenderTarget2D wereldViewRenderTarget;
        private Viewport wereldViewViewport;
        private EditorCamera wereldViewCamera;

        public EditorCamera WereldViewCamera
        {
            get { return wereldViewCamera; }
            set { wereldViewCamera = value; }
        }
	

        private EditorGrid grid;

        private IEditorTool activeTool;

        public TWEditor( TWWereld nWereld )
        {
            wereld = nWereld;

            font = wereld.Game.Content.Load<SpriteFont>( wereld.Game.EngineFiles.DefaultFontAsset );

            rootWindow = new MHGameWork.TheWizards.ServerClient.Gui.GuiWindow();
            rootWindow.Service = wereld.Game.GuiService;
            wereld.Game.GuiService.TopLevelControls.Add( rootWindow );
            rootWindow.tex = Texture2D.FromFile( wereld.Game.GraphicsDevice, wereld.Game.EngineFiles.GuiDefaultSkin.GetFullFilename() );
            rootWindow.tileSet = new Gui.TileSet( 228, 150,
                          new int[] { 52, 415, 80 },
                          new int[] { 76, 291, 49 } );

            rootWindow.Position = Vector2.Zero;
            rootWindow.Size = new Vector2( wereld.Game.Window.ClientBounds.Width, wereld.Game.Window.ClientBounds.Height );
            rootWindow.BorderStyle = MHGameWork.TheWizards.ServerClient.Gui.GuiWindow.WindowBorderStyle.None;
            rootWindow.Load( wereld.Game.GraphicsDevice );

            knpExit = CreateButton( new Vector2( 600, 500 ), new Vector2( 100, 45 ), "Exit" );
            rootWindow.Controls.Add( knpExit );
            knpExit.MouseUp += new EventHandler<MHGameWork.TheWizards.ServerClient.Gui.MouseEventArgs>( knpExit_MouseUp );

            imgWereldView = new MHGameWork.TheWizards.ServerClient.Gui.GuiImage();
            imgWereldView.Position = new Vector2( 10, 50 );
            imgWereldView.BackgroundColor = Color.Sienna;
            imgWereldView.Size = new Vector2( 500, 500 );
            imgWereldView.Load( wereld.Game.GraphicsDevice );
            imgWereldView.Service = wereld.Game.GuiService;
            imgWereldView.MouseDown += new EventHandler<MHGameWork.TheWizards.ServerClient.Gui.MouseEventArgs>( imgWereldView_MouseDown );
            rootWindow.Controls.Add( imgWereldView );

            CreateWereldViewRenderTarget();

            wereldViewCamera = new EditorCamera( this );

            grid = new EditorGrid( Game );

        }

        void imgWereldView_MouseDown( object sender, MHGameWork.TheWizards.ServerClient.Gui.MouseEventArgs e )
        {
            UpdateWereldViewCameraMoveMode();
        }

        public void SetActiveTool( IEditorTool tool )
        {
            if ( tool == activeTool ) return;
            if ( activeTool != null )
            {
                activeTool.OnDeactivate();
            }

            activeTool = tool;

            activeTool.OnActivate();
        }

        private Gui.GuiButton CreateButton( Vector2 position, Vector2 size, string text )
        {
            Gui.GuiButton button = new MHGameWork.TheWizards.ServerClient.Gui.GuiButton();

            button.Texture = Texture2D.FromFile( wereld.Game.GraphicsDevice,
                wereld.Game.EngineFiles.GuiButton001.GetFullFilename() );
            button.TileSet = new Gui.TileSet( 18, 12, new int[] { 15, 194, 15 }, new int[] { 15, 43, 15 } );
            button.Font = font;
            button.Service = wereld.Game.GuiService;
            //wereld.Game.GuiService.TopLevelControls.Add( button );
            button.Load( wereld.Game.GraphicsDevice );

            button.Position = position;
            button.Size = size;
            button.Text = text;


            return button;
        }

        void knpExit_MouseUp( object sender, MHGameWork.TheWizards.ServerClient.Gui.MouseEventArgs e )
        {
            if ( Exit != null ) Exit( this, null );

        }

        private void CreateWereldViewRenderTarget()
        {
            if ( wereldViewRenderTarget != null ) wereldViewRenderTarget.Dispose();
            wereldViewRenderTarget = null;

            Point size = new Point( 500, 500 );

            //wereldViewRenderTarget = new RenderTarget2D( wereld.Game.GraphicsDevice,
            //    (int)imgWereldView.Size.X, (int)imgWereldView.Size.Y, 1, SurfaceFormat.Color );

            wereldViewRenderTarget = new RenderTarget2D( Game.GraphicsDevice,
                size.X, size.Y, 1,
                SurfaceFormat.Color
               , Game.GraphicsDevice.PresentationParameters.MultiSampleType
               , Game.GraphicsDevice.PresentationParameters.MultiSampleQuality );

            wereldViewViewport = Game.GraphicsDevice.Viewport;
            wereldViewViewport.Width = size.X;
            wereldViewViewport.Height = size.Y;
        }


        public void UpdateWereldViewCameraMoveMode()
        {
            if ( Game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) )
            {


                if ( Game.Mouse.LeftMousePressed && Game.Mouse.RightMousePressed )
                {

                    wereldViewCamera.ActiveMoveMode = EditorCamera.MoveMode.MoveY;
                }
                else if ( Game.Mouse.LeftMousePressed )
                {
                    wereldViewCamera.ActiveMoveMode = EditorCamera.MoveMode.MoveXZ;
                }
                else if ( Game.Mouse.RightMousePressed )
                {
                    if ( !Game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftControl ) )
                    {
                        wereldViewCamera.ActiveMoveMode = EditorCamera.MoveMode.Orbit;
                        if ( Game.Mouse.CursorEnabled ) wereldViewCamera.OrbitPoint =
                              RaycastWereld( ImgWereldView.Size * 0.5f );

                    }
                    else
                    {
                        wereldViewCamera.ActiveMoveMode = EditorCamera.MoveMode.RotateYawRoll;
                    }
                }
                else
                {
                    wereldViewCamera.ActiveMoveMode = EditorCamera.MoveMode.None;
                }
            }
            else
            {
                wereldViewCamera.ActiveMoveMode = EditorCamera.MoveMode.None;

            }

            if ( wereldViewCamera.ActiveMoveMode == EditorCamera.MoveMode.None )
            {
                Game.Mouse.CursorEnabled = true;
            }
            else
            {
                Game.Mouse.CursorEnabled = false;
            }
        }

        public void Update()
        {
            if ( Game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) )
            {
                if ( Game.Mouse.CursorEnabled )
                {
                    // Only allow camera movement

                    if ( imgWereldView.CheckOnControl( Game.Mouse.CursorPosition ) )
                    {
                        UpdateWereldViewCameraMoveMode();

                    }

                }
            }
            else
            {
                if ( activeTool != null ) activeTool.Update();
            }


            if ( wereldViewCamera.ActiveMoveMode != EditorCamera.MoveMode.None )
            {
                UpdateWereldViewCameraMoveMode();

            }

            if ( wereldViewCamera.Enabled )
                wereldViewCamera.Update();

        }

        public Vector2 GetWereldViewCursorPos()
        {
            return new Vector2(
                        Game.Mouse.CursorPosition.X - ImgWereldView.Position.X,
                        Game.Mouse.CursorPosition.Y - ImgWereldView.Position.Y );
        }

        BasicEffect effect;
        public void RenderEntity( WereldEntity ent )
        {
            if ( effect == null ) effect = new BasicEffect( Game.GraphicsDevice, null );

            //effect.AmbientLightColor = Color.Gray;
            //effect.DiffuseColor = Color.DarkCyan;
            //effect.DirectionalLight0.Enabled = true;
            //effect.DirectionalLight0.DiffuseColor = Color.DarkGreen;
            //effect.DirectionalLight0.Direction = Vector3.Normalize( new Vector3( 1, 2, 1 ) );

            effect.EnableDefaultLighting();
            effect.PreferPerPixelLighting = true;

            effect.View = Game.Camera.View;
            effect.Projection = Game.Camera.Projection;

            effect.World = ent.Models[ 0 ].WorldMatrix;

            effect.Begin();
            for ( int iPass = 0; iPass < effect.CurrentTechnique.Passes.Count; iPass++ )
            {
                effect.CurrentTechnique.Passes[ iPass ].Begin();

                Game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                ent.Models[ 0 ].Mesh.DrawPrimitives();

                effect.CurrentTechnique.Passes[ iPass ].End();

            }
            effect.End();
        }

        public Vector3 RaycastWereld( Vector2 pos )
        {
            Plane pl = new Plane( 0, 1, 0, 0 );
            Ray ray = GetWereldViewRay( pos );
            float? result = ray.Intersects( pl );



            if ( !result.HasValue )
            {
                ray.Direction = -ray.Direction;
                result = ray.Intersects( pl );
                if ( !result.HasValue )
                    //throw new InvalidOperationException( "This is impossible with the plane!" );
                    //Whell, this isn't impossible when the user looks parallel with the plane
                    // use vector3.Zero?
                    // TODO: WARNING: This stinks real bad.
                    return Vector3.Zero;
            }
            return ray.Position + ( ray.Direction * result.Value );
        }
        public List<EditorRaycastResult<WereldEntity>> RaycastWereldEntities( Vector2 pos )
        {
            List<EditorRaycastResult<WereldEntity>> ret = new List<EditorRaycastResult<WereldEntity>>();
            Ray ray = GetWereldViewRay( GetWereldViewCursorPos() );
            for ( int i = 0; i < wereld.Entities.Count; i++ )
            {

                float? dist;
                BoundingBox BB = wereld.Entities.GetByIndex( i ).BoundingBox;
                ray.Intersects( ref BB, out dist );
                if ( dist.HasValue )
                {
                    ret.Add( new EditorRaycastResult<WereldEntity>( dist.Value, wereld.Entities.GetByIndex( i ) ) );
                }
            }

            return ret;
        }


        public Ray GetWereldViewRay( Vector2 pos )
        {
            Viewport viewport = wereldViewViewport;

            Vector3 nearSource = viewport.Unproject( new Vector3( pos.X, pos.Y, viewport.MinDepth ), wereldViewCamera.Projection, wereldViewCamera.View, Matrix.Identity );
            Vector3 farSource = viewport.Unproject( new Vector3( pos.X, pos.Y, viewport.MaxDepth ), wereldViewCamera.Projection, wereldViewCamera.View, Matrix.Identity );
            Vector3 direction = farSource - nearSource;
            direction.Normalize();

            Ray ray = new Ray( nearSource, direction );

            return ray;


        }

        public void Render()
        {
            Viewport oldViewport = Game.GraphicsDevice.Viewport;
            RenderTarget oldTarget = Game.GraphicsDevice.GetRenderTarget( 0 );

            //Set new rendertarget
            Game.GraphicsDevice.SetRenderTarget( 0, wereldViewRenderTarget );
            Game.GraphicsDevice.Viewport = wereldViewViewport;
            Game.SetCamera( wereldViewCamera );

            //Render the world.

            Game.GraphicsDevice.Clear( ClearOptions.Target, Color.Gold, 1, 0 );


            Game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            Game.GraphicsDevice.RenderState.DepthBufferEnable = true;



            for ( int i = 0; i < wereld.Entities.Count; i++ )
            {
                RenderEntity( wereld.Entities.GetByIndex( i ) );
            }


            grid.Render();

            if ( activeTool != null ) activeTool.Render();

            Game.LineManager3D.Render();








            //Restore rendertarget
            Game.GraphicsDevice.SetRenderTarget( 0, oldTarget as RenderTarget2D );

            Game.GraphicsDevice.Viewport = oldViewport;

            //if ( imgWereldView.Texture != null ) imgWereldView.Texture.Dispose();

            //Texture2D tex = wereldViewRenderTarget.GetTexture();
            //tex.Save( "Temp.png", ImageFileFormat.Png );
            //imgWereldView.boe = tex;
            imgWereldView.Texture = wereldViewRenderTarget.GetTexture();

        }


        public XNAGame Game
        { get { return wereld.Game; } }



        public static void TestEditor()
        {
            TWEditor editor = null;

            TestXNAGame game = new TestXNAGame( "Editor.TestEditor" );

            game.initCode =
                delegate
                {
                    editor = new TWEditor( new TWWereld( game ) );
                    game.Mouse.CursorEnabled = true;

                    editor.Exit += delegate { game.Exit(); };
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

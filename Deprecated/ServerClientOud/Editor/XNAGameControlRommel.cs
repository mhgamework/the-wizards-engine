using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{

    /// <summary>
    /// Should add a component sysmem like in XNA for Initialize,load,render,update
    /// </summary>
    public class XNAGameControl : Nuclex.GameControl, IXNAGame
    {


        /**
         * 
         * The XNA Rendering Part
         * 
         **/



        /*private GraphicsDevice graphicsDevice;

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            CreateGraphicsDevice();
            ResetGraphicsDevice();
        }

        private void CreateGraphicsDevice()
    {

    }
        private void ResetGraphicsDevice()
        {

        }*/



















































        private XNAGameFiles engineFiles;

        public XNAGameFiles EngineFiles
        {
            get { return engineFiles; }
            set { engineFiles = value; }
        }

        private ICamera camera;

        public ICamera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        private TWMouse mouse;

        public TWMouse Mouse
        {
            get { return mouse; }
            set { mouse = value; }
        }

        private TWKeyboard keyboard;

        public TWKeyboard Keyboard
        {
            get { return keyboard; }
            set { keyboard = value; }
        }

        private LineManager3D lineManager3D;

        public LineManager3D LineManager3D
        {
            get { return lineManager3D; }
        }

        EditorGrid grid;

        private EditorCamera editorCamera;

        public EditorCamera EditorCamera
        {
            get { return editorCamera; }
            //set { editorCamera = value; }
        }

        private bool isActive;

        /// <summary>
        /// TODO: should be true when this control has focus
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }



        //EditorGizmoTranslation tempGizmo;
        //EditorGizmoRotation tempGizmo;

        //public event EventHandler Render;
        //public event EventHandler MouseDown;
        //public event EventHandler MousePressed;
        //public event EventHandler MouseUp;

        public event EventHandler InitializeEvent;
        public event GameTimeEventHandler DrawEvent;
        public event GameTimeEventHandler UpdateEvent;
        public delegate void GameTimeEventHandler( GameTime ntime );

        public XNAGameControl()
        {
            engineFiles = new XNAGameFiles();
            engineFiles.LoadDefaults( System.Windows.Forms.Application.StartupPath + "\\" );
            editorCamera = new EditorCamera( this );
            camera = editorCamera;
            mouse = new TWMouse( this );
            keyboard = new TWKeyboard();
            grid = new EditorGrid( this );
            Mouse.CursorEnabled = true;

            this.Activated += new EventHandler( ModelViewerXNA_Activated );
            this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>( graphics_PreparingDeviceSettings );


        }



        void graphics_PreparingDeviceSettings( object sender, PreparingDeviceSettingsEventArgs e )
        {
            foreach ( GraphicsAdapter curAdapter in GraphicsAdapter.Adapters )
            {
                if ( curAdapter.Description.Contains( "NVIDIA PerfHUD" ) )
                {
                    e.GraphicsDeviceInformation.Adapter = curAdapter;
                    e.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
                    System.Windows.Forms.MessageBox.Show( "Using NVIDIA PerfHUD adapter!" );
                    break;
                }
            }
        }

        public void SetCamera( ICamera cam )
        {
            camera = cam;
        }

        void ModelViewerXNA_Activated( object sender, EventArgs e )
        {
            // Set the mouse window handle
            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = this.Handle;
        }

        protected override void Initialize()
        {
            base.Initialize();
            lineManager3D = new LineManager3D( this );

            if ( InitializeEvent != null ) InitializeEvent( this, null );


        }





        protected override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {

            base.Update( gameTime );

            mouse.UpdateMouseState();
            keyboard.UpdateKeyboardState( Microsoft.Xna.Framework.Input.Keyboard.GetState() );
            if ( !UpdateCameraMoveMode() )
            {
                //Check for a select
                if ( mouse.LeftMouseJustPressed )
                {
                    //TODO

                    //Vector2 cursorPos;
                    //cursorPos = new Vector2( mouse.CursorPosition.X, mouse.CursorPosition.Y );
                    //EditorModel model = editorObject.RaycastModel( GetWereldViewRay( cursorPos ) );
                }


            }

            editorCamera.Update();


            if ( UpdateEvent != null ) UpdateEvent( gameTime );
        }

        public Ray GetWereldViewRay( Vector2 pos )
        {
            Viewport viewport = GraphicsDevice.Viewport;

            Vector3 nearSource = viewport.Unproject( new Vector3( pos.X, pos.Y, viewport.MinDepth ), camera.Projection, camera.View, Matrix.Identity );
            Vector3 farSource = viewport.Unproject( new Vector3( pos.X, pos.Y, viewport.MaxDepth ), camera.Projection, camera.View, Matrix.Identity );
            Vector3 direction = farSource - nearSource;
            direction.Normalize();

            Ray ray = new Ray( nearSource, direction );

            return ray;


        }

        public bool UpdateCameraMoveMode()
        {
            if ( Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) )
            {


                if ( Mouse.LeftMousePressed && Mouse.RightMousePressed )
                {

                    editorCamera.ActiveMoveMode = EditorCamera.MoveMode.MoveY;
                }
                else if ( Mouse.LeftMousePressed )
                {
                    editorCamera.ActiveMoveMode = EditorCamera.MoveMode.MoveXZ;
                }
                else if ( Mouse.RightMousePressed )
                {
                    if ( !Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftControl ) )
                    {
                        editorCamera.ActiveMoveMode = EditorCamera.MoveMode.Orbit;
                        //if ( Mouse.CursorEnabled ) camera.OrbitPoint =
                        //      RaycastWereld( ImgWereldView.Size * 0.5f );

                    }
                    else
                    {
                        editorCamera.ActiveMoveMode = EditorCamera.MoveMode.RotateYawRoll;
                    }
                }
                else
                {
                    editorCamera.ActiveMoveMode = EditorCamera.MoveMode.None;
                }
            }
            else
            {
                editorCamera.ActiveMoveMode = EditorCamera.MoveMode.None;

            }

            if ( editorCamera.ActiveMoveMode == EditorCamera.MoveMode.None )
            {
                Mouse.CursorEnabled = true;
            }
            else
            {
                Mouse.CursorEnabled = false;
                return true;
            }
            return false;
        }


        protected override void Draw( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Draw( gameTime );

            this.graphics.GraphicsDevice.Clear( Microsoft.Xna.Framework.Graphics.Color.SkyBlue );
            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            grid.Render();

            if ( DrawEvent != null ) DrawEvent( gameTime );

            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            lineManager3D.Render();


        }

        public bool IsCursorOnControl()
        {
            if ( mouse.CursorEnabled == false ) throw new InvalidOperationException( "Cursor is not enabled!" );

            if ( mouse.CursorPosition.X < 0 ) return false;
            if ( mouse.CursorPosition.Y < 0 ) return false;
            if ( mouse.CursorPosition.X > Size.Width ) return false;
            if ( mouse.CursorPosition.Y > Size.Height ) return false;

            return true;
        }

        #region IXNAGame Members

        public Microsoft.Xna.Framework.Graphics.GraphicsDevice GraphicsDevice
        {
            get { return base.graphics.GraphicsDevice; }
        }


        #endregion

        Microsoft.Xna.Framework.Point IXNAGame.ClientSize
        {
            get { return new Microsoft.Xna.Framework.Point( base.Size.Width, base.Size.Height ); }
        }












        private static XNAGameControlTestForm form = new XNAGameControlTestForm();
        private static XNAGameControl panel1;
        private static XNAGameControl panel2;
        private static DevComponents.DotNetBar.DockContainerItem item1;
        private static DevComponents.DotNetBar.DockContainerItem item2;

        public static void TestCreateDispose()
        {
            form.KeyPreview = true;
            form.KeyDown += new KeyEventHandler( form_KeyDown );
            Application.Run( form );



        }



        static void form_KeyDown( object sender, KeyEventArgs e )
        
        {
            form.bar1.RecalcLayout();
            if (e.KeyCode == Keys.P)
            {
                if ( panel1 != null ) return;

                item1 = new DevComponents.DotNetBar.DockContainerItem();
                form.bar1.Items.Add( item1 );
                item1.Control = new DevComponents.DotNetBar.PanelDockContainer();

                panel1 = new XNAGameControl();
                item1.Control.Controls.Add( panel1 );
                panel1.BackColor = System.Drawing.Color.Red;
                panel1.Dock = DockStyle.Fill;

                if ( panel1 == null ) return;
                panel1.Exit();
                panel1.Dispose();
                panel1 = null;

                item1.Dispose();
                item1 = null;




                item1 = new DevComponents.DotNetBar.DockContainerItem();
                form.bar1.Items.Add( item1 );
                item1.Control = new DevComponents.DotNetBar.PanelDockContainer();

                panel1 = new XNAGameControl();
                item1.Control.Controls.Add( panel1 );
                //item1.Text = "hi";
                panel1.BackColor = System.Drawing.Color.Red;
                panel1.Dock = DockStyle.Fill;

            }

            if ( e.KeyCode == Keys.A )
            {
                if ( panel1 != null ) return;

                item1 = new DevComponents.DotNetBar.DockContainerItem();
                form.bar1.Items.Add( item1 );
                item1.Control = new DevComponents.DotNetBar.PanelDockContainer();

                panel1 = new XNAGameControl();
                item1.Control.Controls.Add( panel1 );
                panel1.BackColor = System.Drawing.Color.Red;
                panel1.Dock = DockStyle.Fill;


            }
            if ( e.KeyCode == Keys.B )
            {
                if ( panel1 == null ) return;
                panel1.Exit();
                panel1.Dispose();
                panel1 = null;

                //item1.Dispose();
                item1 = null;

            }
            if ( e.KeyCode == Keys.C )
            {
                if ( panel2 != null ) return;
                item2 = new DevComponents.DotNetBar.DockContainerItem();
                form.bar1.Items.Add( item2 );
                item2.Control = new DevComponents.DotNetBar.PanelDockContainer();

                panel2 = new XNAGameControl();
                item2.Control.Controls.Add( panel2 );
                panel2.BackColor = System.Drawing.Color.Green;
                panel2.Dock = DockStyle.Fill;
            }
            if ( e.KeyCode == Keys.D )
            {
                if ( panel2 == null ) return;
                panel2.Exit();
                panel2.Dispose();
                panel2 = null;

                form.bar1.Items.Remove( item2 );
                item2.Dispose();
                item2 = null;
            }

        }

    }
}

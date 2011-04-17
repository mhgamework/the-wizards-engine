using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{

    /// <summary>
    /// Should add a component system like in XNA for Initialize,load,render,update?
    /// TODO: There's a lot of code in common with XNAGame in this class. It should be linked using inheritance or smth.
    /// 
    /// </summary>
    public class XNAGameControl : Nuclex.GameControl, IXNAGame
    {
        private float elapsed;
        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public float Elapsed
        {
            get { return elapsed; }
            set { elapsed = value; }
        }

        private XNAGameFiles engineFiles;
        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public XNAGameFiles EngineFiles
        {
            get { return engineFiles; }
            set { engineFiles = value; }
        }

        private ICamera camera;

        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public ICamera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        private TWMouse mouse;

        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public TWMouse Mouse
        {
            get { return mouse; }
            set { mouse = value; }
        }

        private TWKeyboard keyboard;

        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public TWKeyboard Keyboard
        {
            get { return keyboard; }
            set { keyboard = value; }
        }

        private LineManager3D lineManager3D;

        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public LineManager3D LineManager3D
        {
            get { return lineManager3D; }
        }

        EditorGridOld grid;

        private EditorCameraOld editorCamera;

        [System.ComponentModel.DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public EditorCameraOld EditorCamera
        {
            get { return editorCamera; }
            //set { editorCamera = value; }
        }

        private List<IXNAObject> xnaObjects = new List<IXNAObject>();

        /// <summary>
        /// To detect a change is the cursor enabled value
        /// </summary>
        private bool cursorEnabledLastFrame;



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
            editorCamera = new EditorCameraOld( this );
            camera = editorCamera;
            mouse = new TWMouse( this );
            keyboard = new TWKeyboard();
            grid = new EditorGridOld( this );
            Mouse.CursorEnabled = true;

            this.Activated += new EventHandler( ModelViewerXNA_Activated );
            this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>( graphics_PreparingDeviceSettings );
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( XNAGameControl_KeyDown );
            this.Disposed += new EventHandler( XNAGameControl_Disposed );

            // To ensure the cursor's first state is visible, so that cursor.show is not called before cursor.hide in update()
            cursorEnabledLastFrame = true;

        
        }

        void XNAGameControl_Disposed( object sender, EventArgs e )
        {
            //TODO: validate
            Exit();
        }

        void XNAGameControl_KeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
        {

            e.Handled = true;
            e.SuppressKeyPress = true;

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
            for ( int i = 0; i < xnaObjects.Count; i++ )
            {
                xnaObjects[ i ].Initialize( this );
            }

        }





        protected override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {

            base.Update( gameTime );

            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            mouse.UpdateMouseState();
            keyboard.UpdateKeyboardState( Microsoft.Xna.Framework.Input.Keyboard.GetState() );
            UpdateCameraMoveMode();
            editorCamera.Update();


            if ( UpdateEvent != null ) UpdateEvent( gameTime );
            for ( int i = 0; i < xnaObjects.Count; i++ )
            {
                xnaObjects[ i ].Update( this );
            }

            // Does not seem to work
            if ( keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) )
            {
                if ( TopLevelControl is DevExpress.XtraBars.Ribbon.RibbonForm )
                {
                    ( (DevExpress.XtraBars.Ribbon.RibbonForm)TopLevelControl ).Ribbon.KeyTipManager.HideKeyTips();
                }
            }

            // Check if cursor enabled changed
            if ( cursorEnabledLastFrame != Mouse.CursorEnabled )
            {
                if ( mouse.CursorEnabled )
                {
                    System.Windows.Forms.Cursor.Show();
                }
                else
                {
                    System.Windows.Forms.Cursor.Hide();
                }
                cursorEnabledLastFrame = mouse.CursorEnabled;
            }

            for (int i = 0; i < shaders.Count; i++)
            {
                var shader = shaders[i];
                shader.Update();
            }

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
            return editorCamera.UpdateCameraMoveModeDefaultControls();
        }


        protected override void Draw( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Draw( gameTime );

            this.graphics.GraphicsDevice.Clear( Microsoft.Xna.Framework.Graphics.Color.SkyBlue );
            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            grid.Render();

            if ( DrawEvent != null ) DrawEvent( gameTime );
            for ( int i = 0; i < xnaObjects.Count; i++ )
            {
                xnaObjects[ i ].Render( this );
            }

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

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );
            editorCamera.AspectRatio = (float)Width / Height;
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


        #region IXNAGame Members


        public void AddXNAObject( MHGameWork.TheWizards.Graphics.IXNAObject obj )
        {
            xnaObjects.Add( obj );
        }

        #endregion

        #region IXNAGame Members



        public bool IsCursorInWindow()
        {
            return IsCursorOnControl();
        }

        private List<BasicShader> shaders = new List<BasicShader>();
        public void AddBasicShader(BasicShader basicShader)
        {
            shaders.Add(basicShader);
        }

        public void InvokeUpdate(Action action)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

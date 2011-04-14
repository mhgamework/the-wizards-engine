using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class ModelViewerXNA : Nuclex.GameControl, IXNAGame
    {
        private EditorObject editorObject;

        public EditorObject EditorObject
        {
            get { return editorObject; }
            set { editorObject = value; }
        }


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

        EditorCamera editorCamera;

        EditorGizmoTranslation gizmoTranslation;
        EditorGizmoRotation gizmoRotation;
        EditorGizmoScaling gizmoScaling;
        //EditorGizmoTranslation tempGizmo;
        //EditorGizmoRotation tempGizmo;

        //public event EventHandler Render;
        //public event EventHandler MouseDown;
        //public event EventHandler MousePressed;
        //public event EventHandler MouseUp;

        public ModelViewerXNA( EditorObject nObj )
        {
            editorObject = nObj;
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

        void btnObjectScale_Click( object sender, EventArgs e )
        {
            gizmoScaling.Enabled = true;

            gizmoTranslation.Enabled = false;
            gizmoRotation.Enabled = false;
            UpdateTransformButtons();
        }

        void btnObjectRotate_Click( object sender, EventArgs e )
        {
            gizmoScaling.Enabled = false;

            gizmoRotation.Enabled = !gizmoRotation.Enabled;
            UpdateTransformButtons();
        }

        void btnObjectMove_Click( object sender, EventArgs e )
        {
            gizmoScaling.Enabled = false;
            gizmoTranslation.Enabled = !gizmoTranslation.Enabled;
            UpdateTransformButtons();
        }

        void btnObjectSelect_Click( object sender, EventArgs e )
        {
            gizmoTranslation.Enabled = false;
            gizmoRotation.Enabled = false;
            UpdateTransformButtons();
        }

        public void dockContainerItem_TabActivated( object sender, EventArgs e )
        {
            editorObject.EditorForm.btnObjectSelect.Click += new EventHandler( btnObjectSelect_Click );
            editorObject.EditorForm.btnObjectMove.Click += new EventHandler( btnObjectMove_Click );
            editorObject.EditorForm.btnObjectRotate.Click += new EventHandler( btnObjectRotate_Click );
            editorObject.EditorForm.btnObjectScale.Click += new EventHandler( btnObjectScale_Click );
            UpdateTransformButtons();
        }

        public void dockContainerItem_TabDeActivated( object sender, EventArgs e )
        {
            editorObject.EditorForm.btnObjectSelect.Click -= new EventHandler( btnObjectSelect_Click );
            editorObject.EditorForm.btnObjectMove.Click -= new EventHandler( btnObjectMove_Click );
            editorObject.EditorForm.btnObjectRotate.Click -= new EventHandler( btnObjectRotate_Click );
            editorObject.EditorForm.btnObjectScale.Click -= new EventHandler( btnObjectScale_Click );

        }


        private void UpdateTransformButtons()
        {
            editorObject.EditorForm.btnObjectSelect.Checked = !gizmoTranslation.Enabled && !gizmoRotation.Enabled;
            editorObject.EditorForm.btnObjectMove.Checked = gizmoTranslation.Enabled;
            editorObject.EditorForm.btnObjectRotate.Checked = gizmoRotation.Enabled;
            editorObject.EditorForm.btnObjectScale.Checked = gizmoScaling.Enabled;
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
            editorObject.Initialize();

            gizmoTranslation = new EditorGizmoTranslation( editorObject.EditorForm );
            gizmoTranslation.Load( this );
            gizmoTranslation.PositionChanged += new EventHandler( gizmoTranslation_PositionChanged );

            gizmoRotation = new EditorGizmoRotation( editorObject.EditorForm );
            gizmoRotation.Load( this );
            gizmoRotation.RotationChanged += new EventHandler( gizmoRotation_RotationChanged );


            gizmoScaling = new EditorGizmoScaling( editorObject.EditorForm );
            gizmoScaling.Load( this );
            gizmoScaling.ScalingChanged += new EventHandler( gizmoScaling_ScalingChanged );
        }

        void gizmoScaling_ScalingChanged( object sender, EventArgs e )
        {
            if ( editorObject.Models.Count > 0 )
            {
                EditorModel model = editorObject.Models[ 0 ];
                model.WorldMatrix = Matrix.CreateScale( gizmoScaling.Scaling )
                    * Matrix.CreateFromQuaternion( gizmoRotation.RotationQuat )
                    * Matrix.CreateTranslation( gizmoTranslation.Position );

            }
        }

        void gizmoRotation_RotationChanged( object sender, EventArgs e )
        {
            if ( editorObject.Models.Count > 0 )
            {
                EditorModel model = editorObject.Models[ 0 ];
                model.WorldMatrix = Matrix.CreateScale( gizmoScaling.Scaling )
                    * Matrix.CreateFromQuaternion( gizmoRotation.RotationQuat )
                    * Matrix.CreateTranslation( gizmoTranslation.Position );

            }
        }

        void gizmoTranslation_PositionChanged( object sender, EventArgs e )
        {
            gizmoRotation.Position = gizmoTranslation.Position;
            gizmoScaling.Position = gizmoTranslation.Position;

            if ( editorObject.Models.Count > 0 )
            {
                EditorModel model = editorObject.Models[ 0 ];
                model.WorldMatrix = Matrix.CreateScale( gizmoScaling.Scaling )
                    * Matrix.CreateFromQuaternion( gizmoRotation.RotationQuat )
                    * Matrix.CreateTranslation( gizmoTranslation.Position );

            }
        }



        protected override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {
            if ( editorObject.dockContainerItem != null && !editorObject.dockContainerItem.Displayed ) return;
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

            if ( gizmoRotation.ActiveHoverPart == EditorGizmoRotation.GizmoPart.None )
                gizmoTranslation.Update( this );
            if ( gizmoTranslation.ActiveHoverPart == EditorGizmoTranslation.GizmoPart.None )
                gizmoRotation.Update( this );

            gizmoScaling.Update( this );
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
            if ( editorObject.dockContainerItem != null && !editorObject.dockContainerItem.Displayed ) return;
            base.Draw( gameTime );

            this.graphics.GraphicsDevice.Clear( Microsoft.Xna.Framework.Graphics.Color.SkyBlue );
            grid.Render();

            GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
            EditorObject.Render();

            lineManager3D.Render();

            gizmoRotation.Render( this );
            gizmoTranslation.Render( this );
            gizmoScaling.Render( this );
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

    }
}

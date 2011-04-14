using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars.Ribbon;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Editor.Transform
{
    /// <summary>
    /// TODO: remove the devexpress shizzle from this class
    /// </summary>
    public class TransformControl
    {
        private IXNAGame game;
        private TransformForm form;

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                form.btnSelect.Enabled = value;
                form.btnMove.Enabled = value;
                form.btnRotate.Enabled = value;
                form.btnScale.Enabled = value;
            }
        }

        EditorGizmoTranslation gizmoTranslation;
        EditorGizmoRotation gizmoRotation;
        EditorGizmoScaling gizmoScaling;

        public Vector3 Position
        {
            get { return gizmoTranslation.Position; }
            set { gizmoTranslation.Position = value; }
        }
        public Quaternion Rotation
        {
            get { return gizmoRotation.RotationQuat; }
            set { gizmoRotation.RotationQuat = value; }
        }
        public Vector3 Scale
        {
            get { return gizmoScaling.Scaling; }
            set { gizmoScaling.Scaling = value; }
        }



        [Obsolete("Userinterface should be independent, use other constructor")]
        public TransformControl( IXNAGame _game, RibbonForm targetForm, RibbonPage targetPage )
        {
            game = _game;
            form = new TransformForm();

            // Add ribbon group
            DevExpressRibbonMerger.MergeRepositoryItems( form, targetForm );
            DevExpressRibbonMerger.MergeBarItems( form, targetForm );
            DevExpressRibbonMerger.MergePage( form.ribbonPage1, targetPage );


            gizmoTranslation = new EditorGizmoTranslation();
            gizmoRotation = new EditorGizmoRotation();
            gizmoScaling = new EditorGizmoScaling();

            form.btnSelect.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnObjectSelect_Click );
            form.btnMove.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnObjectMove_Click );
            form.btnRotate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnObjectRotate_Click );
            form.btnScale.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnObjectScale_Click );
            UpdateTransformButtons();




        }

        public TransformControl(IXNAGame _game)
        {
            game = _game;

            gizmoTranslation = new EditorGizmoTranslation();
            gizmoRotation = new EditorGizmoRotation();
            gizmoScaling = new EditorGizmoScaling();


        }

        public void SetTransformation( Vector3 position, Quaternion rotation, Vector3 scale )
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
        public void SetTransformation( Transformation transform )
        {
            Position = transform.Translation;
            Rotation = transform.Rotation;
            Scale = transform.Scaling;
        }
        public Transformation GetTransformation()
        {
            return new Transformation( Scale, Rotation, Position );
        }

        public Matrix GetTransformationMatrix()
        {
            return Matrix.CreateScale( Scale )
               * Matrix.CreateFromQuaternion( Rotation )
               * Matrix.CreateTranslation( Position );
        }

        public bool IsGizmoTargeted()
        {
            return !( gizmoTranslation.ActiveHoverPart == EditorGizmoTranslation.GizmoPart.None &&
                gizmoRotation.ActiveHoverPart == EditorGizmoRotation.GizmoPart.None &&
                gizmoScaling.ActiveHoverPart == EditorGizmoScaling.GizmoPart.None );
        }



        public void Update()
        {
            if ( !enabled ) return;

            //TODO: this line should cancel the camera update in the Update method in XNAGameControl
            //if ( editorObject.dockContainerItem != null && !editorObject.dockContainerItem.Displayed ) return;

            //if ( selectedModel != null )
            if ( enabled )
            {
                if ( gizmoRotation.ActiveHoverPart == EditorGizmoRotation.GizmoPart.None )
                    gizmoTranslation.Update( game );
                if ( gizmoTranslation.ActiveHoverPart == EditorGizmoTranslation.GizmoPart.None )
                    gizmoRotation.Update( game );

                gizmoScaling.Update( game );
            }

        }

        public void Render()
        {
            if ( !enabled ) return;

            //Note: Force the linemanager to render before the gizmo for correct display
            game.LineManager3D.Render();

            game.GraphicsDevice.Clear( ClearOptions.DepthBuffer, Vector4.Zero, 1, 0 );

            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace;

            gizmoRotation.Render( game );
            gizmoTranslation.Render( game );
            gizmoScaling.Render( game );

            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace;

            gizmoRotation.Render( game );
            gizmoTranslation.Render( game );
            gizmoScaling.Render( game );



        }

        public void Initialize()
        {
            gizmoTranslation.Load( game );
            gizmoTranslation.PositionChanged += new EventHandler( gizmoTranslation_PositionChanged );
            gizmoRotation.Load( game );
            gizmoScaling.Load( game );
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
            gizmoScaling.Enabled = false;
            UpdateTransformButtons();
        }

        private void UpdateTransformButtons()
        {
            if (form == null) return;
            form.btnSelect.Down = !gizmoTranslation.Enabled && !gizmoRotation.Enabled && !gizmoScaling.Enabled;
            form.btnMove.Down = gizmoTranslation.Enabled;
            form.btnRotate.Down = gizmoRotation.Enabled;
            form.btnScale.Down = gizmoScaling.Enabled;
        }

        void gizmoTranslation_PositionChanged( object sender, EventArgs e )
        {
            gizmoRotation.Position = gizmoTranslation.Position;
            gizmoScaling.Position = gizmoTranslation.Position;
        }




    }
}

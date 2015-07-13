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
    public class TransformControl : IXNAObject
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
            }
        }

        EditorGizmoTranslation gizmoTranslation;
        EditorGizmoRotation gizmoRotation;
        EditorGizmoScaling gizmoScaling;

        private GizmoMode mode;
        public GizmoMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                updateGizmoEnables();
            }
        }

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


        public TransformControl()
        {
            gizmoTranslation = new EditorGizmoTranslation();
            gizmoRotation = new EditorGizmoRotation();
            gizmoScaling = new EditorGizmoScaling();


        }

        public void SetTransformation(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
        public void SetTransformation(Transformation transform)
        {
            Position = transform.Translation;
            Rotation = transform.Rotation;
            Scale = transform.Scaling;
        }
        public Transformation GetTransformation()
        {
            return new Transformation(Scale, Rotation, Position);
        }

        public Matrix GetTransformationMatrix()
        {
            return Matrix.CreateScale(Scale)
               * Matrix.CreateFromQuaternion(Rotation)
               * Matrix.CreateTranslation(Position);
        }

        public bool IsGizmoTargeted()
        {
            return !(gizmoTranslation.ActiveHoverPart == EditorGizmoTranslation.GizmoPart.None &&
                gizmoRotation.ActiveHoverPart == EditorGizmoRotation.GizmoPart.None &&
                gizmoScaling.ActiveHoverPart == EditorGizmoScaling.GizmoPart.None);
        }

        public bool IsDragging()
        {
            return gizmoTranslation.ActiveMoveMode != EditorGizmoTranslation.GizmoPart.None
                   || gizmoRotation.ActiveMoveMode != EditorGizmoRotation.GizmoPart.None
                   || gizmoScaling.ActiveMoveMode != EditorGizmoScaling.GizmoPart.None;
        }

        void gizmoTranslation_PositionChanged(object sender, EventArgs e)
        {
            gizmoRotation.Position = gizmoTranslation.Position;
            gizmoScaling.Position = gizmoTranslation.Position;
        }


        public void updateGizmoEnables()
        {
            gizmoTranslation.Enabled = false;
            gizmoRotation.Enabled = false;
            gizmoScaling.Enabled = false;
            switch (Mode)
            {
                case GizmoMode.Translation:
                    gizmoTranslation.Enabled = true;
                    break;
                case GizmoMode.Rotation:
                    gizmoRotation.Enabled = true;
                    break;
                case GizmoMode.Scaling:
                    gizmoScaling.Enabled = true;
                    break;
            }
        }


        public void Update()
        {
            if (!enabled) return;

            //TODO: this line should cancel the camera update in the Update method in XNAGameControl
            //if ( editorObject.dockContainerItem != null && !editorObject.dockContainerItem.Displayed ) return;

            //if ( selectedModel != null )
            if (gizmoRotation.ActiveHoverPart == EditorGizmoRotation.GizmoPart.None)
                gizmoTranslation.Update(game);
            if (gizmoTranslation.ActiveHoverPart == EditorGizmoTranslation.GizmoPart.None)
                gizmoRotation.Update(game);

            gizmoScaling.Update(game);
        }
        public void Render()
        {
            if (!enabled) return;

            //Note: Force the linemanager to render before the gizmo for correct display
            game.LineManager3D.Render();

            game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Vector4.Zero, 1, 0);

            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace;

            gizmoRotation.Render(game);
            gizmoTranslation.Render(game);
            gizmoScaling.Render(game);

            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace;

            gizmoRotation.Render(game);
            gizmoTranslation.Render(game);
            gizmoScaling.Render(game);



        }
        public void Initialize()
        {
            gizmoTranslation.Load(game);
            gizmoTranslation.PositionChanged += new EventHandler(gizmoTranslation_PositionChanged);
            gizmoRotation.Load(game);
            gizmoScaling.Load(game);
        }


        public void Initialize(IXNAGame _game)
        {
            game = _game;
            Initialize();
        }
        public void Render(IXNAGame _game)
        {
            Render();
        }
        public void Update(IXNAGame _game)
        {
            Update();
        }


        public enum GizmoMode
        {
            None = 0,
            Translation,
            Rotation,
            Scaling
        }
    }
}

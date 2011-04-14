using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Collada;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorGizmoTranslation
    {
        /// <summary>
        /// First 3 bits for Center, Axis, Tip or Plane, next 3 for x, y and z
        /// </summary>
        [Flags]
        public enum GizmoPart
        {
            None = 0,
            Center = 1,
            Axis = 2,
            Tip = 3,
            Plane = 4,
            X = 1 << 3,
            Y = 1 << 4,
            Z = 1 << 5,
            AxisX = Axis | X,
            AxisY = Axis | Y,
            AxisZ = Axis | Z,
            PlaneXY = Plane | X | Y,
            PlaneXZ = Plane | X | Z,
            PlaneYZ = Plane | Y | Z,
            TipX = Tip | X,
            TipY = Tip | Y,
            TipZ = Tip | Z
        }
        private class SelectionPart
        {
            public GizmoPart Part;
            public BoundingBox Box;

            public SelectionPart()
            {
                Part = GizmoPart.None;
            }

            public SelectionPart( GizmoPart nPart, BoundingBox nBB )
            {
                Part = nPart;
                Box = nBB;
            }


            public EditorRaycastResult<SelectionPart> Raycast( Ray ray )
            {
                return new EditorRaycastResult<SelectionPart>( ray.Intersects( Box ), this );
            }
        }

        public event EventHandler StartMoveMode;
        public event EventHandler EndMoveMode;

        private bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set
            {
                bool changed = position != value;
                position = value;
                if ( changed ) OnPositionChanged();

            }
        }

        public event EventHandler PositionChanged;

        private GizmoPart activeMoveMode;

        public GizmoPart ActiveMoveMode
        {
            get { return activeMoveMode; }
            set { activeMoveMode = value; }
        }

        private Vector3 activeMoveModeRelativeClickPoint;
        private float activeMoveModeScale;

        private Dictionary<GizmoPart, Mesh> meshesDictionary = new Dictionary<GizmoPart, Mesh>();
        private List<Mesh> meshes = new List<Mesh>();

        private GizmoPart activeHoverPart = GizmoPart.None;

        public GizmoPart ActiveHoverPart
        {
            get { return activeHoverPart; }
            set { activeHoverPart = value; }
        }



        private List<SelectionPart> selectionParts = new List<SelectionPart>();


        private CustomCamera gizmoCamera;

        private float maxDistFromCameraFactor = 20f;

        public EditorGizmoTranslation()
        {
            position = Vector3.Zero;
        }

        [Obsolete( "Editor not needed anymore" )]
        public EditorGizmoTranslation( WizardsEditor nEditor )
        {
            position = Vector3.Zero;
        }

        public void Load( IXNAGame game )
        {
            gizmoCamera = new CustomCamera( game );
            ColladaModel model;
            using ( System.IO.Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Editor.FromEditorProject.Transform.Files.GizmoTranslation001.DAE",
                "GizmoTranslation001.DAE" ) )
            {
                model = ColladaModel.FromStream( strm );
            }
            //= ColladaModel.FromFile( (GameFile)editor.Files.GizmoColladaModel );

            Dictionary<string, GizmoPart> partNames = new Dictionary<string, GizmoPart>();
            partNames.Add( "axisX-mesh", GizmoPart.AxisX );
            partNames.Add( "axisY-mesh", GizmoPart.AxisY );
            partNames.Add( "axisZ-mesh", GizmoPart.AxisZ );
            partNames.Add( "center-mesh", GizmoPart.Center );
            partNames.Add( "planeXY-mesh", GizmoPart.PlaneXY );
            partNames.Add( "planeXZ-mesh", GizmoPart.PlaneXZ );
            partNames.Add( "planeYZ-mesh", GizmoPart.PlaneYZ );
            partNames.Add( "tipX-mesh", GizmoPart.TipX );
            partNames.Add( "tipY-mesh", GizmoPart.TipY );
            partNames.Add( "tipZ-mesh", GizmoPart.TipZ );
            for ( int i = 0; i < model.Scene.Nodes.Count; i++ )
            {
                LoadColladaGizmoParts( game, model.Scene.Nodes[ i ], partNames );
            }


            BoundingBox bb;
            bb = new BoundingBox( new Vector3( 0, -0.05f, -0.05f ), new Vector3( 1.2f, 0.05f, 0.05f ) );
            selectionParts.Add( new SelectionPart( GizmoPart.AxisX, bb ) );
            bb = new BoundingBox( new Vector3( -0.05f, 0, -0.05f ), new Vector3( 0.05f, 1.2f, 0.05f ) );
            selectionParts.Add( new SelectionPart( GizmoPart.AxisY, bb ) );
            bb = new BoundingBox( new Vector3( -0.05f, -0.05f, 0 ), new Vector3( 0.05f, 0.05f, 1.2f ) );
            selectionParts.Add( new SelectionPart( GizmoPart.AxisZ, bb ) );


            bb = new BoundingBox( new Vector3( 0.05f, 0.05f, -0.025f ), new Vector3( 0.45f, 0.45f, 0.025f ) );
            selectionParts.Add( new SelectionPart( GizmoPart.PlaneXY, bb ) );
            bb = new BoundingBox( new Vector3( 0.05f, -0.025f, 0.05f ), new Vector3( 0.45f, 0.025f, 0.45f ) );
            selectionParts.Add( new SelectionPart( GizmoPart.PlaneXZ, bb ) );
            bb = new BoundingBox( new Vector3( -0.025f, 0.05f, 0.05f ), new Vector3( 0.025f, 0.45f, 0.45f ) );
            selectionParts.Add( new SelectionPart( GizmoPart.PlaneYZ, bb ) );

        }

        private void LoadColladaGizmoParts( IXNAGame game, ColladaSceneNodeBase node, Dictionary<string, GizmoPart> partNames )
        {
            for ( int i = 0; i < node.Nodes.Count; i++ )
            {
                LoadColladaGizmoParts( game, node.Nodes[ i ], partNames );
            }
            if ( node.Instance_Geometry == null ) return;

            GizmoPart partType = GizmoPart.None;

            if ( partNames.TryGetValue( node.Instance_Geometry.Name, out partType ) )
            {
                // Load this mesh, assume having only one primitiveList
                Mesh gizmoPartMesh = Mesh.FromColladaMeshPrimitiveList( game, node.Instance_Geometry.Parts[ 0 ] );
                gizmoPartMesh.LocalMatrix = node.GetFullMatrix();
                meshesDictionary.Add( partType, gizmoPartMesh );
                meshes.Add( gizmoPartMesh );


            }
        }

        public void Render( IXNAGame game )
        {
            if ( !enabled ) return;
            /*EditorCamera cam = (EditorCamera)game.Camera;
            if ( !game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) && false )
            {
                if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                {
                    cam.Position = new Vector3( -0.3f, 0, 0 );
                }
                if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Up ) )
                {
                    cam.Position = cam.Position + new Vector3( 0, 0, -0.02f );
                }
                if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Down ) )
                {
                    cam.Position = cam.Position + new Vector3( 0, 0, 0.02f );
                }
                cam.Orientation = cam.CreateLookAt( cam.Position + new Vector3( 0, 0, -1 ) );
            }

            Vector3 pos1 = new Vector3( 0, 0, -12 );
            Vector3 pos2 = new Vector3( 0, 0, -3 );

            Vector3 pos2D = game.GraphicsDevice.Viewport.Project( position, cam.Projection, cam.View, Matrix.Identity );
            pos2D.Z = 0.98f;
            Vector3 pos = game.GraphicsDevice.Viewport.Unproject( pos2D, cam.Projection, cam.View, Matrix.Identity );
            renderedPos = pos;

            Matrix worldMatrix = Matrix.CreateScale( 1 ) * Matrix.CreateTranslation( pos );
            for ( int i = 0; i < meshes.Count; i++ )
            {
                //meshes[ i ].RenderDirect( game, worldMatrix );
            }

            //worldMatrix = Matrix.CreateScale( 1 * ( -pos2.Z - ( -cam.Position.Z ) ) / ( -pos1.Z - ( -cam.Position.Z ) ) ) * Matrix.CreateTranslation( pos2 );
            position = pos2;*/

            //game.GraphicsDevice.Clear( ClearOptions.DepthBuffer, Vector4.Zero, 1, 0 );

            Matrix worldMatrix;
            worldMatrix = Matrix.CreateScale( CalculateGizmoScale( game ) ) * Matrix.CreateTranslation( position );
            for ( int i = 0; i < meshes.Count; i++ )
            {
                meshes[ i ].RenderDirect( game, worldMatrix );
            }
            for ( int i = 0; i < selectionParts.Count; i++ )
            {
                //game.LineManager3D.AddBox( selectionParts[ i ].Box, Color.Red );
            }

        }

        public void Update( IXNAGame game )
        {
            if ( !enabled ) return;


            if ( game.Mouse.CursorEnabled )
            {
                if ( game.Mouse.LeftMousePressed && !game.Mouse.LeftMouseJustPressed )
                {
                    DoMoveMode( game, activeMoveMode );
                }
                else if ( game.Mouse.LeftMouseJustReleased )
                {
                    bool flag = activeMoveMode != GizmoPart.None;
                    activeMoveMode = GizmoPart.None;
                    if ( flag )
                    {
                        if ( EndMoveMode != null ) EndMoveMode( this, null );
                    }

                }
                else
                {
                    //editor.Text = game.Mouse.CursorPosition.X.ToString() + " " + game.Mouse.CursorPosition.Y.ToString();
                    Ray ray = game.GetWereldViewRay( new Vector2( game.Mouse.CursorPosition.X, game.Mouse.CursorPosition.Y ) );

                    // transform the ray to local space
                    ray.Position -= position;
                    // Deapply the scaling
                    ray.Position /= CalculateGizmoScale( game );

                    EditorRaycastResult<SelectionPart> closest = new EditorRaycastResult<SelectionPart>();
                    for ( int i = 0; i < selectionParts.Count; i++ )
                    {
                        EditorRaycastResult<SelectionPart> result = selectionParts[ i ].Raycast( ray );
                        if ( result.CompareTo( closest ) == -1 ) closest = result;
                    }
                    if ( closest.IsHit == true )
                    {
                        //Point on something
                        if ( game.Mouse.LeftMouseJustPressed )
                        {
                            // The clicked point must be transformed to world space
                            OnPartClicked( game, closest.Item.Part, ( ray.Position + ray.Direction * closest.Distance ) * CalculateGizmoScale( game ) + position );
                        }
                        else
                        {
                            OnPartHover( game, closest.Item.Part );
                        }
                    }
                    else
                    {
                        OnPartHover( game, GizmoPart.None );
                    }
                }
            }
        }

        private float CalculateMaxDistFromCamera( IXNAGame game )
        {
            // make dependent on the camera height
            Vector3 camPos = game.Camera.ViewInverse.Translation;
            return maxDistFromCameraFactor * camPos.Y;
        }

        private float CalculateDistFromCamera( IXNAGame game )
        {
            return Vector3.Distance( position, game.Camera.ViewInverse.Translation );
        }

        private float CalculateGizmoScale( IXNAGame game )
        {
            if ( activeMoveMode != GizmoPart.None ) return activeMoveModeScale;
            return CalculateDistFromCamera( game ) * 0.14f;
        }

        private void OnPartClicked( IXNAGame game, GizmoPart type, Vector3 clickedPoint )
        {
            if ( ( type & GizmoPart.Plane ) != GizmoPart.None )
            {

                Plane p = GetXNAPlane( type );
                Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );

                float? dist = ray.Intersects( p );
                if ( !dist.HasValue ) throw new Exception();



                activeMoveModeRelativeClickPoint = ray.Position + ray.Direction * dist.Value - position;
            }
            else if ( ( type & GizmoPart.Axis ) != GizmoPart.None )
            {
                Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );
                EditorRaycastResult<GizmoPart> hit = RaycastAxisPlanes( ray, type );
                if ( !hit.IsHit ) throw new Exception();
                activeMoveModeRelativeClickPoint = ray.Position + ray.Direction * hit.Distance - position;
            }
            // must be called before setting the activeMoveMode (see the function CalculateGizmoScale)
            activeMoveModeScale = CalculateGizmoScale( game );

            activeMoveMode = type;

            if ( StartMoveMode != null ) StartMoveMode( this, null );


        }
        private void DoMoveMode( IXNAGame game, GizmoPart moveMode )
        {
            if ( moveMode == GizmoPart.PlaneXY || moveMode == GizmoPart.PlaneXZ || moveMode == GizmoPart.PlaneYZ ) DoMovePlane( game, moveMode );
            if ( moveMode == GizmoPart.AxisX || moveMode == GizmoPart.AxisY || moveMode == GizmoPart.AxisZ ) DoMoveAxis( game, moveMode );


        }
        private void DoMoveAxis( IXNAGame game, GizmoPart type )
        {
            Vector3 oldPos = position;

            //Just calculate the closest intersection point with one of the three planes and project it onto the selected axis
            // TODO: i have a strong feeling its better to use mathextra.functions.lineLineintersect for calculating targetPoint

            Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );

            EditorRaycastResult<GizmoPart> hit = RaycastAxisPlanes( ray, type );

            if ( hit.IsHit == false ) return;

            Vector3 targetPoint = ray.Position + ray.Direction * hit.Distance;

            game.LineManager3D.AddLine( targetPoint, targetPoint + Vector3.UnitY * 3, Color.Gray );

            switch ( type )
            {
                case GizmoPart.AxisX:
                    position.X = targetPoint.X - activeMoveModeRelativeClickPoint.X;
                    break;
                case GizmoPart.AxisY:
                    position.Y = targetPoint.Y - activeMoveModeRelativeClickPoint.Y;
                    break;
                case GizmoPart.AxisZ:
                    position.Z = targetPoint.Z - activeMoveModeRelativeClickPoint.Z;
                    break;
            }

            if ( CalculateDistFromCamera( game ) > CalculateMaxDistFromCamera( game ) )
                position = oldPos;
            else
                OnPositionChanged();

        }
        private EditorRaycastResult<GizmoPart> RaycastAxisPlanes( Ray ray, GizmoPart axis )
        {
            GizmoPart plane1 = GizmoPart.None;
            GizmoPart plane2 = GizmoPart.None;

            switch ( axis )
            {
                case GizmoPart.AxisX:
                    plane1 = GizmoPart.PlaneXY;
                    plane2 = GizmoPart.PlaneXZ;
                    break;
                case GizmoPart.AxisY:
                    plane1 = GizmoPart.PlaneXY;
                    plane2 = GizmoPart.PlaneYZ;
                    break;
                case GizmoPart.AxisZ:
                    plane1 = GizmoPart.PlaneYZ;
                    plane2 = GizmoPart.PlaneXZ;
                    break;
            }

            EditorRaycastResult<GizmoPart> closest = new EditorRaycastResult<GizmoPart>();
            EditorRaycastResult<GizmoPart> temp;
            temp = new EditorRaycastResult<GizmoPart>( ray.Intersects( GetXNAPlane( plane1 ) ), plane1 );
            if ( temp.IsCloser( closest ) ) closest = temp;
            temp = new EditorRaycastResult<GizmoPart>( ray.Intersects( GetXNAPlane( plane2 ) ), plane2 );
            if ( temp.IsCloser( closest ) ) closest = temp;

            return closest;
        }

        private void DoMovePlane( IXNAGame game, GizmoPart type )
        {
            Vector3 oldPos = position;

            Plane p = GetXNAPlane( type );

            Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );

            float? dist = ray.Intersects( p );
            if ( dist.HasValue )
            {
                position = ray.Direction * dist.Value + ray.Position - activeMoveModeRelativeClickPoint;
            }

            if ( CalculateDistFromCamera( game ) > CalculateMaxDistFromCamera( game ) )
                position = oldPos;
            else
                OnPositionChanged();
        }
        private Plane GetXNAPlane( GizmoPart part )
        {
            Plane p = new Plane();

            switch ( part )
            {
                case GizmoPart.PlaneXY:
                    p = new Plane( position, position + Vector3.UnitX, position + Vector3.UnitY );
                    break;
                case GizmoPart.PlaneXZ:
                    p = new Plane( position, position + Vector3.UnitX, position + Vector3.UnitZ );
                    break;
                case GizmoPart.PlaneYZ:
                    p = new Plane( position, position + Vector3.UnitY, position + Vector3.UnitZ );
                    break;
            }

            return p;
        }

        private void OnPartHover( IXNAGame game, GizmoPart type )
        {
            if ( type != activeHoverPart )
            {
                switch ( activeHoverPart )
                {
                    case GizmoPart.AxisX:
                        meshesDictionary[ GizmoPart.AxisX ].Shader.SetParameter( "diffuseColor", Color.Red );
                        meshesDictionary[ GizmoPart.TipX ].Shader.SetParameter( "diffuseColor", Color.Red );
                        break;
                    case GizmoPart.AxisY:
                        meshesDictionary[ GizmoPart.AxisY ].Shader.SetParameter( "diffuseColor", Color.Green );
                        meshesDictionary[ GizmoPart.TipY ].Shader.SetParameter( "diffuseColor", Color.Green );
                        break;
                    case GizmoPart.AxisZ:
                        meshesDictionary[ GizmoPart.AxisZ ].Shader.SetParameter( "diffuseColor", Color.Blue );
                        meshesDictionary[ GizmoPart.TipZ ].Shader.SetParameter( "diffuseColor", Color.Blue );
                        break;

                    case GizmoPart.PlaneXY:
                        meshesDictionary[ GizmoPart.PlaneXY ].Shader.SetParameter( "diffuseColor", Color.Gold );
                        break;
                    case GizmoPart.PlaneXZ:
                        meshesDictionary[ GizmoPart.PlaneXZ ].Shader.SetParameter( "diffuseColor", Color.Gold );
                        break;
                    case GizmoPart.PlaneYZ:
                        meshesDictionary[ GizmoPart.PlaneYZ ].Shader.SetParameter( "diffuseColor", Color.Gold );
                        break;
                }
                activeHoverPart = GizmoPart.None;
            }
            switch ( type )
            {
                case GizmoPart.AxisX:
                    meshesDictionary[ GizmoPart.AxisX ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    meshesDictionary[ GizmoPart.TipX ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
                case GizmoPart.AxisY:
                    meshesDictionary[ GizmoPart.AxisY ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    meshesDictionary[ GizmoPart.TipY ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
                case GizmoPart.AxisZ:
                    meshesDictionary[ GizmoPart.AxisZ ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    meshesDictionary[ GizmoPart.TipZ ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;

                case GizmoPart.PlaneXY:
                    meshesDictionary[ GizmoPart.PlaneXY ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
                case GizmoPart.PlaneXZ:
                    meshesDictionary[ GizmoPart.PlaneXZ ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
                case GizmoPart.PlaneYZ:
                    meshesDictionary[ GizmoPart.PlaneYZ ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
            }

            activeHoverPart = type;

        }

        private void OnPositionChanged()
        {
            if ( PositionChanged != null ) PositionChanged( this, null );
        }



    }
}

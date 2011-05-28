using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Collada;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core;

namespace MHGameWork.TheWizards.Editor.Transform
{
    public class EditorGizmoRotation
    {
        /// <summary>
        /// First 3 bits for Center, Axis, Tip or Plane, next 3 for x, y and z
        /// </summary>
        [Flags]
        public enum GizmoPart
        {
            None = 0,
            TorusX = 1,
            TorusY = 2,
            TorusZ = 3,
            PlaneXY = 4,
            PlaneYZ = 5,
            PlaneXZ = 6
        }
        private class SelectionPart
        {
            public GizmoPart Part;
            public Torus Torus;

            public SelectionPart()
            {
                Part = GizmoPart.None;
            }

            public SelectionPart( GizmoPart nPart, Torus nTorus )
            {
                Part = nPart;
                Torus = nTorus;
            }


            public EditorRaycastResult<SelectionPart> Raycast( Ray ray )
            {
                return new EditorRaycastResult<SelectionPart>( Torus.Intersects( ray ), this );
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
            set { position = value; }
        }

        private Quaternion rotationQuat = Quaternion.Identity;

        public Quaternion RotationQuat
        {
            get { return rotationQuat; }
            set
            {
                bool changed = rotationQuat != value;
                rotationQuat = value;
                if ( changed ) OnRotationChanged();

            }
        }


        public event EventHandler RotationChanged;

        private GizmoPart activeMoveMode;

        public GizmoPart ActiveMoveMode
        {
            get { return activeMoveMode; }
            set { activeMoveMode = value; }
        }


        private Vector3 activeMoveModeClickPoint;
        private Ray activeMoveModeTangentRay;
        private Quaternion activeMoveModeStartRotation;
        private Vector3 activeMoveModeRotatedPoint;

        private Dictionary<GizmoPart, Mesh> meshesDictionary = new Dictionary<GizmoPart, Mesh>();
        private List<Mesh> meshes = new List<Mesh>();
        private GizmoPart activeHoverPart = GizmoPart.None;

        public GizmoPart ActiveHoverPart
        {
            get { return activeHoverPart; }
            set { activeHoverPart = value; }
        }


        private List<SelectionPart> selectionParts = new List<SelectionPart>();
        private Dictionary<GizmoPart, SelectionPart> selectionPartsDictionary = new Dictionary<GizmoPart, SelectionPart>();





      
        public EditorGizmoRotation()
        {
            position = Vector3.Zero;
        }

        public void Load( IXNAGame game )
        {
            ColladaModel model;
            using ( System.IO.Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Editor.Transform.Files.GizmoRotation001.DAE",
                "GizmoRotation001.DAE" ) )
            {
                model = ColladaModel.FromStream( strm );
            }
            //= ColladaModel.FromFile( (GameFile)editor.Files.GizmoColladaModel );
            //ColladaModel model = ColladaModel.FromFile( (GameFile)editor.Files.GizmoRotationColladaModel );

            Dictionary<string, GizmoPart> partNames = new Dictionary<string, GizmoPart>();
            partNames.Add( "torusX-mesh", GizmoPart.TorusX );
            partNames.Add( "torusY-mesh", GizmoPart.TorusY );
            partNames.Add( "torusZ-mesh", GizmoPart.TorusZ );

            for ( int i = 0; i < model.Scene.Nodes.Count; i++ )
            {
                LoadColladaGizmoParts( game, model.Scene.Nodes[ i ], partNames );
            }


            Torus t;
            t = new Torus( Vector3.Zero, Quaternion.Identity, 0.99f, 0.08f );
            t.Orientation = Quaternion.Identity;
            selectionParts.Add( new SelectionPart( GizmoPart.TorusZ, t ) );
            selectionPartsDictionary.Add( GizmoPart.TorusZ, selectionParts[ selectionParts.Count - 1 ] );

            t = new Torus( Vector3.Zero, Quaternion.Identity, 0.99f, 0.08f );
            t.Orientation = Quaternion.CreateFromYawPitchRoll( MathHelper.PiOver2, 0, 0 );
            selectionParts.Add( new SelectionPart( GizmoPart.TorusX, t ) );
            selectionPartsDictionary.Add( GizmoPart.TorusX, selectionParts[ selectionParts.Count - 1 ] );

            t = new Torus( Vector3.Zero, Quaternion.Identity, 0.99f, 0.08f );
            t.Orientation = Quaternion.CreateFromYawPitchRoll( 0, -MathHelper.PiOver2, 0 );
            selectionParts.Add( new SelectionPart( GizmoPart.TorusY, t ) );
            selectionPartsDictionary.Add( GizmoPart.TorusY, selectionParts[ selectionParts.Count - 1 ] );


        }

        private void LoadColladaGizmoParts( IXNAGame game, ColladaSceneNodeBase node, Dictionary<string, GizmoPart> partNames )
        {
            for ( int i = 0; i < node.Nodes.Count; i++ )
            {
                LoadColladaGizmoParts( game, node.Nodes[ i ], partNames );
            }
            if ( node.Instance_Geometry != null )
            {
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
        }

        public void Render( IXNAGame game )
        {
            if ( !enabled ) return;
            /*game.LineManager3D.AddLine( new Vector3( 5, -10, 0 ), new Vector3( 5, 10, 0 ), Color.Red );
            if ( game.Mouse.CursorEnabled )
            {
                
                Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );

                Vector3 pa, pb; float mua, mub;
                MathExtra.Functions.LineLineIntersect( new Vector3( 5, -10, 0 ), new Vector3( 5, 10, 0 ), ray.Position, ray.Position + ray.Direction, out pa, out  pb, out  mua, out  mub );
                game.LineManager3D.AddLine( pa + new Vector3( 0, 0, 0 ), pa + new Vector3( 0, 0, 3 ), Color.Green );
            }*/

            //game.GraphicsDevice.Clear( ClearOptions.DepthBuffer, Vector4.Zero, 1, 0 );

            float scale = CalculateGizmoScale( game );

            Matrix worldMatrix;
            worldMatrix = Matrix.CreateScale( scale ) * Matrix.CreateTranslation( position );
            for ( int i = 0; i < meshes.Count; i++ )
            {
                meshes[ i ].RenderDirect( game, worldMatrix );
            }

            float tangentLength = 3 * scale;
            game.LineManager3D.AddLine( activeMoveModeTangentRay.Position + activeMoveModeTangentRay.Direction * tangentLength
                , activeMoveModeTangentRay.Position + activeMoveModeTangentRay.Direction * -tangentLength, Color.Red );
            game.LineManager3D.AddLine( activeMoveModeRotatedPoint + new Vector3( 0, 0, 0 ), activeMoveModeRotatedPoint + new Vector3( 0, scale * 1, 0 ), Color.Green );

        }

        public void Update( IXNAGame game )
        {
            if ( !enabled ) return;
            //if ( game.Mouse.LeftMouseJustPressed ) throw new Exception();
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
                            OnPartHover(closest.Item.Part );
                        }
                    }
                    else
                    {
                        OnPartHover(GizmoPart.None );
                    }
                }
            }
        }


        private float CalculateDistFromCamera( IXNAGame game )
        {
            return Vector3.Distance( position, game.Camera.ViewInverse.Translation );
        }

        private float CalculateGizmoScale( IXNAGame game )
        {
            //if ( activeMoveMode != GizmoPart.None ) return activeMoveModeScale;
            return CalculateDistFromCamera( game ) * 0.14f;
        }

        private void OnPartClicked( IXNAGame game, GizmoPart type, Vector3 clickedPoint )
        {
            Vector3 worldClickedPoint = clickedPoint;
            Torus torus = selectionPartsDictionary[ type ].Torus;

            // EDIT: first transform to torus' local space, that is, also translate
            clickedPoint -= position;
            // Also detransform scaling
            clickedPoint /= CalculateGizmoScale( game );
            // EDIT: this should be Zero 
            //clickedPoint -= torus.Center;
            // Transform the clickedpoint to a space where the torus is in the xy plane
            clickedPoint = Vector3.Transform( clickedPoint, Quaternion.Inverse( torus.Orientation ) );
            // Project on xy plane.
            clickedPoint.Z = 0;
            // Put on the center of the torus' ring
            clickedPoint.Normalize();
            clickedPoint *= torus.Radius1;

            // Calculate the tangent to the circle through the center of the torus' ring, through the projected clickedpoint
            // x = cos t
            // y = sin t
            // x' = -y
            // y' = x
            Vector3 tangent = new Vector3( -clickedPoint.Y, clickedPoint.X, 0 );
            tangent.Normalize();

            // Now transform to world space
            tangent = Vector3.Transform( tangent, torus.Orientation );
            clickedPoint = Vector3.Transform( clickedPoint, torus.Orientation );


            // Now scale the point according to active scaling
            clickedPoint *= CalculateGizmoScale( game );

            clickedPoint += position;

            activeMoveModeClickPoint = clickedPoint;
            activeMoveModeTangentRay = new Ray( clickedPoint, tangent );
            activeMoveModeStartRotation = rotationQuat;

            activeMoveMode = type;

            /*// Little trick, do one update, and reset the rotation, this way no start artifacts
            Quaternion q = rotationQuat;
            DoRotateAxis( game, type );
            rotationQuat = q;
            activeMoveModeStartRotation = q;*/

            Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );

            Vector3 pa, pb; float mua, mub;
            MathExtra.Functions.LineLineIntersect( activeMoveModeTangentRay.Position, activeMoveModeTangentRay.Position + activeMoveModeTangentRay.Direction
                , ray.Position, ray.Position + ray.Direction, out pa, out  pb, out  mua, out  mub );
            activeMoveModeClickPoint = pa;


            if ( StartMoveMode != null ) StartMoveMode( this, null );


        }
        private void DoMoveMode( IXNAGame game, GizmoPart moveMode )
        {
            if ( moveMode == GizmoPart.None ) return;
            DoRotateAxis( game, moveMode );
            /*if ( moveMode == GizmoPart.PlaneXY || moveMode == GizmoPart.PlaneXZ || moveMode == GizmoPart.PlaneYZ ) DoMovePlane( game, moveMode );
            if ( moveMode == GizmoPart.AxisX || moveMode == GizmoPart.AxisY || moveMode == GizmoPart.AxisZ ) DoMoveAxis( game, moveMode );*/

        }
        private void DoRotateAxis( IXNAGame game, GizmoPart moveMode )
        {


            Ray ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );

            Vector3 pa, pb; float mua, mub;
            MathExtra.Functions.LineLineIntersect( activeMoveModeTangentRay.Position, activeMoveModeTangentRay.Position + activeMoveModeTangentRay.Direction
                , ray.Position, ray.Position + ray.Direction, out pa, out  pb, out  mua, out  mub );

            activeMoveModeRotatedPoint = pa;


            float dist = Vector3.Dot( pa - activeMoveModeClickPoint, activeMoveModeTangentRay.Direction );
            //Since this factor depends on the scaling, unscale it
            dist /= CalculateGizmoScale( game );

            float factor = 1;
            // Optionally apply a factor
            dist *= factor;

            Quaternion diff = Quaternion.Identity;
            switch ( moveMode )
            {
                case GizmoPart.TorusX:
                    diff = Quaternion.CreateFromAxisAngle( Vector3.UnitX, dist );
                    break;
                case GizmoPart.TorusY:
                    diff = Quaternion.CreateFromAxisAngle( Vector3.UnitY, dist );
                    // Math error solved
                    /*// Little tweak: this is because of the tangent not always being clockwise or counterclockwise (stupid math error)
                    dist = -dist;*/
                    break;
                case GizmoPart.TorusZ:
                    diff = Quaternion.CreateFromAxisAngle( Vector3.UnitZ, dist );
                    break;
            }



            RotationQuat = diff * activeMoveModeStartRotation;



        }


      
        private void OnPartHover(GizmoPart type )
        {
            if ( type != activeHoverPart )
            {
                switch ( activeHoverPart )
                {
                    case GizmoPart.TorusX:
                        meshesDictionary[ GizmoPart.TorusX ].Shader.SetParameter( "diffuseColor", Color.Red );
                        break;
                    case GizmoPart.TorusY:
                        meshesDictionary[ GizmoPart.TorusY ].Shader.SetParameter( "diffuseColor", Color.Green );
                        break;
                    case GizmoPart.TorusZ:
                        meshesDictionary[ GizmoPart.TorusZ ].Shader.SetParameter( "diffuseColor", Color.Blue );
                        break;
                }
                activeHoverPart = GizmoPart.None;
            }
            switch ( type )
            {
                case GizmoPart.TorusX:
                    meshesDictionary[ GizmoPart.TorusX ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
                case GizmoPart.TorusY:
                    meshesDictionary[ GizmoPart.TorusY ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;
                case GizmoPart.TorusZ:
                    meshesDictionary[ GizmoPart.TorusZ ].Shader.SetParameter( "diffuseColor", Color.Yellow );
                    break;

            
            }

            activeHoverPart = type;

        }

        private void OnRotationChanged()
        {
            if ( RotationChanged != null ) RotationChanged( this, null );
        }



    }
}

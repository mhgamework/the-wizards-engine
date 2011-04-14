using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class EditorMoveEntityTool : IEditorTool
    {
        private TWEditor editor;

        private List<WereldEntity> selectedEntities = new List<WereldEntity>();
        private BoundingBox selectionBounding;
        private Vector3 selectionCenter;


        private EditorGizmoOud gizmo;

        private EditorGizmoOud.Axes selectedAxes;

        private Vector3 gizmoClickedPoint;



        public EditorMoveEntityTool( TWEditor nEditor )
        {
            editor = nEditor;
            gizmo = new EditorGizmoOud( editor.Game );
            gizmo.Radius = 1;

        }


        public void Render()
        {

            if ( selectedEntities.Count > 0 )
            {
                for ( int i = 0; i < selectedEntities.Count; i++ )
                {
                    Game.LineManager3D.AddBox( selectedEntities[ i ].BoundingBox, Color.LightBlue );

                }

                Game.LineManager3D.AddBox( selectionBounding, Color.Blue );


                Vector3 center = selectionCenter;

                gizmo.Position = center;

                gizmo.Render();

            }

        }

        public void Update()
        {
            if ( Game.Mouse.CursorEnabled )
            {
                if ( selectedAxes == EditorGizmoOud.Axes.None )
                {
                    if ( Game.Mouse.LeftMouseJustPressed )
                    {
                        DoClick();
                    }
                }
                else
                {
                    Game.LineManager3D.AddLine( gizmoClickedPoint, gizmoClickedPoint + new Vector3( 0, 1, 0 ), Color.Black );

                    if ( selectedAxes == EditorGizmoOud.Axes.X
                        || selectedAxes == EditorGizmoOud.Axes.Y
                        || selectedAxes == EditorGizmoOud.Axes.Z )
                    {

                        // find a plane that is parallel to the near plane and contains the gizmo's xAxis
                        BoundingFrustum frustum = new BoundingFrustum( editor.WereldViewCamera.ViewProjection );
                        Plane p = frustum.Near;
                        p = MathFunctions.PlaneThroughPoint( gizmoClickedPoint, p );
                        p.Normalize();



                        // Find the point pointed on this plane
                        Ray ray = editor.GetWereldViewRay( editor.GetWereldViewCursorPos() );
                        float? dist = ray.Intersects( p );

                        Vector3 pointClicked = ray.Position + ray.Direction * dist.Value;




                        Vector3 move = Vector3.Zero;

                        switch ( selectedAxes )
                        {
                            case EditorGizmoOud.Axes.X:
                                // Now move the axis so that the point on the axis is right below the pointed point.

                                // We just make the x-coords equal.
                                // X-axis is now just vector3.right but can later be transformed due to rotation
                                move.X = pointClicked.X - gizmoClickedPoint.X;

                                break;
                            case EditorGizmoOud.Axes.Y:
                                move.Y = pointClicked.Y - gizmoClickedPoint.Y;

                                break;

                            case EditorGizmoOud.Axes.Z:
                                move.Z = pointClicked.Z - gizmoClickedPoint.Z;

                                break;

                        }

                        for ( int i = 0; i < selectedEntities.Count; i++ )
                        {
                            selectedEntities[ i ].Move( move );
                        }
                        gizmoClickedPoint += move;
                    }
                    else
                    {
                        Vector3 axis1 = Vector3.Zero;
                        Vector3 axis2 = Vector3.Zero;
                        //Get the 2 axes
                        switch ( selectedAxes )
                        {
                            case EditorGizmoOud.Axes.XY:
                                axis1 = gizmo.Position + Vector3.UnitX;
                                axis2 = gizmo.Position + Vector3.UnitY;
                                break;
                            case EditorGizmoOud.Axes.YZ:
                                axis1 = gizmo.Position + Vector3.UnitY;
                                axis2 = gizmo.Position + Vector3.UnitZ;
                                break;
                            case EditorGizmoOud.Axes.XZ:
                                axis1 = gizmo.Position + Vector3.UnitX;
                                axis2 = gizmo.Position + Vector3.UnitZ;
                                break;
                            default:
                                throw new Exception( "Impossible!" );
                        }

                        // Get the plane containing the 2 axes
                        Plane p = new Plane( gizmo.Position, axis1, axis2 );

                        //Move the plane so it goes through the clicked point
                        p = MathFunctions.PlaneThroughPoint( gizmoClickedPoint, p );

                        Ray ray = editor.GetWereldViewRay( editor.GetWereldViewCursorPos() );

                        float? dist = ray.Intersects( p );
                        if ( dist.HasValue )
                        {
                            Vector3 target = ray.Position + ray.Direction * dist.Value;
                            Vector3 displacement = target - gizmoClickedPoint;

                            for ( int i = 0; i < selectedEntities.Count; i++ )
                            {
                                selectedEntities[ i ].Move( displacement );
                            }
                            gizmoClickedPoint = target;
                        }
                    }
                    UpdateSelectionBounding();
                }
            }

            if ( !Game.Mouse.LeftMousePressed ) selectedAxes = EditorGizmoOud.Axes.None;

        }



        private void DoClick()
        {
            if ( selectedEntities.Count > 0 )
            {
                Ray ray = editor.GetWereldViewRay( editor.GetWereldViewCursorPos() );
                EditorRaycastResult<EditorGizmoOud.Axes>[] axes = gizmo.Raycast( ray );
                Array.Sort( axes );
                if ( axes[ 0 ].IsHit )
                {

                    selectedAxes = axes[ 0 ].Item;
                    gizmoClickedPoint = ray.Position + ray.Direction * axes[ 0 ].Distance;

                    switch ( selectedAxes )
                    {
                        case EditorGizmoOud.Axes.X:
                            //selectedEntity.Move( new Vector3( 1, 0, 0 ) );
                            break;
                    }

                    return;
                }
            }




            DoSelect();
            return;
        }

        private void UpdateSelectionBounding()
        {
            if ( selectedEntities.Count == 0 ) return;
            BoundingBox bounding = selectedEntities[ 0 ].BoundingBox;
            for ( int i = 1; i < selectedEntities.Count; i++ )
            {
                bounding = BoundingBox.CreateMerged( bounding, selectedEntities[ i ].BoundingBox );
            }
            selectionBounding = bounding;
            selectionCenter = selectionBounding.Min + selectionBounding.Max;
            selectionCenter *= 0.5f;
        }

        private void DoSelect()
        {
            if ( !Game.Mouse.CursorEnabled ) return;
            if ( Game.Mouse.LeftMouseJustPressed )
            {
                if ( !Game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftControl ) )
                    selectedEntities.Clear();
                List<EditorRaycastResult<WereldEntity>> hits = editor.RaycastWereldEntities( editor.GetWereldViewCursorPos() );
                if ( hits.Count > 0 )
                {
                    hits.Sort();
                    selectedEntities.Add( hits[ 0 ].Item );
                }
                UpdateSelectionBounding();
            }
        }

        public void OnActivate()
        {
        }

        public void OnDeactivate()
        {
        }

        public XNAGame Game { get { return editor.Game; } }


        public static void TestTool()
        {

            TestXNAGame game = new TestXNAGame( "EditorMoveEntityTool.TestTool" );

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

                    EditorMoveEntityTool tool = new EditorMoveEntityTool( editor );

                    WereldEntity entity = new WereldEntity( wereld );
                    entity.BoundingBox = new BoundingBox(
                        new Vector3( -0.5f, 0, -1.5f ),
                        new Vector3( 0.5f, 2f, 1.5f ) );
                    WereldModel model = new WereldModel();
                    model.WorldMatrix =
                        Matrix.CreateFromYawPitchRoll( 0, -MathHelper.PiOver2, 0 )
                        * Matrix.CreateScale( 0.02f );

                    WereldMesh mesh = new WereldMesh( wereld );
                    mesh.LoadFromXml( TWXmlNode.GetRootNodeFromFile( game.EngineFiles.DirUnitTests + "Mesh_TestSerialize.xml" ) );



                    model.Mesh = mesh;
                    entity.AddNewModel( model );

                    Matrix baseWorldMatrix = Matrix.Identity;
                    baseWorldMatrix *=

                    editor.Wereld.Entities.AddNew( entity.Clone( wereld ) );
                    editor.Wereld.Entities.GetByIndex( editor.Wereld.Entities.Count - 1 ).Move(
                        new Vector3( 10, 0, 0 ) );
                    editor.Wereld.Entities.AddNew( entity.Clone( wereld ) );
                    editor.Wereld.Entities.GetByIndex( editor.Wereld.Entities.Count - 1 ).Move(
                        new Vector3( 0, 0, 5 ) );
                    editor.Wereld.Entities.AddNew( entity.Clone( wereld ) );
                    editor.Wereld.Entities.GetByIndex( editor.Wereld.Entities.Count - 1 ).Move(
                        new Vector3( 5, 0, 0 ) );

                    editor.Wereld.Entities.AddNew( entity.Clone( wereld ) );
                    editor.Wereld.Entities.GetByIndex( editor.Wereld.Entities.Count - 1 ).Move(
                        new Vector3( 20, 0, 0 ) );

                    game.Mouse.CursorEnabled = true;

                    editor.SetActiveTool( tool );

                    editor.WereldViewCamera.Position = new Vector3( 5, 2, 10 );
                    editor.WereldViewCamera.Orientation = Quaternion.Identity;

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

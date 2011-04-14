using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.Entities.Editor.Tools
{
    public class EntitySelectionTool
    {

       


        public void Update()
        {
            //TODO:



            //if ( selectedEntity != null )
            //{
            //    if ( gizmoRotation.ActiveHoverPart == EditorGizmoRotation.GizmoPart.None )
            //        gizmoTranslation.Update( XNAGameControl );
            //    if ( gizmoTranslation.ActiveHoverPart == EditorGizmoTranslation.GizmoPart.None )
            //        gizmoRotation.Update( XNAGameControl );

            //    gizmoScaling.Update( XNAGameControl );
            //}

            //// If gizmo's are not being targeted
            //if ( !IsGizmosTargeted() )
            //{
            //    UpdatePickWorldOud( PickType.All );

            //    if ( XNAGameControl.Mouse.LeftMouseJustPressed )
            //    {
            //        // When don't click on a model but on an empty place in de window, deselect
            //        // Otherwise, don't
            //        if ( XNAGameControl.IsCursorOnControl() )
            //        {
            //            if ( pickedType == PickType.Entities )
            //            {
            //                SelectEntity( pickedEntity );
            //            }
            //            else
            //            {
            //                SelectEntity( null );
            //            }

            //        }
            //    }

            //}

            //if ( selectedEntity != null && !IsGizmosMoving() )
            //{
            //    if ( XNAGameControl.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Delete ) )
            //    {
            //        // Delete selected entity
            //        UnloadEntityRenderData( selectedEntity );
            //        editor.DeleteEditorEntity( selectedEntity );
            //        SelectEntity( null );
            //    }
            //}
        }

        public void Initialize()
        {
            //for ( int i = 0; i < editor.EditorEntities.Count; i++ )
            //{
            //    InitializeEntityRenderData( editor.EditorEntities[ i ] );
            //}

        }

    }
}

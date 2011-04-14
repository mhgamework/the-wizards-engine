using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Editor.World
{
    public class TransformTool : MHGameWork.TheWizards.ServerClient.Editor.IEditorTool
    {
        WorldEditor editor;
        IWorldEditorSelectable active;
        public TransformTool( WorldEditor _worldEditor )
        {
            editor = _worldEditor;
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
            editor.TransformControl.Enabled = false;
        }

        public void Update()
        {
            if ( editor.XNAGameControl.Mouse.LeftMouseJustPressed )
            {
                if ( !editor.TransformControl.IsGizmoTargeted() )
                {
                    // Select object
                    CheckSelect();
                }
            }
            if ( active != null )
            {
                active.Update();
                editor.XNAGameControl.EditorCamera.OrbitPoint = active.GetSelectionCenter();
            }
        }


        private void CheckSelect()
        {
            if ( editor.XNAGameControl.IsCursorOnControl() == false ) return;
            Raycast.RaycastResult<IWorldEditorSelectable> result;
            result = Raycast.RaycastHelper.MultipleRayscast<IWorldEditorSelectable>( editor.Selectables, GetMousePickRay() );
            if ( result.IsHit == false )
            {
                if ( active != null ) active.Deselect();
                active = null;
                editor.TransformControl.Enabled = false;
            }
            else
            {
                active = result.Item;
                result.Item.SelectLastRaycasted();
                editor.TransformControl.Enabled = true;
            }
        }

        private Ray GetMousePickRay()
        {
            return editor.XNAGameControl.GetWereldViewRay( editor.XNAGameControl.Mouse.CursorPositionVector );
        }


        public void Render()
        {
            if ( active != null )
            {
                active.Render();
            }
        }
    }
}

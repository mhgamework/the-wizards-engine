using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class UndoActionEntityTransformation : IUndoAction
    {
        private Transformation oldTransform;

        public Transformation OldTransform
        {
            get { return oldTransform; }
            set { oldTransform = value; }
        }

        private Transformation newTransform;

        public Transformation NewTransform
        {
            get { return newTransform; }
            set { newTransform = value; }
        }

        private EditorEntity editorEntity;

        public EditorEntity EditorEntity
        {
            get { return editorEntity; }
            set { editorEntity = value; }
        }

        private WorldEditor worldEditor;

        public WorldEditor WorldEditor
        {
            get { return worldEditor; }
            set { worldEditor = value; }
        }



        #region IUndoAction Members

        public void Undo()
        {
            //editorEntity.FullData.Transform = oldTransform;
            //worldEditor.UpdateEntityRenderDataTransform( editorEntity );
            //// If selected entity this will put the gizmo's into place
            //worldEditor.SelectEntity( worldEditor.SelectedEntity );
        }

        public void Redo()
        {
            //editorEntity.FullData.Transform = newTransform;
            //worldEditor.UpdateEntityRenderDataTransform( editorEntity );
            //// If selected entity this will put the gizmo's into place
            //worldEditor.SelectEntity( worldEditor.SelectedEntity );
        }

        #endregion
    }
}

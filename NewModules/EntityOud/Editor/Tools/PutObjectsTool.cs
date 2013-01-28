using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Editor.Scene;
namespace MHGameWork.TheWizards.Entities.Editor
{
    /// <summary>
    /// TODO: move to correct namespace
    /// </summary>
    public class PutObjectsTool : ServerClient.Editor.IEditorTool
    {
        private SceneEditor editor;
        private EditorObjectRenderData selectedPutObjectRenderData;
        EditorEntityRenderMode editorEntityRenderMode;


        private bool selectedPutObjectChanged;
        private EditorObject selectedPutObject;
        public EditorObject SelectedPutObject
        {
            get { return selectedPutObject; }
            set
            {
                selectedPutObject = value;
                selectedPutObjectChanged = true;
            }
        }

        public PutObjectsTool(SceneEditor _editor)
        {
            editor = _editor;

        }

        #region IEditorTool Members

        public void Activate()
        {
            LoadSelectedPutObject();
            editorEntityRenderMode = editor.FindRenderMode<EditorEntityRenderMode>();

        }

        public void Deactivate()
        {
        }

        public void Update()
        {
            if (selectedPutObjectChanged)
            {
                LoadSelectedPutObject();
                selectedPutObjectChanged = false;
            }

            SceneEditor.PickResult pick = editor.PickWorld(SceneEditor.PickType.GroundPlane);

            if (editor.Game.Mouse.LeftMouseJustPressed)
            {
                CheckPutObject(pick);
            }


            /*if (selectedPutObject != null && pick.Picked)
            {
                //editor.Form.Text = pickPoint.ToString();
                selectedPutObjectRenderData.SetWorldMatrix(Matrix.CreateTranslation(pick.Point));
                selectedPutObjectRenderData.Render(editor.Game);


            }*/
        }

        /*private void CheckSelectedObject()
        {
            EditorObject newObj = ees.EntityWizardsEditorForm.ObjectsList.GetSelectedObject();
            if ( newObj != selectedPutObject )
            {
                selectedPutObject = newObj;
                LoadSelectedPutObject();

            }
        }*/

        public void PutObject(Vector3 position, EditorObject eObj)
        {
            EditorEntity ent = editor.EditorScene.CreateEntity(eObj);
            throw new InvalidOperationException();
            //ent.CoreData.Transformation = new Transformation(Vector3.One, Quaternion.Identity, position);

            //TODO: This is one fishy call!
            editorEntityRenderMode.InitializeEntityRenderData(ent);

        }

        private void CheckPutObject(SceneEditor.PickResult pick)
        {
            if (selectedPutObject == null) return;
            if (pick.Picked == false) return;
            // Solved: Check if the cursor is on the viewport, we don't want objects placed when pressing toolbar buttons
            if (!editor.Game.IsCursorInWindow()) return;

            if (editor.Game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt)) System.Diagnostics.Debugger.Break();

            PutObject(pick.Point, selectedPutObject);

        }

        public void Render()
        {
            if (selectedPutObjectRenderData != null)
            {
                selectedPutObjectRenderData.Render(editor.XNAGameControl);
            }
            //if ( selectedEntity != null )
            //{
            //    XNAGameControl.LineManager3D.AddAABB( selectedEntity.EditorObject.FullData.BoundingBox, selectedEntity.FullData.Transform.CreateMatrix(), Color.Black );
            //}
        }


        public void LoadSelectedPutObject()
        {
            return;
            // Discard old data
            if (selectedPutObjectRenderData != null)
            {
                selectedPutObjectRenderData.Dispose();
                selectedPutObjectRenderData = null;
            }
            // Load renderdata for the selected object to show a ghost image of the object to place

            if (selectedPutObject != null)
            {
                selectedPutObjectRenderData = new EditorObjectRenderData();
                selectedPutObjectRenderData.Initialize(editor.Game, selectedPutObject.FullData);

            }
        }

        #endregion



    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.Editor.Transform;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Entities.Editor
{
    /// <summary>
    /// Creates and maintains an object tab on the editor form, processes user input on the form
    /// and applies them to the editor object
    /// </summary>
    public class ObjectEditor
    {

        private ObjectEditorForm form;
        public ObjectEditorForm Form
        {
            get { return form; }
            set { form = value; }
        }


        private List<EditorModelRenderData> modelsRenderData = new List<EditorModelRenderData>();
        /// <summary>
        /// Note, i probably should make an EditorObjectRenderData containing these objects
        /// </summary>
        public List<EditorModelRenderData> ModelsRenderData
        {
            get { return modelsRenderData; }
            set { modelsRenderData = value; }
        }

        private EditorModel selectedModel;


        private EditorObject editorObject;
        public EditorObject EditorObject
        {
            get { return editorObject; }
            //set { editorObject = value; }
        }

        private WizardsEditor editor;
        public WizardsEditor Editor
        {
            get { return editor; }
            //set { editor = value; }
        }

        // The dockcontaineritem representing the tab for the model viewer (on barMain)
        public XNAGameControl XNAGameControl
        {
            get { return form.xnaGameControl; }
            //set { xnaGameControl = value; }
        }

        private OpenFileDialog ofdAddModel;

        private TheWizards.Editor.Transform.TransformControl transformControl;

        public ObjectEditor(EditorObject obj, WizardsEditor nEditor)
        {
            editorObject = obj;
            editor = nEditor;

            form = new ObjectEditorForm();
            nEditor.AddMDIForm(form);

            transformControl = new TransformControl(XNAGameControl, form, form.pageGeneral);


            ofdAddModel = new OpenFileDialog();
            ofdAddModel.Filter = "Collada Models|*.dae";


            XNAGameControl.InitializeEvent += new EventHandler(xnaGameControl_InitializeEvent);
            XNAGameControl.DrawEvent += new XNAGameControl.GameTimeEventHandler(xnaGameControl_DrawEvent);
            XNAGameControl.UpdateEvent += new XNAGameControl.GameTimeEventHandler(xnaGameControl_UpdateEvent);


            form.Activated += new EventHandler(form_Activated);
            form.FormClosing += new FormClosingEventHandler(form_FormClosing);
            form.btnAddModel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnAddModel_ItemClick);
            form.txtName.EditValueChanged += new EventHandler(txtName_EditValueChanged);


            //WARNING: not sure whether the device will be initialized or not.
            //          possibly it initializes on a different thread,
            //          which may cause next call to fail
            xnaGameControl_InitializeEvent(null, null);



            editorObject.Changed += new EventHandler(editorObject_Changed);
            editorObject_Changed(editorObject, null);


        }

        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseInternal();
        }

        void form_Activated(object sender, EventArgs e)
        {
            if (form == null) return;
            //TODO: this doesn't seem to work right now.
            editor.FormNew.Ribbon.SelectedPage = form.pageGeneral;
            editor.FormNew.Ribbon.Update();
        }

        void txtName_EditValueChanged(object sender, EventArgs e)
        {
            editorObject.Name = form.txtName.EditValue.ToString();
        }

        void btnAddModel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = ofdAddModel.ShowDialog(editor.Form);
            if (result != DialogResult.OK) return;
            AddColladaModel(ofdAddModel.FileName);
        }

        public void AddColladaModel(string filename)
        {

            ColladaMeshImporter importer = new ColladaMeshImporter(editor.WorldDatabase);

            Collada.COLLADA140.COLLADA collada;
            EditorMesh mesh;




            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            {
                collada = Collada.COLLADA140.COLLADA.FromStream(fs);
            }
            mesh = importer.ImportMesh(collada);



            //TODO

            /*GameFile file = new GameFile(filename);
            ColladaModel collada = ColladaModel.FromFile(file);

            AddColladaModel(collada);*/
        }
        public void AddColladaModel(ColladaModel colladaModel)
        {
            //TODO
            List<EditorModel> newModels = editorObject.AddColladaModels(colladaModel);

            for (int i = 0; i < newModels.Count; i++)
            {
                AddRenderDataFromModel(newModels[i]);

            }
        }

        void editorObject_Changed(object sender, EventArgs e)
        {
            form.Text = "Object: " + editorObject.Name;
            form.txtName.EditValue = editorObject.Name;

            // Following may only be done when the ObjectEditor is the active tab.
            //if ( dockTab.Selected == false ) return;

            //if ( !editor.Form.txtObjectName.Focused )
            //    editor.Form.txtObjectName.ControlText = editorObject.Name;



        }

        void xnaGameControl_UpdateEvent(Microsoft.Xna.Framework.GameTime ntime)
        {
            if (transformControl.Enabled)
            {
                transformControl.Update();

                selectedModel.FullData.ObjectMatrix = transformControl.GetTransformationMatrix();
                selectedModel.RenderData.ObjectMatrix = selectedModel.FullData.ObjectMatrix;

                XNAGameControl.EditorCamera.OrbitPoint = transformControl.Position;
            }
            //TODO: this line should cancel the camera update in the Update method in XNAGameControl
            //if ( editorObject.dockContainerItem != null && !editorObject.dockContainerItem.Displayed ) return;

            // If gizmo's are not being targeted
            if (transformControl.IsGizmoTargeted() == false)
            {
                if (XNAGameControl.Mouse.CursorEnabled)
                {
                    UpdatePickModels();
                    if (XNAGameControl.Mouse.LeftMouseJustPressed)
                    {
                        // When don't click on a model but on an empty place in de window, deselect
                        // Otherwise, don't
                        if (XNAGameControl.IsCursorOnControl())
                        {
                            SelectModel(pickModel);
                        }
                    }
                }
                else
                {
                    pickModel = null;
                }
            }
            else
            {
                pickModel = null;
            }
        }

        void xnaGameControl_DrawEvent(Microsoft.Xna.Framework.GameTime ntime)
        {
            // Note: this can occur when we are disposing of the xnaGameControl
            if (XNAGameControl == null) return;
            //TODO: this line should cancel the camera update in the Update method in XNAGameControl
            //if ( editorObject.dockContainerItem != null && !editorObject.dockContainerItem.Displayed ) return;

            XNAGameControl.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;

            //TODO: Do the render

            for (int i = 0; i < modelsRenderData.Count; i++)
            {
                modelsRenderData[i].RenderBatched();
                //TODO: material class is quite stupid at the moment
                modelsRenderData[i].Material.Render(XNAGameControl);
            }

            if (pickModel != null)
            {
                XNAGameControl.LineManager3D.AddLine(pickPoint, pickPoint + Vector3.Up * 3, Color.Green);
                XNAGameControl.LineManager3D.AddTriangle(pickV1, pickV2, pickV3, Color.Yellow);

            }

            if (selectedModel != null)
            {
                XNAGameControl.LineManager3D.AddAABB(selectedModel.FullData.BoundingBox, selectedModel.FullData.ObjectMatrix, Color.Black);
            }

            transformControl.Render();

        }

        void xnaGameControl_InitializeEvent(object sender, EventArgs e)
        {
            //TODO: Do the initialize

            for (int i = 0; i < editorObject.Models.Count; i++)
            {
                AddRenderDataFromModel(editorObject.Models[i]);
            }

            transformControl.Initialize();
        }

        public void CloseInternal()
        {

            // VERRY important! stop the xna thread!!
            //XNAGameControl.Exit();
            //XNAGameControl.Dispose();
            //form = null;

            Save();

            for (int i = 0; i < this.modelsRenderData.Count; i++)
            {
                modelsRenderData[i].Dispose();
            }
            modelsRenderData = null;
        }

        public void Close()
        {
            form.Close();
        }

        public void Save()
        {
            //TODO: only save when editorObject is changed
            editorObject.Save();
        }


        private void SelectModel(EditorModel model)
        {
            selectedModel = model;
            if (selectedModel != null)
            {
                Vector3 scale, transl;
                Quaternion rot;
                selectedModel.FullData.ObjectMatrix.Decompose(out scale, out rot, out transl);
                transformControl.SetTransformation(transl, rot, scale);
                transformControl.Enabled = true;

            }
            else
            {
                transformControl.Enabled = false;
            }
        }

        public void dockTab_TabActivated(object sender, EventArgs e)
        {
            //NOTE: maybe i should make an interface instead of using these events, and making the WorldEditor invoking the functions
            //  Note: in the interface instead of adding and removing event handlers all the time
            //EDIT: TODO: I'm quite sure i should.


            //editorObject.EditorForm.btnObjectAddModel.Click += new EventHandler( btnObjectAddModel_Click );
            // Allready done in editorObject_Changed
            //editor.Form.txtObjectName.ControlText = editorObject.Name;
            editor.Form.txtObjectName.InputTextChanged += new TextBoxItem.TextChangedEventHandler(txtObjectName_InputTextChanged);

            editor.EnableObjectEditorButtons();
        }

        public void dockTab_TabDeActivated(object sender, EventArgs e)
        {

            //editorObject.EditorForm.btnObjectAddModel.Click -= new EventHandler( btnObjectAddModel_Click );
            editor.Form.txtObjectName.InputTextChanged -= new TextBoxItem.TextChangedEventHandler(txtObjectName_InputTextChanged);
            editor.DisableObjectEditorButtons();
        }

        private void txtObjectName_InputTextChanged(object sender)
        {
            EditorObject.Name = editor.Form.txtObjectName.ControlText;
        }



        private EditorModel pickModel;
        private Vector3 pickPoint;
        private Vector3 pickV1;
        private Vector3 pickV2;
        private Vector3 pickV3;

        private void UpdatePickModels()
        {
            Ray ray = XNAGameControl.GetWereldViewRay(XNAGameControl.Mouse.CursorPositionVector);

            EditorModel closestModel = null;
            float? closestDist = null;
            Vector3 closestV1 = Vector3.Zero, closestV2 = Vector3.Zero, closestV3 = Vector3.Zero;

            for (int i = 0; i < editorObject.Models.Count; i++)
            {
                EditorModel model = editorObject.Models[i];
                Vector3 v1, v2, v3;
                float? dist = model.RayIntersectsModel(ray, Matrix.Identity, out v1, out v2, out v3);

                if (!dist.HasValue) continue;

                if (closestDist.HasValue && closestDist.Value <= dist.Value) continue;

                closestDist = dist;
                closestModel = model;
                closestV1 = v1;
                closestV2 = v2;
                closestV3 = v3;
            }
            if (closestDist.HasValue)
            {
                pickModel = closestModel;
                pickPoint = ray.Position + ray.Direction * closestDist.Value;
                pickV1 = closestV1;
                pickV2 = closestV2;
                pickV3 = closestV3;
            }
            else
            {
                pickModel = null;
            }
        }

        private void AddRenderDataFromModel(EditorModel model)
        {
            EditorModelRenderData renderData = new EditorModelRenderData();
            renderData.Initialize(XNAGameControl, model.FullData);
            modelsRenderData.Add(renderData);
            //EDIT: NOTE: an EditorObject should not have a Transform!!! an entity should
            //renderData.SetParentWorldMatrix( editorObject.FullData.Transform.CreateMatrix() );
            renderData.SetParentWorldMatrix(Matrix.Identity);
            model.RenderData = renderData;
        }

    }
}

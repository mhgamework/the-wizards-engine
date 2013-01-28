using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Entities.Editor;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Entity.Editor
{
    /// <summary>
    /// This should be a non-data class EDIT: (nog steeds ongeveer correct)
    /// Er is een volledige implementatie van dit model op papier. Dit object zou uit een database class opgehaald moeten worden.
    /// </summary>
    public class EditorObject : MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>, IObject
    {

        private DataItem dataItem;
        public DataItem DataItem
        {
            get { return dataItem; }
        }

        public TaggedObject TaggedObject;

        private string name;
        /// <summary>
        /// Note: changes to this variable should be written to the fulldata class somewhere
        /// Note: Maybe fulldata should be splitted into sections
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if ( name == value ) return;
                name = value;

                hasChanged = true;
                OnChanged();
            }
        }

        public event EventHandler Changed;

        private ObjectFullData fullData;

        /// <summary>
        /// This FullData should be unloaded when the object is not under active editing and 
        /// reloaded when being edited by user. This requires an implementation of the Database Class
        /// </summary>
        public ObjectFullData FullData
        {
            get { return fullData; }
            set { fullData = value; }
        }

        private EditorObjectRenderData renderData;
        /// <summary>
        /// This should be done by the database class
        /// </summary>
        public EditorObjectRenderData RenderData
        {
            get { return renderData; }
            set { renderData = value; }
        }


        private ObjectEditor objectEditor;
        public ObjectEditor ObjectEditor
        {
            get { return objectEditor; }
            //set { objectEditor = value; }
        }

        private WizardsEditor editor;
        public WizardsEditor Editor
        {
            get { return editor; }
            //set { editor = value; }
        }

        private bool hasChanged;
        public bool HasChanged
        {
            get { return hasChanged; }
        }

        public ObjectCoreData CoreData = new ObjectCoreData();

        public WizardsEditorFormDevcomponents EditorForm
        {
            get { return editor.Form; }
            //set { editorForm = value; }
        }

        private List<EditorModel> models;

        public List<EditorModel> Models
        {
            get { return models; }
            set { models = value; }
        }


        public EditorObject()
        {
            name = "Noname";
            models = new List<EditorModel>();


        }


        /// <summary>
        /// Sets the DataItem this EditorObject belongs to in the WorldDatabase
        /// </summary>
        public void SetDataItem(DataItem item)
        {
            if (dataItem != null)
                throw new InvalidOperationException("Can only set the DataItem once!!!");
            dataItem = item;
        }

        #region ISimpleTag<TaggedObject> Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.InitTag( TaggedObject obj )
        {
            TaggedObject = obj;
            editor = obj.TagManager.Database.FindService<WizardsEditor>();
            fullData = obj.GetTag<ObjectFullData>();
            LoadFromFullData();

        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.AddReference( TaggedObject obj )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        public void LoadFromFullData()
        {
            name = fullData.Name;
            for ( int i = 0; i < fullData.Models.Count; i++ )
            {
                EditorModel m = new EditorModel();
                m.FullData = fullData.Models[ i ];
                models.Add( m );
            }
        }

        public void SaveToFullData()
        {
            fullData.Name = name;
        }

        public void SaveToDisk()
        {
            fullData.SaveToDisk( editor.Database );
        }

        public void Save()
        {
            SaveToFullData();
            SaveToDisk();
            hasChanged = false;
        }


        /// <summary>
        /// Note: This function adds multiple models if found in the file
        /// </summary>
        /// <param name="filename"></param>
        public List<EditorModel> AddColladaModels( ColladaModel model )
        {
            throw new NotImplementedException();
            List<EditorModel> newModels = EditorModel.FromColladaModel( this, model );
            models.AddRange( newModels );

            for ( int i = 0; i < newModels.Count; i++ )
            {
                FullData.Models.Add( newModels[ i ].FullData );
            }

            CalculateBounding();

            return newModels;

        }

        public EditorMesh AddMeshFromCollada(Collada.COLLADA140.COLLADA collada)
        {
            // Create and add new meshes here!
            ColladaMeshImporter importer = new ColladaMeshImporter();

            EditorMesh mesh;
            mesh = importer.ImportMesh(collada);

            CoreData.Meshes.Add(mesh);
            

            return mesh;
        }

        /// <summary>
        /// Checks whether a ray intersects a model. 
        /// Returns the distance along the ray to the point of intersection, or null
        /// if there is no intersection.
        /// </summary>
        public float? RaycastObject( Ray ray, Matrix worldMatrix, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3 )
        {
            //TODO: do a boundingSphere check on the entire object?


            //EditorModel closestModel = null;
            float? closestDist = null;
            Vector3 closestV1 = Vector3.Zero, closestV2 = Vector3.Zero, closestV3 = Vector3.Zero;

            for ( int i = 0; i < Models.Count; i++ )
            {
                EditorModel model = Models[ i ];
                Vector3 v1, v2, v3;
                float? dist = model.RayIntersectsModel( ray, worldMatrix, out v1, out v2, out v3 );

                if ( !dist.HasValue ) continue;

                if ( closestDist.HasValue && closestDist.Value <= dist.Value ) continue;

                closestDist = dist;
                //closestModel = model;
                closestV1 = v1;
                closestV2 = v2;
                closestV3 = v3;
            }

            vertex1 = closestV1;
            vertex2 = closestV2;
            vertex3 = closestV3;

            return closestDist;

        }

        //void nodeObjectsListAll_NodeDoubleClick( object sender, EventArgs e )
        //{
        //    MakeActiveObject();
        //    OpenObjectEditor();
        //}

        public void MakeActiveObject()
        {
            //    EditorForm.SetActiveEditorObject( this );
            OnChanged();
            //    //nodeObjectsListAll.Style = new DevComponents.DotNetBar.ElementStyle();
            //    //nodeObjectsListAll.Style.BackColor = System.Drawing.Color.Black;
        }
        public void UnMakeActiveObject()
        {
            //    if ( EditorForm.ActiveEditorObject != this ) return;
            //    EditorForm.SetActiveEditorObject( null );
        }

        //void nodeObjectsListAll_NodeClick( object sender, EventArgs e )
        //{
        //    //MakeActiveObject();
        //}


        public void OpenObjectEditor()
        {
            if ( ObjectEditor == null )
            {
                objectEditor = new ObjectEditor( this, editor );
                ObjectEditor.Form.FormClosed += new System.Windows.Forms.FormClosedEventHandler( Form_FormClosed );
            }
            ObjectEditor.Form.Show();
            ObjectEditor.Form.Focus();
            //Note: is this necessary?
            //OnChanged();


        }

        void Form_FormClosed( object sender, System.Windows.Forms.FormClosedEventArgs e )
        {
            //TODO: Dispose ObjectEditor
            objectEditor = null;
        }


        public void CloseObjectEditor()
        {
            if ( ObjectEditor == null ) return;
            ObjectEditor.Close();
            objectEditor = null;
            ////Note: fishy...
            //if ( objectEditor == null ) return;
            //ObjectEditor.Close();
            //objectEditor = null;


            ////TODO: temporary save/update method
            //CalculateBounding();
            //SaveToFullData();

        }


        void ObjectEditor_TabClosed( object sender, DockTabClosingEventArgs e )
        {
            CloseObjectEditor();
        }



        void dockContainerItem_TabClosed( object sender, EventArgs e )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        void dockContainerItem_TabDeactivated( object sender, EventArgs e )
        {
            UnMakeActiveObject();
        }

        void dockContainerItem_TabActivated( object sender, EventArgs e )
        {
            MakeActiveObject();
        }



        public void CalculateBounding()
        {
            //TODO: maybe a vertex accurate method would be more appropriate
            List<Vector3> corners = new List<Vector3>();


            for ( int i = 0; i < models.Count; i++ )
            {
                Vector3[] temp = models[ i ].FullData.BoundingBox.GetCorners();
                Matrix objectMatrix = models[ i ].FullData.ObjectMatrix;
                Vector3.Transform( temp, ref objectMatrix, temp );
                corners.AddRange( temp );
            }

            if ( corners.Count == 0 )
            {
                fullData.BoundingBox = new BoundingBox();
                fullData.BoundingSphere = new BoundingSphere();
                return;
            }

            fullData.BoundingBox = BoundingBox.CreateFromPoints( corners );
            fullData.BoundingSphere = BoundingSphere.CreateFromPoints( corners ); ;
        }

        private void OnChanged()
        {
            if ( Changed != null ) Changed( this, null );


        }





    }
}
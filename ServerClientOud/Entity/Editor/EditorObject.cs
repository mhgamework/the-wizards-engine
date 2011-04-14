using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.Entity;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Entity.Editor
{
    /// <summary>
    /// This should be a non-data class EDIT: (nog steeds ongeveer correct)
    /// Er is een volledige implementatie van dit model op papier. Dit object zou uit een database class opgehaald moeten worden.
    /// </summary>
    public class EditorObject : Database.ISimpleTag<Entity.TaggedObject>
    {
        public TaggedObject TaggedObject;

        private string name;
        /// <summary>
        /// Note: changes to this variable should be written to the fulldata class somewhere
        /// Note: Maybe fulldata should be splitted into sections
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; OnChanged(); }
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

        #region ISimpleTag<TaggedObject> Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.InitTag( TaggedObject obj )
        {
            TaggedObject = obj;
            editor = obj.TagManager.Database.FindService<WizardsEditor>();
            fullData = obj.GetTag<Entity.ObjectFullData>();
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


        /// <summary>
        /// Note: This function adds multiple models if found in the file
        /// </summary>
        /// <param name="filename"></param>
        public List<EditorModel> AddModels( string filename )
        {
            GameFile file = new GameFile( filename );
            List<EditorModel> newModels = EditorModel.FromColladaFile( this, file );
            models.AddRange( newModels );

            for ( int i = 0; i < newModels.Count; i++ )
            {
                FullData.Models.Add( newModels[ i ].FullData );
            }


            return newModels;

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
            if ( ObjectEditor == null ) objectEditor = new ObjectEditor( this, editor );

            objectEditor.Show();
            ObjectEditor.TabClosed += new EventHandler<DockTabClosingEventArgs>( ObjectEditor_TabClosed );

            //Note: is this necessary?
            //OnChanged();


        }


        public void CloseObjectEditor()
        {
            //Note: fishy...
            if ( objectEditor == null ) return;
            ObjectEditor.Close();
            objectEditor = null;


            //TODO: temporary save/update method
            CalculateBounding();
            SaveToFullData();

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
using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Entities.Editor
{
    /// <summary>
    /// TODO: move to the application logic project
    /// </summary>
    public class EntityWizardsEditorExtension : IWizardsEditorDatabaseExtension
    {
        private WizardsEditor editor;
        private EntityWizardsEditorForm EntityWizardsEditorForm;
        public EntityWizardsEditorExtension(WizardsEditor editor)
        {
            this.editor = editor;

            editor.AddDatabaseExtension(this);

            EntityEditorService ees = editor.Database.RequireService<EntityEditorService>();

            EntityWizardsEditorForm = new EntityWizardsEditorForm(this);
            EntityWizardsEditorForm.LoadIntoEditor(editor);

            EntityWorldEditorExtension EntityWorldEditorExtension = new EntityWorldEditorExtension(ees);
            editor.AddWorldEditorExtension(EntityWorldEditorExtension);


            editorObjects = new List<EditorObject>();
            dataItemTypeObject = editor.WorldDatabase.FindOrCreateDataItemType("Object");

            editor.WorldDatabase.RegisterDataElementType(typeof(ObjectFullData), "ObjectFullData");
            editor.WorldDatabase.AddDataElementFactory(
                new ObjectFullDataFactory(editor.WorldDatabase), true);

        }

        private List<EditorObject> editorObjects;

        public List<EditorObject> EditorObjects
        {
            get { return editorObjects; }
        }

        private DataItemType dataItemTypeObject;


        public void BeforeSaveWorkingCopy(MHGameWork.TheWizards.WorldDatabase.WorldDatabase database)
        {
            for (int i = 0; i < editorObjects.Count; i++)
            {
                EditorObject iObj = editorObjects[i];
                if (iObj.DataItem == null)
                {
                    // Create a new DataItem for this object
                    DataItem item = editor.WorldDatabase.WorkingCopy.CreateNewDataItem(dataItemTypeObject);
                    iObj.SetDataItem(item);

                    editor.WorldDatabase.WorkingCopy.PutDataElement(item, iObj.FullData);



                }
            }
        }

        public void AfterLoadWorkingcopy(MHGameWork.TheWizards.WorldDatabase.WorldDatabase database)
        {
            List<DataItem> objectItems = database.WorkingCopy.Revision.GetDataItemsOfType(dataItemTypeObject);
            for (int i = 0; i < objectItems.Count; i++)
            {
                //TODO
                throw new NotImplementedException();

            }

        }

        public EditorObject CreateNewObject()
        {
            TaggedObject obj = EntityWizardsEditorForm.Ems.CreateObject();
            EditorObject eObj = obj.GetTag<EditorObject>();

            EntityWizardsEditorForm.ObjectsList.AddObject(eObj);


            EditorObjects.Add(eObj);

            return eObj;

        }

        

    }
}

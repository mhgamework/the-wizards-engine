using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.Entities.Editor
{
    /// <summary>
    /// Can be splitted into layout and logic classes
    /// </summary>
    public partial class EntityWizardsEditorForm : DevExpress.XtraBars.Ribbon.RibbonForm,
        MHGameWork.TheWizards.Editor.IDevexpressHasDockmanager
    {
        WizardsEditor editor;
        EntityManagerService ems;
        public EditorObjectsList ObjectsList;
        private EntityWizardsEditorExtension extension;

        public EntityManagerService Ems
        {
            get { return ems; }
        }

        public EntityWizardsEditorForm(EntityWizardsEditorExtension _extension)
        {
            extension = _extension;
            InitializeComponent();
        }

        public void LoadIntoEditor(WizardsEditor _editor)
        {
            editor = _editor;
            ObjectsList = new EditorObjectsList(this);
            MHGameWork.TheWizards.Editor.DevExpressRibbonMerger.MergeRibbonForm(this, _editor.FormNew);
            ems = editor.Database.FindService<EntityManagerService>();
        }

        private void btnCreateObject_ItemClick(object sender, ItemClickEventArgs e)
        {
            EditorObject eObj = extension.CreateNewObject();



            eObj.OpenObjectEditor();
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }

        public DevExpress.XtraBars.Docking.DockManager GetDockManager()
        {
            return dockManager1;
        }
    }
}
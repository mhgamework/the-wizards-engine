using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entities.Editor;
using System.Drawing;
using MHGameWork.TheWizards.Entity.Editor;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /// <summary>
    /// Manages the list of objects in a dockable window of the editor.
    /// </summary>
    public class EditorObjectsList
    {
        private readonly Dictionary<EditorObject, TreeListNode> nodes = new Dictionary<EditorObject, TreeListNode>();
        private Dictionary<TreeListNode, EditorObject> objects = new Dictionary<TreeListNode, EditorObject>();

        private WizardsEditor editor;

        public WizardsEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }

        private EntityWizardsEditorForm form;

        public EntityWizardsEditorForm Form
        {
            get { return form; }
            set { form = value; }
        }


        private TreeList tree;
        private TreeListNode nodeObjects;
        private TreeListNode nodeAll;

        private EditorObject selectedObject;

        public EditorObject SelectedObject
        {
            get { return selectedObject; }
            //set { selectedObject = value; }
        }




        public EditorObjectsList( EntityWizardsEditorForm _form )
        {
            //editor = nEditor;
            form = _form;

            tree = form.treeObjects;
            InitTree();
        }

        private void InitTree()
        {
            tree.Nodes.Clear();

            nodeObjects = tree.AppendNode( new object[] { "Objects" }, -1 );

            nodeAll = tree.AppendNode( new object[] { "All" }, nodeObjects );
            tree.FocusedNodeChanged += new FocusedNodeChangedEventHandler( tree_FocusedNodeChanged );
            tree.DoubleClick += new EventHandler( tree_DoubleClick );
        }


        public void tree_DoubleClick( object sender, EventArgs e )
        {
            Point clientP = System.Windows.Forms.Cursor.Position;
            clientP = tree.PointToClient( clientP );
            TreeListHitInfo hit = tree.CalcHitInfo( clientP );
            if ( hit.Node == null ) return;
            EditorObject eObj = null;
            if ( objects.TryGetValue( hit.Node, out eObj ) )
            {
                eObj.OpenObjectEditor();
            }


        }

        void tree_FocusedNodeChanged( object sender, FocusedNodeChangedEventArgs e )
        {
            if ( tree.FocusedNode == null ) throw new Exception();
            EditorObject eObj = null;
            if ( objects.TryGetValue( tree.FocusedNode, out eObj ) )
            {
                SelectObject( eObj );
            }
        }

        public void SelectObject( EditorObject obj )
        {
            selectedObject = obj;
            if ( SelectedObjectChanged != null ) SelectedObjectChanged( this, null );
        }

        public void AddObject( EditorObject obj )
        {
            TreeListNode node = tree.AppendNode( new object[] { obj.Name }, nodeAll );


            nodes.Add( obj, node );
            objects.Add( node, obj );



            obj.Changed += new EventHandler( obj_Changed );
            //node.NodeDoubleClick += new EventHandler( node_NodeDoubleClick );

            //tree.BeforeNodeSelect += new AdvTreeNodeCancelEventHandler( tree_BeforeNodeSelect );
            //tree.AfterNodeSelect += new AdvTreeNodeEventHandler( tree_AfterNodeSelect );

        }

        //void tree_AfterNodeSelect( object sender, AdvTreeNodeEventArgs e )
        //{
        //    CheckSelectedObjectChanged();
        //}


        //void tree_BeforeNodeSelect( object sender, AdvTreeNodeCancelEventArgs e )
        //{

        //}

        void node_NodeClick( object sender, EventArgs e )
        {
            CheckSelectedObjectChanged();
        }

        void node_NodeDoubleClick( object sender, EventArgs e )
        {
            //EditorObject obj = objects[ (Node)sender ];

            //obj.OpenObjectEditor();
        }

        void obj_Changed( object sender, EventArgs e )
        {
            if ( !( sender is EditorObject ) )
                throw new Exception( "Shouldn't the sender always be the object that has changed?" );

            EditorObject obj = (EditorObject)sender;

            nodes[ obj ].SetValue( tree.Columns[ 0 ], obj.Name );
        }

        public EditorObject GetSelectedObject()
        {
            //throw new Exception( "Deprecated" );
            //Not deprecated anymore, events are now deprecated ...

            if ( tree.FocusedNode == null ) return null;
            EditorObject eObj = null;
            objects.TryGetValue( tree.FocusedNode, out eObj );
            return eObj;
        }

        [Obsolete( "Maybe this is necessary, but for now just check this yourself in an xnagame loop..." )]
        public event EventHandler SelectedObjectChanged;

        private void CheckSelectedObjectChanged()
        {
            //EditorObject obj = null;
            //if ( tree.SelectedNode != null )
            //{
            //    objects.TryGetValue( tree.SelectedNode, out obj );
            //}
            //if ( selectedObject != obj )
            //{
            //    // Changed!
            //    selectedObject = obj;
            //    if ( SelectedObjectChanged != null ) SelectedObjectChanged( this, null );

            //}
        }

    }
}

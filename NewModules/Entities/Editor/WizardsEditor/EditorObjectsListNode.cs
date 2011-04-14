using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraTreeList.Nodes;
using MHGameWork.TheWizards.ServerClient.Entity.Editor;

namespace MHGameWork.TheWizards.Entities.Editor.WizardsEditor
{
    public class EditorObjectsListNode : TreeListNode
    {
        public EditorObject EditorObject;

        public EditorObjectsListNode( int id, TreeListNodes owner )
            : base( id, owner )
        {
        }
        //private string NodeParameters()
        //{
        //    string s = "Node " + this.Id.ToString();
        //    if ( this.HasChildren )
        //    {
        //        s += "; ChildNodes Count = " + this.Nodes.Count;
        //        if ( this.Expanded )
        //            s += "; Expanded";
        //    }
        //    if ( this.Tag != null && !this.Tag.Equals( "" ) )
        //        s += "; Tag: " + this.Tag.ToString();
        //    return s;
        //}
        public override object this[ object columnID ]
        {
            get { return EditorObject.Name; }
            set
            {
                if ( columnID.Equals( this.TreeList.Columns[ 0 ] ) )
                {
                    this.
                    //this.Tag = value;
                    this.TreeList.LayoutChanged();
                }
            }
        }


    }
}

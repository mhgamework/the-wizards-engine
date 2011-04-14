using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiControlCollection
    {
        private List<GuiControl> innerList;
        private GuiControl parent;

        public GuiControlCollection(GuiControl nParent)
        {
            parent = nParent;
            innerList = new List<GuiControl>();
        }

        public void Add( GuiControl item )
        {
            innerList.Add( item );
            item.Parent = parent;
            if ( item.TabIndex == -1 )
            {
                //Get max tabindex
                int max = 0;
                for ( int i = 0; i < innerList.Count; i++ )
                {
                    if ( innerList[ i ].TabIndex > max ) max = innerList[ i ].TabIndex;
                }
                item.TabIndex = max + 1;
            }

        }

        public void Remove( GuiControl item )
        {
            innerList.Remove( item );
            item.Parent = null;

        }


        public void SetChildIndex( GuiControl item, int targetIndex )
        {
            throw new InvalidOperationException( "Operation not yet implemented!" );
        }


        public int Count
        {
            get { return innerList.Count; }
        }

        public GuiControl this[ int index ]
        {
            get { return innerList[ index ]; }
        }
    }
}

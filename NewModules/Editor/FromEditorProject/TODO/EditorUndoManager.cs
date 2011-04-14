using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorUndoManager
    {
        private IUndoAction[] undoList;

        private int maxUndo;

        public int MaxUndo
        {
            get { return maxUndo; }
            //set { maxUndo = value; }
        }

        /// <summary>
        /// Index of the first undoAction in the undoList
        /// </summary>
        private int firstIndex;

        /// <summary>
        /// Index of the last UndoAction in the undoList. This is -1 when list is empty.
        /// </summary>
        private int lastIndex;

        /// <summary>
        /// Index of the IUndoAction that will be undone when Undo is called. This is -1 when all actions have been undone.
        /// </summary>
        private int currentIndex;

        public EditorUndoManager( int nMaxUndo )
        {
            maxUndo = nMaxUndo;

            undoList = new IUndoAction[ maxUndo ];

            firstIndex = 0;
            lastIndex = -1;
            currentIndex = -1;

        }

        public void AddIUndoAction( IUndoAction action )
        {
            if ( currentIndex == -1 )
            {
                // All actions have been undone, and adding an undo disables redo, so we actually have a fresh state
                firstIndex = 0;
                lastIndex = 0;
                currentIndex = 0;
                undoList[ 0 ] = action;
            }
            else
            {
                // Add the undo after the current
                currentIndex = IndexInArray( currentIndex + 1 );
                undoList[ currentIndex ] = action;

                lastIndex = currentIndex;
                if ( firstIndex == lastIndex ) firstIndex = IndexInArray( lastIndex + 1 );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns true when an action was undone</returns>
        public bool Undo()
        {
            if ( lastIndex == -1 ) return false;
            if ( currentIndex == -1 ) return false;

            undoList[ currentIndex ].Undo();
            if ( currentIndex == firstIndex )
            {
                currentIndex = -1;
            }
            else
            {
                currentIndex = IndexInArray( currentIndex - 1 );
            }

            return true;
        }

        public bool Redo()
        {
            if ( lastIndex == -1 ) return false;
            if ( currentIndex == lastIndex )
            {
                // No redo possible
                return false;
            }

            if ( currentIndex == -1 )
            {
                currentIndex = firstIndex;
            }
            else
            {
                currentIndex = IndexInArray( currentIndex + 1 );
            }

            undoList[ currentIndex ].Redo();

            return true;

        }

        private int IndexInArray( int index )
        {
            return ( index + MaxUndo ) % maxUndo;
        }



    }
}

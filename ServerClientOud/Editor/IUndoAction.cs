using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public interface IUndoAction
    {
        void Undo();
        void Redo();
    }
}

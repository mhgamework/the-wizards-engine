using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public interface IEditorTool
    {
        void Activate();
        void Deactivate();
        void Update();
        void Render();

    }
}

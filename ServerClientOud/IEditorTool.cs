using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IEditorTool
    {

        void Render();
        void Update();
        void OnActivate();
        void OnDeactivate();

    }
}

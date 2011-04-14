using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Editor.World
{
    public interface IWorldEditorRenderMode
    {
        void Activate();
        void Deactivate();
        void Update();
        void Render();

    }
}

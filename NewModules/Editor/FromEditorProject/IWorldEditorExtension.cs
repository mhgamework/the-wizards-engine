using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Editor
{
    public interface IWorldEditorExtension
    {
        void Load( ServerClient.Editor.WorldEditor worldEditor );

        void Unload( ServerClient.Editor.WorldEditor worldEditor );
        
    }
}
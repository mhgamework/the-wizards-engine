using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.GodGame.ToolSelection
{
    public interface IToolSelectionItem
    {
        string GetDisplayName();
        void Select(ToolSelectionMenu menu);
    }
}

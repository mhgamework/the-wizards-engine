using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Editor
{
    public interface IWizardsEditorDatabaseExtension
    {
        void BeforeSaveWorkingCopy(TheWizards.WorldDatabase.WorldDatabase database);

        void AfterLoadWorkingcopy(TheWizards.WorldDatabase.WorldDatabase database);
    }
}

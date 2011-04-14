using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;

namespace MHGameWork.TheWizards.Character.Editor
{
    public class CharacterEditorService : TheWizards.ServerClient.Database.IGameService002
    {

        public WizardsEditor WizardsEditor;
        public CharacterWizardsEditorForm CharacterWizardsEditorForm;


        #region IGameService002 Members

        public void Load( Database.Database _database )
        {
            WizardsEditor = _database.FindService<WizardsEditor>();
            CharacterWizardsEditorForm = new CharacterWizardsEditorForm();
            CharacterWizardsEditorForm.LoadIntoEditor( WizardsEditor );

        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

}

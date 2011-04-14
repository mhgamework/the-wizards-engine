using System;
using MHGameWork.TheWizards.Editor;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient.TWClient;
using Database=MHGameWork.TheWizards.Database.Database;

namespace SamplePlugin
{
    public class SamplePlugin : IPlugin002
    {
        WizardsEditor editor;
       


        public void LoadPlugin( Database database )
        {
            editor = database.FindService<WizardsEditor>();

            //Test.RibbonForm1 form = new global::SamplePlugin.Test.RibbonForm1();
            //editor.FormNew.Ribbon.Pages.Add( form.ribbonPageMain );

        }

    }
}
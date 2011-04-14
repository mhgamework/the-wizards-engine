using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Terrain.Editor;
using MHGameWork.TheWizards.ServerClient.Entity;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class EntityEditorPlugin : ServerClient.TWClient.IPlugin002
    {

        public void LoadPlugin( Database.Database database )
        {
            //Note: disabled this
            //database.RequireService<EntityEditorService>();

            
        }

       

    }
}

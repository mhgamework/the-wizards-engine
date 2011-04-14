using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Terrain
{
    public class TerrainPlugin : TWClient.IPlugin002
    {

        #region IPlugin002 Members

        public void LoadPlugin( MHGameWork.TheWizards.ServerClient.Database.Database database )
        {
            TerrainManagerService terrManager = new TerrainManagerService( database );
            database.AddService( terrManager );

            TWClient.RendererService rendererService = database.FindService<TWClient.RendererService>();
            if ( rendererService != null )
            {
                //rendererService.AddIRenderer( new Terrain.Rendering.TerrainRendererService( database ) );
            }


        }

        #endregion
    }
}

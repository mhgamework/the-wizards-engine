using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Database;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public class PhysicsService : IGameService002
    {
        private StillDesign.PhysX.Core physXCore;



        public PhysicsService()
        {
           physXCore = null;
		   physXCore = new StillDesign.PhysX.Core(); //Temp
            
        }

        #region IGameService002 Members

        public void Load( MHGameWork.TheWizards.Database.Database _database )
        {
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
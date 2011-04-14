using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    [Obsolete( "Use IGameService002 instead. See Database for details" )]
    public interface IGameService : IDisposable
    {
    }
    /// <summary>
    /// This is the new Service interface.
    /// </summary>
    public interface IGameService002 : IDisposable, IGameService
    {
        void Load( TheWizards.Database.Database _database );
    }
}

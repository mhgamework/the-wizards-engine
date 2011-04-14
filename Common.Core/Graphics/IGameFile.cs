using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public interface IGameFile
    {
        int ID
        { get;}

        string GetFullFilename();
    }
}

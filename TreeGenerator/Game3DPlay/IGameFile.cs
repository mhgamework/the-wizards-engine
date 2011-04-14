using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IGameFile
    {
        int ID
        { get;}

        string GetFullFilename();
    }
}

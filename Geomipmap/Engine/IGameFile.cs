using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.Engine
{
    public interface IGameFile
    {
        //const int ClientGameFileStartID = 1000000000;
        /// <summary>
        /// This is the unique ID of the gamefile
        /// There are the server files: this are the files that are common to all users and the server
        /// There are user files: this are files that are created by the client itself (preprocessed data, keyboard settings, graphics settings etc)
        /// Values:
        /// -1                                      : This is the same a null, so no gamefile
        /// 0                                       : This means the the ID is not yet set. (while -1 means it is set but it is not a file)
        /// 1 to ClientGameFileStartID -1           : This are ID's for server files
        /// ClientGameFileStartID to int.MaxValue   : this are ID's for client files
        /// </summary>
        int ID
        { get;}

        string GetFullFilename();
    }
}

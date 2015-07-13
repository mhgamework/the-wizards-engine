using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Server
{
    public interface IClient
    {
        /// <summary>
        /// Returns true if this client is ready for transmitting
        /// </summary>
        Boolean IsReady { get; }
    }
}

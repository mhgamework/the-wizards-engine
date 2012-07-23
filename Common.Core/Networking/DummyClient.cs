using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.Networking
{
    public class DummyClient : IClient
    {
        public string Name { get; set; }

        public DummyClient(string name)
        {
            Name = name;
        }

        public bool IsReady
        {
            get { return true; }
        }
    }
}

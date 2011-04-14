using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public struct MeshID : IID
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
	
    }
}

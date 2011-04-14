using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public struct WereldModelID : IID
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
	

        public WereldModelID( int nID )
        {
            id = nID;
        }

        public override string ToString()
        {
            return id.ToString();
        }

        public static WereldModelID Empty
        { get { return new WereldModelID( -1 ); } }

    }
}

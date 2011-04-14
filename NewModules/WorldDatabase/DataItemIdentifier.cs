using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public struct DataItemIdentifier
    {
          private int ID;

        public DataItemIdentifier(int id)
        {
            ID = id;
        }

        public int Id
        {
            get { return ID; }
        }
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TileEngine
{
    public class TileFaceType
    {
        private static int nextID = 1;
        public int ID { get; private set; }

        public TileFaceType()
        {
            ID = nextID;
            ID++;
        }


        public string Name { get; set; }
        private TileFaceType parent;

        public bool FlipWinding { get; set;}

        public TileFaceType GetRoot()
        {
            
            if (parent == null)
                return this;

            return parent.GetRoot();
        }

        public void SetParent(TileFaceType value)
        {
            parent = value;
        }

     
        public bool GetTotalWinding()
        {
            if (parent == null) return FlipWinding;
            return FlipWinding ^ parent.GetTotalWinding();
        }
    }
}

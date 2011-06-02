using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.TileEngine
{
    public class TileFaceType : IAsset
    {
        private static int nextID = 1;
        public int ID { get; private set; }

        public Guid Guid { get; private set; }

        public TileFaceType(Guid guid)
        {
            ID = nextID;
            nextID++;
            
            this.Guid = guid;
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

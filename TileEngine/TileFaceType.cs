using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TileEngine
{
    public class TileFaceType
    {
        public string Name { get; set; }
        private TileFaceType parent;

        public bool flipWinding { get; set;}

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
    }
}

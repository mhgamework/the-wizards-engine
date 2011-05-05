using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TileEngine
{
    public class TileFaceType
    {
        public string Name;
        private TileFaceType root;

        public TileFaceType GetRoot()
        {
            if (root == null)
                return this;

            return root.GetRoot();
        }

        public void SetRoot(TileFaceType newRoot)
        {
            root = newRoot;
        }
    }
}

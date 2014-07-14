using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class HoleType : GameVoxelType
    {
        public HoleType() : base("Hole")
        {
            Color = System.Drawing.Color.Gray;
        }
    }
}

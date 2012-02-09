using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Resposible for storing the available DynamicTypes
    /// </summary>
    public class DynamicTypeFactory
    {
        public List<WallType> WallTypes = new List<WallType>();
        public List<FloorType> FloorTypes = new List<FloorType>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for storing and providing BuildUnits of this type.
    /// </summary>
    public class FloorType
    {
        private BuildUnit defaultUnit;
        public BuildUnit DefaultUnit
        {
            get { return new BuildUnit(defaultUnit.Mesh, "FloorType"); }
            set { defaultUnit = value; value.buildUnitType = "FloorType"; }
        }
    }
}

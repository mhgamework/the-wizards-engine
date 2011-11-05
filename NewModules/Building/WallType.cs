using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for storing and providing BuildUnits of this type.
    /// </summary>
    public class WallType
    {
        private BuildUnit straightUnit;
        public BuildUnit StraightUnit
        {
            get { return new BuildUnit(straightUnit.Mesh, "WallType"); }
            set { straightUnit = value; value.buildUnitType = "WallType"; }
        }

        private BuildUnit pillarUnit;
        public BuildUnit PillarUnit
        {
            get { return new BuildUnit(pillarUnit.Mesh, "WallType"); }
            set { pillarUnit = value; value.buildUnitType = "WallType"; }
        }

        private BuildUnit skewUnit;
        public BuildUnit SkewUnit
        {
            get { return new BuildUnit(skewUnit.Mesh, "WallType"); }
            set { skewUnit = value; value.buildUnitType = "WallType"; }
        }
    }
}

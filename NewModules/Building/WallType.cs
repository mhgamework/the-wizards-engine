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
            get { return new BuildUnit(straightUnit.Mesh, "StraightWallType"); }
            set { straightUnit = value; value.buildUnitType = "StraightWallType"; }
        }

        private BuildUnit pillarUnit;
        public BuildUnit PillarUnit
        {
            get { return new BuildUnit(pillarUnit.Mesh, "StraightWallType"); }
            set { pillarUnit = value;  value.buildUnitType = "StraightWallType"; }
        }

        private BuildUnit skewUnit;
        public BuildUnit SkewUnit
        {
            get { return new BuildUnit(skewUnit.Mesh, "SkewWallType"); }
            set { skewUnit = value; value.buildUnitType = "SkewWallType"; }
        }
    }
}

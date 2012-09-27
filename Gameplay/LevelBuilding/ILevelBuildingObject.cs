using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.LevelBuilding
{

    public interface ILevelBuildingObject
    {
        Vector3 Position { get; set; }
        Quaternion Rotation {get; set;}

        bool Visible { set; }

        ILevelBuildingObject Clone();

    }
}

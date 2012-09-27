using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.LevelBuilding
{

    public interface ILevelBuildingObjectType
    {
        void ProcessInput(LevelBuildingObjectFactory factory, LevelBuildingInfo info);
    }
}

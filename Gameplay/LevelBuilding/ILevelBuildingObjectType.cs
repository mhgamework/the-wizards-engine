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
        /// <summary>
        /// Returns the type that an object obtained by a raycast in the world on an object placed with this LevelBuildingObjectType should have.
        /// </summary>
        /// <returns></returns>
        Type GetMatchingObjectType();
    }
}

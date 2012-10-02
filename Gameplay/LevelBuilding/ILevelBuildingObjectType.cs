using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.LevelBuilding
{

    public interface ILevelBuildingObjectType
    {
        /// <summary>
        /// When this method is called, there is an object in the world of this LBObjectType created, or a new one should be created.
        /// </summary>
        void ProcessInput(LevelBuildingObjectFactory factory, LevelBuildingInfo info);
        object GetNewObject();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.LevelBuilding
{
    /// <summary>
    /// Responsible for manipulating BuildingObjects in the world
    /// </summary>
    public interface ILevelBuildingObjectType
    {
        /// <summary>
        /// When this method is called, there is an object in the world of this LBObjectType created, or a new one should be created.
        /// </summary>
        void ProcessInput(LevelBuildingObjectFactory factory, LevelBuildingInfo info);
        object GetNewObject();
        void Delete(object o);
    }
}

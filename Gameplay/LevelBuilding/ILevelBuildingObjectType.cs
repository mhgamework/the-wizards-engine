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

        /// <summary>
        /// Produces a new object of the type it can manipulate. (only creates)
        /// </summary>
        object GetNewObject();

        /// <summary>
        /// Perform a cleanup of the object so it can be safely removed.
        /// </summary>
        void Delete(object o);

        /// <summary>
        /// Returns true if this ObjectType can manipulate the given object.
        /// </summary>
        bool CanHandleObject(object o);
    }
}

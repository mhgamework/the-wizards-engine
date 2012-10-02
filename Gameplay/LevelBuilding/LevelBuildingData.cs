using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingData
    {
        public Dictionary<Object, ILevelBuildingObjectType> Data { get; private set; }

        public LevelBuildingData()
        {
            Data = new Dictionary<object, ILevelBuildingObjectType>();
        }

        public void AddLevelBuildingObject(Object o, ILevelBuildingObjectType type)
        {
            Data.Add(o, type);
        }

        public void RemoveLevelBuildingObject(Object o)
        {
            Data.Remove(o);
        }
    }
}

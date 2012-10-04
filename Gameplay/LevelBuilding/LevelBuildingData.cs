using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingData
    {
        public List<Object> Data { get; private set; }

        public LevelBuildingData()
        {
            Data = new List<Object>();
        }

        public void AddLevelBuildingObject(Object o)
        {
            Data.Add(o);
        }

        public void RemoveLevelBuildingObject(Object o)
        {
            Data.Remove(o);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingObjectFactory
    {
        private List<ILevelBuildingObjectType> types = new List<ILevelBuildingObjectType>();

        public void AddLevelBuildingObjectType(ILevelBuildingObjectType t)
        {
            if (!types.Contains(t))
                types.Add(t);
        }

        public ILevelBuildingObjectType GetNextType(ILevelBuildingObjectType current)
        {
            if (current == null || !types.Contains(current))
                return types[0];

            int index = types.IndexOf(current);
            if (index == types.Count - 1)
                return types[0];
            
            return types[index + 1];
        }

        public ILevelBuildingObjectType GetPreviousType(ILevelBuildingObjectType current)
        {
            if (current == null || !types.Contains(current))
                return types[0];

            int index = types.IndexOf(current);
            if (index == 0)
                return types[types.Count - 1];

            return types[index - 1];
        }

        public Object CreateFromType(ILevelBuildingObjectType t)
        {
            throw new NotImplementedException();
        }

        public ILevelBuildingObjectType GetTypeFromObject(object o)
        {
            throw new NotImplementedException();
        }
    }
}

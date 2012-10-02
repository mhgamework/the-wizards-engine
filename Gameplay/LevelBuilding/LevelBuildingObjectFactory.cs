using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.LevelBuilding
{
    /// <summary>
    /// Responsible for LevelBuildingObject creation and deletion, as well as providing all possible LevelBuildingObjectTypes.
    /// </summary>
    public class LevelBuildingObjectFactory
    {
        private List<ILevelBuildingObjectType> types = new List<ILevelBuildingObjectType>();
        public LevelBuildingData LevelBuildingData { get; private set; }

        public LevelBuildingObjectFactory()
        {
            LevelBuildingData = new LevelBuildingData();
        }

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
            if (!types.Contains(t))
                throw new Exception("Invalid LevelBuildingObjectType given!");

            var created = t.GetNewObject();
            LevelBuildingData.AddLevelBuildingObject(created, t);

            return created;
        }

        public ILevelBuildingObjectType GetLevelBuildingTypeFromObject(object o)
        {
            ILevelBuildingObjectType val;
            LevelBuildingData.Data.TryGetValue(o, out val);
            return val;
        }
    }
}

using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.TileEngine
{
    public class SimpleWorldObjectTypeFactory : IWorldObjectTypeFactory
    {
        private List<WorldObjectType> types = new List<WorldObjectType>();


        public WorldObjectType GetWorldObjectType(Guid guid)
        {
            return types.Find(o => o.Guid.Equals(guid));
        }


        public void AddWorldObjectType(WorldObjectType type)
        {
            if (GetWorldObjectType(type.Guid) != null)
                throw new ArgumentException("Same guid already added tot the factory");

            types.Add(type);

        }
    }
}
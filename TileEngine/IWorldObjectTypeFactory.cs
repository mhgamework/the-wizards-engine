using System;

namespace MHGameWork.TheWizards.TileEngine
{
    public interface IWorldObjectTypeFactory

    {
        WorldObjectType GetWorldObjectType(Guid guid);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public interface IWorldObjectTypeFactory
    {
        WorldObjectType GetWorldObjectType(Guid guid);
    }
}

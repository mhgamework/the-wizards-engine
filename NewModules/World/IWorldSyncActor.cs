using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.World
{
    public interface IWorldSyncActor
    {
        Vector3 GlobalPosition { get; set; }
        Matrix GlobalOrientation { get; set; }
    }
}

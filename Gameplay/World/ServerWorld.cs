using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.World
{
    /// <summary>
    /// Responsible for providing WorldData
    /// Responsible for allowing changes to world data, and allow access to those changes.
    /// Algorithm: store all changes for 1 frame into a buffer, so that all classes needing those changes can apply them in 1 frame
    /// NOTE: maybe this is a subclass of ClientWorld? Probably a shared part
    /// </summary>
    public class ServerWorld
    {
    }
}

using System;
using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;

namespace MHGameWork.TheWizards.RTSTestCase1
{
    /// <summary>
    /// Binds entities to visualize, simulate physx for the objects in the RTSTestCase1
    /// </summary>
    public class PhysicalSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var c in TW.Data.GetChangesOfType<Physical>().ToArray())
            {
                if (c.Change == ModelChange.Removed)
                    continue;
                ((Physical)c.ModelObject).Update();
            }

        }


    }
}

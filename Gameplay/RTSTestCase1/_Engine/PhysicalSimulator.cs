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
            foreach (var c in TW.Data.GetChangesOfType<IPhysical>().ToArray())
            {
                var phys = c.ModelObject as IPhysical;

                if (phys.Physical == null)
                    throw new InvalidOperationException(
                        "The Physical object should be assigned by the modelobject. " +
                        "Later on this should be done automatically in the constructor of a modelobject!!");

                if (c.Change == ModelChange.Removed)
                {
                    //TODO: this is not supported anymore!!!
                    //if (phys.Physical.Entity != null)
                    //    TW.Data.RemoveObject(phys.Physical.Entity);
                    //phys.Physical.Entity = null;
                    //TW.Data.RemoveObject(phys.Physical);
                    //phys.Physical = null;

                    continue;
                }

                //if (phys.Physical == null) phys.Physical = new Physical();
                //phys.UpdatePhysical();
            }
            foreach (var phys in TW.Data.Objects.OfType<IPhysical>())
            {
                // This should be called on change only, but due to bugs it was moved to executing every frame.
                phys.UpdatePhysical();
            }

            foreach (var c in TW.Data.GetChangesOfType<Physical>().ToArray())
            {
                if (c.Change == ModelChange.Removed)
                    continue;
                ((Physical)c.ModelObject).Update();
            }

        }


    }
}

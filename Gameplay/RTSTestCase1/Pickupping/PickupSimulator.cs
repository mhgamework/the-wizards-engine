using System;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.Pickupping
{
    /// <summary>
    /// Responsible for simulating a pickup of a droppedthing
    /// </summary>
    public class PickupSimulator : ISimulator
    {
        private readonly PickupPhysXUpdater pickupPhysXUpdater = new PickupPhysXUpdater();

        public void Simulate()
        {
            foreach (IPickupObject obj in TW.Data.Objects.Where(o => o is IPickupObject).Cast<IPickupObject>().ToArray())
                pickupPhysXUpdater.Update(obj);
        }
    }
}
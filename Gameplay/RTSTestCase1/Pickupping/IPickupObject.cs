using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.Pickupping
{
    /// <summary>
    /// Represents smth that can pickup and hold an object
    /// </summary>
    public interface IPickupObject
    {
        Actor GetHoldingActor();
        Vector3 GetHoldingPosition();
        void DropHolding();
    }
}

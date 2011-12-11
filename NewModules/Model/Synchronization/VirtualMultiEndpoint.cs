using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Model.Synchronization
{
    /// <summary>
    /// endpoint that forwards to multiple endpoints
    /// </summary>
    public class VirtualMultiEndpoint:  IVirtualEndpoint
    {
        public void ApplyModelChanges(VirtualModelSyncer.ChangesBuffer changes)
        {
            throw new NotImplementedException();
        }
    }
}

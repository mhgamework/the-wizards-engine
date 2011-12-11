using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Model.Synchronization
{
    public interface IVirtualEndpoint
    {
        void ApplyModelChanges(VirtualModelSyncer.ChangesBuffer changes);
    }
}

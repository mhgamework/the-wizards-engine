using System;

namespace MHGameWork.TheWizards.ModelContainer.Synchronization
{
    /// <summary>
    /// endpoint that forwards to multiple endpoints
    /// TODO: finish this
    /// </summary>
    public class VirtualMultiEndpoint:  IVirtualEndpoint
    {
        public void ApplyModelChanges(VirtualModelSyncer.ChangesBuffer changes)
        {
            throw new NotImplementedException();
        }
    }
}

﻿namespace MHGameWork.TheWizards.ModelContainer.Synchronization
{
    /// <summary>
    /// TODO: finish this
    /// </summary>
    public interface IVirtualEndpoint
    {
        void ApplyModelChanges(VirtualModelSyncer.ChangesBuffer changes);
    }
}

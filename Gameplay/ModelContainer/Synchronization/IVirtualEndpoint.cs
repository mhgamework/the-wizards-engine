namespace MHGameWork.TheWizards.ModelContainer.Synchronization
{
    public interface IVirtualEndpoint
    {
        void ApplyModelChanges(VirtualModelSyncer.ChangesBuffer changes);
    }
}

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// If this interface is implemented by a class annotated with the PersistedObjectAttribute, 
    /// its event handlers are called when serializing/deserializing
    /// </summary>
    public interface IPOEventsReceiver
    {
        void OnBeforeSerialize();
        void OnAfterDeserialize();
    }
}
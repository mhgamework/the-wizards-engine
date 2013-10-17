namespace MHGameWork.TheWizards.SkyMerchant._Engine.ChangeNotification
{
    /// <summary>
    /// Collects all changes from the TrackChangesAttribute
    /// </summary>
    public interface IChangesSink
    {
        void NotifyChange(object obj, string locationName);
    }

    class NullChangesSink : IChangesSink
    {
        public void NotifyChange(object obj, string locationName)
        {
            
        }
    }

    public static class ChangesSink
    {
        static ChangesSink()
        {
            Current = new NullChangesSink();
        }
        public static IChangesSink Current { get; private set; }
        public static void SetSink(IChangesSink sink)
        {
            Current = sink;
        }
    }
}
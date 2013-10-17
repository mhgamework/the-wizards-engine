namespace MHGameWork.TheWizards.SkyMerchant._Engine.ChangeNotification
{
    /// <summary>
    /// Implements the default to use IChangesSink. This sink allows creating change observers.
    /// </summary>
    public class DefaultChangesSink:IChangesSink
    {
        public void NotifyChange(object obj, string locationName)
        {
            throw new System.NotImplementedException();
        }
    }
}
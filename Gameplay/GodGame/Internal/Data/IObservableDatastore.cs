using System;

namespace MHGameWork.TheWizards.GodGame.Internal.Data
{
    /// <summary>
    /// Exposes a read-write datastore where changes to records can be observed
    /// </summary>
    public interface IObservableDatastore
    {
        IObservable<IDataIdentifier> Changes { get; }
    }
}
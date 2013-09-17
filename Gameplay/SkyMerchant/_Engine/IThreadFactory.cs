using System.Threading;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Responsible for creating new threads
    /// </summary>
    public interface IThreadFactory
    {
        Thread CreateThread(ThreadStart body);
    }
}
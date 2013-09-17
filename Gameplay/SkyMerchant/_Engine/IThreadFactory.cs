using System;
using System.Threading;
using MHGameWork.TheWizards.Debugging;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Responsible for creating new threads
    /// </summary>
    public interface IThreadFactory
    {
        Thread CreateThread(ThreadStart body);
    }

    public class SimpleThreadFactory : IThreadFactory
    {
        public Thread CreateThread(ThreadStart body)
        {
            return new Thread(delegate()
                {
                    try
                    {
                        body();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        DI.Get<IErrorLogger>().Log(ex,"SimpleThreadFactory thread");
                        throw;
                    }
                });
        }
    }
}
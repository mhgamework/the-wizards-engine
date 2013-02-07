using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace MHGameWork.TheWizards.Engine.Services
{
    /// <summary>
    /// Responsible for managing the WPF application for the engine, used as a service.
    /// TODO: add an interface?
    /// TODO: add window manager so they get cleaned up on hotload
    /// </summary>
    public class WPFApplicationService
    {
        private Application app;

        public WPFApplicationService()
        {
            var ev = new AutoResetEvent(false);
            var t = new Thread(delegate()
                {
                    app = new Application();
                    ev.Set();
                    app.Run();
                });
            t.SetApartmentState(ApartmentState.STA);
            t.Name = "Engine WPF Thread";
            t.IsBackground = true;
            t.Start();

            ev.WaitOne();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using MHGameWork.TheWizards.Forms;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.Wpf
{
    public class XNAGameWpf : IWindowFactory, IXNAObject
    {
        private Application app;
        private List<IClassForm> forms;
        public bool ApplicationStarted { get; private set; }

        public XNAGameWpf()
        {
            forms = new List<IClassForm>();

        }

        public Window CreateWindow()
        {
            startApplication();

            return (Window)app.Dispatcher.Invoke(new Func<Window>(() => new Window()), null);
        }


        private void startApplication()
        {
            if (ApplicationStarted) return;

            if (Application.Current != null)
                throw new InvalidOperationException("An Application has already been created by another class!!");

            var t = new Thread(applicationJob);

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            lock (this) while (app == null) Monitor.Wait(this);



        }

        private void applicationJob()
        {
            lock (this)
            {
                app = new Application();
                Monitor.Pulse(this);
            }



            app.Run();
        }

        public void CreateClassForm<T>(T dataContext)
        {
            startApplication();
            forms.Add((IClassForm)app.Dispatcher.Invoke(new Func<T, IClassForm>(createAndShowForm), dataContext));
        }

        private IClassForm createAndShowForm<T>(T dataContext)
        {
            var f = new ClassForm<T>();
            f.DataContext = dataContext;
            f.Show();

            return f;
        }

        public void Initialize(IXNAGame _game)
        {

        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
            foreach (var f in forms)
            {
                f.WriteDataContext();
                f.ReadDataContext();
            }
        }
    }
}

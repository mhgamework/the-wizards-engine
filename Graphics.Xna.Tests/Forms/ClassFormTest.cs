using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MHGameWork.TheWizards.Forms;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Wpf;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Data.Forms
{
    [TestFixture]
    public class ClassFormTest
    {
        [Test]
        public void TestWpfWindow()
        {


            var ev = new AutoResetEvent(false);
            Application app = null;
            var t = new Thread(delegate()
            {
                app = new Application();
                var w = new Window();
                w.Show();

                app.Run();

            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            t.Join();

        }

        [Test]
        public void TestXNAGameWpfWindow()
        {
            var w = new XNAGameWpf();
            var win = w.CreateWindow();
            Application.Current.Dispatcher.Invoke(new Action( win.Show ));

            while (true)
            {
                
            }
        }


        [Test]
        public void TestXNAGameWpfEditBoxMesh()
        {

            var game = new XNAGame();
            var mesh = new BoxMesh();
            game.AddXNAObject(mesh);

            game.Wpf.CreateClassForm(mesh);

            game.Run();
        }


        [Test]
        public void TestSimpleFormRead()
        {
            var form = new ClassForm<TestClass>();


            var t = new TestClass();
            t.Text = "The Text";
            t.Number = 42;

            form.DataContext = t;


            form.Show();

            var app = new Application();
            app.Run();
        }

        [Test]
        public void TestEditSphereMesh()
        {
            ClassForm<SphereMesh> form = null;




            var game = new XNAGame();
            var mesh = new SphereMesh();
            game.AddXNAObject(mesh);





            var ev = new AutoResetEvent(false);
            Application app = null;
            var t = new Thread(delegate()
            {
                app = new Application();
                form = new ClassForm<SphereMesh>();

                form.DataContext = mesh;

                form.Show();
                ev.Set();
                app.Run();
                
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            ev.WaitOne();
            game.UpdateEvent += delegate
                                    {
                                        
                                        form.WriteDataContext();
                                        form.ReadDataContext();
                                    };

            game.Run();
        }

        [Test]
        public void TestEditBoxMesh()
        {
            ClassForm<BoxMesh> form = null;




            var game = new XNAGame();
            var mesh = new BoxMesh();
            game.AddXNAObject(mesh);





            var ev = new AutoResetEvent(false);
            Application app = null;
            var t = new Thread(delegate()
            {
                app = new Application();
                form = new ClassForm<BoxMesh>();

                form.DataContext = mesh;

                form.Show();
                ev.Set();
                app.Run();

            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            ev.WaitOne();
            game.UpdateEvent += delegate
            {

                form.WriteDataContext();
                form.ReadDataContext();
            };

            game.Run();
        }



        private class TestClass
        {
            public string Text;
            public int Number;
        }
    }
}

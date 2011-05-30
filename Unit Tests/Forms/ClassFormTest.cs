using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MHGameWork.TheWizards.Forms;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Forms
{
    [TestFixture]
    public class ClassFormTest
    {
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
            var form = new ClassForm<SphereMesh>();
        

            var game = new XNAGame();
            var mesh = new SphereMesh();
            game.AddXNAObject(mesh);

            game.UpdateEvent += delegate
                                    {
                                        form.WriteDataContext();
                                        form.ReadDataContext();
                                    };

            form.DataContext = mesh;


            form.Show();

            game.Run();
        }




        private class TestClass
        {
            public string Text;
            public int Number;
        }
    }
}

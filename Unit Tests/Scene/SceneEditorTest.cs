using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MHGameWork.TheWizards.Editor.Scene;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Scene;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Editor.Scene
{
    [TestFixture]
    public class SceneEditorTest
    {
        [Test]
        public void TestSceneEditor()
        {
            SimpleEditorForm form = null;
            var game = new XNAGame();
            game.Mouse.CursorEnabled = true;
            game.IsMouseVisible = true;
            game.DrawFps = true;
            game.SpectaterCamera.Enabled = false;
            var ev = new AutoResetEvent(false);
            var t = new Thread(delegate()
                                   {
                                       form = new SimpleEditorForm();
                                       var app = new Application();
                                       ev.Set();
                                       app.Run(form);
                                   });
            t.SetApartmentState(ApartmentState.STA);




            t.Start();
            game.InitializeEvent +=
                delegate
                {
                    int left = game.GetWindowForm().Location.X + game.GetWindowForm().Size.Width;
                    int top = game.GetWindowForm().Location.Y;
                    ev.WaitOne();
                    form.Dispatcher.Invoke(new Action(delegate
                                               {
                                                   form.Left = left + 8;
                                                   form.Top = top;
                                               }), null);

                };
            game.Run();

            //            t.Join();

        }


    }
}

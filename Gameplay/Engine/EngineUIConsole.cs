using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MHGameWork.TheWizards.Engine
{
    public class EngineUIConsole
    {
        private Window window;

        public EngineUIConsole()
        {
            var ev = new AutoResetEvent(false);
            //TODO: move this to the engine!!!!
            if (Application.Current == null)
            {

                var t = new Thread(delegate()
                    {
                        var app = new Application();
                        app.Dispatcher.BeginInvoke(() => ev.Set());
                        app.Run();
                    });
                t.IsBackground = true;
                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                ev.WaitOne();


            }
            Application.Current.Dispatcher.BeginInvoke(createOutputWindow);
        }

        private void createOutputWindow()
        {
            window = new Window();
            window.WindowStyle = WindowStyle.None;

            window.Height = 200;
            TW.Graphics.Form.Form.Move +=
                delegate { window.Dispatcher.BeginInvoke(updatePositionAndSize); };
            TW.Graphics.Form.Form.GotFocus +=
                delegate { window.Dispatcher.BeginInvoke(delegate { window.Topmost = true; }); };
            TW.Graphics.Form.Form.LostFocus +=
                delegate { window.Dispatcher.BeginInvoke(delegate { window.Topmost = false; }); };


            var panel = new DockPanel();
            var list = new ListBox();
            list.Items.Add("ello");

            panel.Children.Add(list);

            window.Content = panel;
            updatePositionAndSize();
            window.Show();


            Console.SetOut(new Writer(list, Console.Out));


            Console.WriteLine("Console attached!");
            //while (true)
            //    Thread.Sleep(2000);
        }

        private void updatePositionAndSize()
        {
            window.Left = TW.Graphics.Form.Form.Left;
            window.Top = TW.Graphics.Form.Form.Top + TW.Graphics.Form.Form.Height;
            window.Width = TW.Graphics.Form.Form.Width;
        }

        private class Writer : TextWriter
        {
            private readonly ListBox box;
            private readonly TextWriter originalWriter;

            private string remainders;

            public Writer(ListBox box, TextWriter originalWriter)
            {
                this.box = box;
                this.originalWriter = originalWriter;
            }

            public override void Write(char[] buffer, int index, int count)
            {
                base.Write(buffer, index, count);
                originalWriter.Write(buffer, index, count);
                var newStr = new String(buffer);
                remainders += newStr;
                while (remainders.Contains("\r\n"))
                {
                    var line = remainders.Substring(0, remainders.IndexOf("\r\n"));
                    remainders = remainders.Substring(remainders.IndexOf("\r\n") + 2);

                    box.Dispatcher.BeginInvoke(() => box.Items.Add(line));
                    box.Dispatcher.BeginInvoke(() => box.ScrollIntoView(line));


                }

            }



            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}
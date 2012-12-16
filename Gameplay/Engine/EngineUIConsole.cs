using System;
using System.IO;
using System.Text;
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
            createOutputWindow();
        }

        private void createOutputWindow()
        {
            window = new Window();
            window.WindowStyle = WindowStyle.None;

            window.Height = 200;
            TW.Graphics.Form.Form.Move += (o, args) => updatePositionAndSize();


            var panel = new DockPanel();
            var list = new ListBox();
            list.Items.Add("ello");

            panel.Children.Add(list);

            window.Content = panel;
            updatePositionAndSize();
            window.Show();


            Console.SetOut(new Writer(list, Console.Out));


            Console.WriteLine("Console attached!");
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

            public Writer(ListBox box, TextWriter originalWriter)
            {
                this.box = box;
                this.originalWriter = originalWriter;
            }

            public override void Write(char[] buffer, int index, int count)
            {
                base.Write(buffer, index, count);
                originalWriter.Write(buffer,index,count);
                box.Dispatcher.BeginInvoke(() => box.Items.Add(new String(buffer)));

            }


            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}
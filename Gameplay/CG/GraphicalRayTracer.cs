using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ComputerGraphics.Math;

namespace MHGameWork.TheWizards.CG
{
    public class GraphicalRayTracer
    {
        public GraphicalRayTracer()
        {
            CreateAndShowMainWindow();
        }

        Window mainWindow;

        private void CreateAndShowMainWindow()
        {
            // Create the application's main window
            mainWindow = new Window();
            mainWindow.Title = "Writeable Bitmap";
            mainWindow.Height = 200;
            mainWindow.Width = 200;

            // Define the Image element
            _random.Stretch = Stretch.None;
            _random.Margin = new Thickness(20);

            // Define a StackPanel to host Controls
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Orientation = Orientation.Vertical;
            myStackPanel.Height = 200;
            myStackPanel.VerticalAlignment = VerticalAlignment.Top;
            myStackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            // Add the Image to the parent StackPanel
            myStackPanel.Children.Add(_random);

            // Add the StackPanel as the Content of the Parent Window Object
            mainWindow.Content = myStackPanel;
            mainWindow.Show();

            dispatcherTimer_Tick(null, null);
            // DispatcherTimer setup
            // The DispatcherTimer will be used to update _random every
            //    second with a new random set of colors.
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.IsEnabled = true;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Start();
            var app = new Application();
            app.Run(mainWindow);


            var rect = new Int32Rect(0, 0, 100, 100);


        }
        //  System.Windows.Threading.DispatcherTimer.Tick handler
        //
        //  Updates the Image element with new random colors
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TracerResult result;
            while (results.TryDequeue(out result))
            {
                //Update writeable bitmap with the colorArray to the image.
                _wb.WritePixels(result.Rectangle, result.ColorArray, _stride, 0);
            }
            _random.Source = _wb;

        }

        private Image _random = new Image();
        // Create the writeable bitmap will be used to write and update.
        private static WriteableBitmap _wb =
            new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgra32, null);
        // Define the rectangle of the writeable image we will modify. 
        // The size is that of the writeable bitmap.
        private static Int32Rect _rect = new Int32Rect(0, 0, _wb.PixelWidth, _wb.PixelHeight);
        // Calculate the number of bytes per pixel. 
        private static int _bytesPerPixel = (_wb.Format.BitsPerPixel + 7) / 8;
        // Stride is bytes per pixel times the number of pixels.
        // Stride is the byte width of a single rectangle row.
        private static int _stride = _wb.PixelWidth * _bytesPerPixel;

        // Create a byte array for a the entire size of bitmap.
        private static int _arraySize = _stride * _wb.PixelHeight;
        private static byte[] _colorArray = new byte[_arraySize];


        private ConcurrentQueue<TracerResult> results = new ConcurrentQueue<TracerResult>();

        private void tracerJob(IRayTracer tracer, List<Int32Rect> rectangles)
        {
            foreach (var rect in rectangles)
            {
                byte[] data = new byte[rect.Width * rect.Height * 4];
                int iData = 0;
                for (int x = rect.X; x < rect.X + rect.Width; x++)
                    for (int y = rect.Y; y < rect.Y + rect.Height; y++)
                    {
                        var color = tracer.GetPixel(new Point2(x, y));
                        data[iData + 0] = (byte)(color.Blue*255);
                        data[iData + 1] = (byte)(color.Green * 255);
                        data[iData + 2] = (byte)(color.Red * 255);
                        data[iData + 3] = 255;
                        iData += 4;
                    }
            }
        }

        class TracerResult
        {
            public Int32Rect Rectangle;
            public byte[] ColorArray;
        }

    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MHGameWork.TheWizards.CG.Math;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class GraphicalRayTracer
    {
        private static int windowSize = 500;

        private readonly IRayTracer tracer;

        public GraphicalRayTracer(IRayTracer tracer)
        {
            this.tracer = tracer;
            CreateAndShowMainWindow();
        }

        Window mainWindow;

        private void CreateAndShowMainWindow()
        {
            // Create the application's main window
            mainWindow = new Window();
            mainWindow.Title = "Writeable Bitmap";
            mainWindow.Height = windowSize;
            mainWindow.Width = windowSize;

            // Define the Image element
            _random.Stretch = Stretch.None;
            //_random.Margin = new Thickness(20);

            // Define a StackPanel to host Controls
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Orientation = Orientation.Vertical;
            myStackPanel.Height = windowSize;
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
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(windowSize);
            dispatcherTimer.Start();



            var rect = new Int32Rect(0, 0, windowSize, windowSize);

            var list = new List<Int32Rect>();
            int size = 8;
            var y = 0;
            var x = 0;
            for (; y < rect.Width - size; y += size)
            {
                x = 0;
                for (; x < rect.Height - size; x += size)
                {
                    list.Add(new Int32Rect(x, y, size, size));
                }
                list.Add(new Int32Rect(x, y, rect.Width - x, size));
            }
            x = 0;
            for (; x < rect.Height - size; x += size)
            {
                list.Add(new Int32Rect(x, y, size, rect.Height - y));
            }
            list.Add(new Int32Rect(x, y, rect.Width - x, rect.Height - y));

            var numThreads = 4;
            for (int iThread = 0; iThread < numThreads; iThread++)
            {
                int thread = iThread;
                queueThread(list.Where((r, i) => (i % numThreads == thread)));
            }


            var app = new Application();
            app.Run(mainWindow);


        }

        private void queueThread(IEnumerable<Int32Rect> list)
        {
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 tracerJob(tracer, list);
                                             });
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
                _wb.WritePixels(new Int32Rect(0, 0, result.Rectangle.Width, result.Rectangle.Height), result.ColorArray, _bytesPerPixel * result.Rectangle.Width, result.Rectangle.X, result.Rectangle.Y);
            }
            _random.Source = _wb;

        }

        private Image _random = new Image();
        // Create the writeable bitmap will be used to write and update.
        private static WriteableBitmap _wb =
            new WriteableBitmap(windowSize, windowSize, 96, 96, PixelFormats.Bgra32, null);
        // Define the rectangle of the writeable image we will modify. 
        // The size is that of the writeable bitmap.
        private static Int32Rect _rect = new Int32Rect(0, 0, _wb.PixelWidth, _wb.PixelHeight);
        private static int _bytesPerPixel = (_wb.Format.BitsPerPixel + 7) / 8;
        // Stride is bytes per pixel times the number of pixels.
        // Stride is the byte width of a single rectangle row.
        private static int _stride = _wb.PixelWidth * _bytesPerPixel;


        private ConcurrentQueue<TracerResult> results = new ConcurrentQueue<TracerResult>();

        private void tracerJob(IRayTracer tracer, IEnumerable<Int32Rect> rectangles)
        {
            foreach (var rect in rectangles)
            {
                var resolution = new Point2(windowSize, windowSize);

                byte[] data = new byte[rect.Width * rect.Height * 4];
                int iData = 0;
                for (int y = rect.Y; y < rect.Y + rect.Height; y++)
                for (int x = rect.X; x < rect.X + rect.Width; x++)
                    {
                        var color = tracer.GetPixel(new Vector2((x + 0.5f) / resolution.X, (y + 0.5f) / resolution.Y));
                        data[iData + 0] = (byte)(color.Blue * 255);
                        data[iData + 1] = (byte)(color.Green * 255);
                        data[iData + 2] = (byte)(color.Red * 255);
                        data[iData + 3] = 255;
                        iData += 4;
                    }

                results.Enqueue(new TracerResult { Rectangle = rect, ColorArray = data });
            }
        }

        class TracerResult
        {
            public Int32Rect Rectangle;
            public byte[] ColorArray;
        }

    }
}

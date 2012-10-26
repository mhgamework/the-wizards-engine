using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly IRayTracer tracer;

        private Point2 windowSize;

        public GraphicalRayTracer(IRayTracer tracer)
        {
            

            windowSize = new Point2(1280, 720);
            this.tracer = new CachedTracer(windowSize, tracer);
            

            _wb = new WriteableBitmap(windowSize.X, windowSize.Y, 96, 96, PixelFormats.Bgra32, null);

            
            _rect = new Int32Rect(0, 0, _wb.PixelWidth, _wb.PixelHeight);
            _bytesPerPixel = (_wb.Format.BitsPerPixel + 7) / 8;
            _stride = _wb.PixelWidth * _bytesPerPixel;


            CreateAndShowMainWindow();
        }

        Window mainWindow;

        private void CreateAndShowMainWindow()
        {
            // Create the application's main window
            mainWindow = new Window();
            mainWindow.Title = "Writeable Bitmap";
            mainWindow.Height = windowSize.Y;
            mainWindow.Width = windowSize.X;

            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            // Define the Image element
            _random.Stretch = Stretch.Fill;
            //_random.Margin = new Thickness(20);

            // Define a StackPanel to host Controls
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Orientation = Orientation.Vertical;
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


            var t = new Thread(fillThreadJob);
            t.IsBackground = true;
            t.Start();



            var app = new Application();
            app.Run(mainWindow);


        }

        private void processTasks()
        {
            if (workersRunning != 0)
                throw new InvalidOperationException();
            var numThreads = 4;
            workersRunning = numThreads;

            for (int iThread = 0; iThread < numThreads; iThread++)
            {
                int thread = iThread;
                queueThread();
            }

            lock (this)
                while (workersRunning > 0) Monitor.Wait(this);
        }

        private void fillThreadJob()
        {
            var rect = new Int32Rect(0, 0, windowSize.X, windowSize.Y);

            int currentResolution = 1;
            while (currentResolution * 2 < windowSize.X && currentResolution * 2 < windowSize.Y)
                currentResolution *= 2;

            var pixelsPerRectangle = 8;

            currentResolution /= 8;

            while (currentResolution > 0)
            {
                foreach (var task in buildSubRectangles(rect, currentResolution * pixelsPerRectangle).Select(r => new Task(r, currentResolution)))
                {
                    tasks.Enqueue(task);
                    if (task.Rectangle.Height < 0) Debugger.Break();
                }
                currentResolution /= 2;


                processTasks();
            }
        }

        private IEnumerable<Int32Rect> buildSubRectangles(Int32Rect sourceRect, int size)
        {
            var y = 0;
            var x = 0;
            for (; y < sourceRect.Height - size; y += size)
            {
                x = 0;
                for (; x < sourceRect.Width - size; x += size)
                {
                    yield return new Int32Rect(x, y, size, size);
                }
                yield return new Int32Rect(x, y, sourceRect.Width - x, size);
            }
            x = 0;
            for (; x < sourceRect.Width - size; x += size)
            {
                yield return new Int32Rect(x, y, size, sourceRect.Height - y);
            }
            yield return new Int32Rect(x, y, sourceRect.Width - x, sourceRect.Height - y);
        }

        private volatile int workersRunning;

        private void queueThread()
        {
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 tracerJob(tracer);
                                                 workersRunning--;
                                                 lock (this) Monitor.PulseAll(this);
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
        private WriteableBitmap _wb;

        // Define the rectangle of the writeable image we will modify. 
        // The size is that of the writeable bitmap.
        private Int32Rect _rect;
        private int _bytesPerPixel;
        // Stride is bytes per pixel times the number of pixels.
        // Stride is the byte width of a single rectangle row.
        private int _stride;


        private ConcurrentQueue<Task> tasks = new ConcurrentQueue<Task>();
        private ConcurrentQueue<TracerResult> results = new ConcurrentQueue<TracerResult>();

        private object tasksLock = new object();

        private void tracerJob(IRayTracer tracer)
        {
            Task task;
            while (tasks.TryDequeue(out task))
            {

                Int32Rect rect = task.Rectangle;

                var resolution = windowSize;

                byte[] data = new byte[rect.Width * rect.Height * 4];
                int iData = 0;
                for (int y = rect.Y; y < rect.Y + rect.Height; y += 1)
                    for (int x = rect.X; x < rect.X + rect.Width; x += 1)
                    {
                        int tempX = x - (x % task.Resolution) + (task.Resolution / 2);
                        int tempY = y - (y % task.Resolution) + (task.Resolution / 2);
                        var color = tracer.GetPixel(new Vector2((tempX + 0.5f) / resolution.X, (tempY + 0.5f) / resolution.Y));
                        if (color.Blue < 0) color.Blue = 0;
                        if (color.Blue > 1) color.Blue = 1;
                        if (color.Green < 0) color.Green = 0;
                        if (color.Green > 1) color.Green = 1;
                        if (color.Red < 0) color.Red = 0;
                        if (color.Red > 1) color.Red = 1;
                        if (color.Alpha < 0) color.Alpha = 0;
                        if (color.Alpha > 1) color.Alpha = 1;


                        data[iData + 0] = (byte)(color.Blue * 255);
                        data[iData + 1] = (byte)(color.Green * 255);
                        data[iData + 2] = (byte)(color.Red * 255);
                        data[iData + 3] = 255;
                        iData += 4;
                    }

                results.Enqueue(new TracerResult { Rectangle = rect, ColorArray = data, Resolution = task.Resolution });
            }
        }

        struct Task
        {
            public Int32Rect Rectangle;
            public int Resolution;

            public Task(Int32Rect rectangle, int resolution)
            {
                Rectangle = rectangle;
                Resolution = resolution;
            }

            public override string ToString()
            {
                return string.Format("Rectangle: {0}, Resolution: {1}", Rectangle, Resolution);
            }
        }


        class TracerResult
        {
            public Int32Rect Rectangle;
            public byte[] ColorArray;
            public int Resolution;
        }

        class CachedTracer : IRayTracer
        {
            private readonly Point2 _size;
            private readonly IRayTracer _tracer;

            private Color4[,] cache;
            private bool[,] cached;

            public CachedTracer(Point2 size, IRayTracer tracer)
            {
                _size = size;
                _tracer = tracer;
                cache = new Color4[size.X * 2, size.Y * 2];
                cached = new bool[size.X * 2, size.Y * 2];
            }

            public Color4 GetPixel(Vector2 pos)
            {
                var x = (int)(pos.X * _size.X);
                var y = (int)(pos.Y * _size.Y);

                if (cached[x, y])
                    return cache[x, y];


                var ret = _tracer.GetPixel(pos);
                cache[x, y] = ret;
                cached[x, y] = true;

                return ret;

            }
        }

    }
}

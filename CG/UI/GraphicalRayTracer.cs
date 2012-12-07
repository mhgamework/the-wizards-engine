using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.WinAPI;

namespace MHGameWork.TheWizards.CG.UI
{
    public class GraphicalRayTracer
    {

        private IRenderedImage tracer;

        private Point2 windowSize;

        public GraphicalRayTracer(IRenderedImage tracer, int numThreads = 4)
        {
            WindowSize = new Point2(1800, 1000);
            //windowSize = new Point2(600, 400);
            MinResolution = 1;

            Run(tracer, numThreads);

        }
        public GraphicalRayTracer()
        {
            WindowSize = new Point2(1800, 1000);
            //windowSize = new Point2(600, 400);
            MinResolution = 1;


        }


        /// <summary>
        /// Must be power of two!!!
        /// </summary>
        public int MinResolution
        {
            get { return minResolution; }
            set { minResolution = value; }
        }

        public Point2 WindowSize
        {
            get { return windowSize; }
            set { windowSize = value; }
        }

        public void Run(IRenderedImage tracer, int numThreads = 4)
        {
            this.numThreads = numThreads;
            originalTracer = tracer;
            this.tracer = new CachedTracer(WindowSize, tracer);

            _wb = new WriteableBitmap(WindowSize.X, WindowSize.Y, 96, 96, PixelFormats.Bgra32, null);


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

            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            mainWindow.Background = Brushes.Red;
            mainWindow.Left = 0;
            if (Win32.GetSystemMetrics(SystemMetric.SM_CMONITORS) == 2)
                mainWindow.Left = 1920;
            mainWindow.Top = 0;
            mainWindow.Width = 1920;
            mainWindow.Height = 1080;



            // Define the Image element
            image.Stretch = Stretch.None;
            image.Width = WindowSize.X;
            image.Height = WindowSize.Y;
            image.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(image_MouseLeftButtonDown);
            image.MouseMove += new MouseEventHandler(image_MouseMove);
            //_random.Margin = new Thickness(20);

            // Define a StackPanel to host Controls
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Orientation = Orientation.Vertical;
            myStackPanel.VerticalAlignment = VerticalAlignment.Top;
            myStackPanel.HorizontalAlignment = HorizontalAlignment.Center;


            // Add the Image to the parent StackPanel
            myStackPanel.Children.Add(image);

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

        void image_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(image);
            var screenPos = CalculateTracerPixelPos((int)pos.X, (int)pos.Y, MinResolution);
            Color4 col = tracer.GetPixel(screenPos);
            mainWindow.Background = new SolidColorBrush(Color.FromRgb((byte)(col.Red * 255), (byte)(col.Green * 255), (byte)(col.Blue * 255)));

        }

        void image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(image);
            var screenPos = CalculateTracerPixelPos((int)pos.X, (int)pos.Y, MinResolution);
            originalTracer.GetPixel(screenPos);
            /*results.Enqueue(new TracerResult
                                {
                                    ColorArray = new byte[] { 0, 0, 255, 255 },
                                    Rectangle = new Int32Rect((int)pos.X, (int)pos.Y, 1, 1),
                                    Resolution = 1
                                });*/
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }


        private void processTasks()
        {
            if (workersRunning != 0)
                throw new InvalidOperationException();
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
            var rect = new Int32Rect(0, 0, WindowSize.X, WindowSize.Y);

            int currentResolution = 1;
            while (currentResolution * 2 < WindowSize.X && currentResolution * 2 < WindowSize.Y)
                currentResolution *= 2;

            var pixelsPerRectangle = 8;

            currentResolution /= pixelsPerRectangle;


            while (currentResolution >= MinResolution)
            {
                foreach (var task in buildSubRectangles(rect, currentResolution * pixelsPerRectangle).Select(r => new Task(r, currentResolution)))
                {
                    tasks.Enqueue(task);
                    if (task.Rectangle.Height < 0) Debugger.Break();
                }
                currentResolution = currentResolution >> 2;


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
            image.Source = _wb;

        }

        private Image image = new Image();
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
        private IRenderedImage originalTracer;
        private int numThreads;
        private int minResolution;

        private void tracerJob(IRenderedImage tracer)
        {
            Task task;
            while (tasks.TryDequeue(out task))
            {

                Int32Rect rect = task.Rectangle;

                byte[] data = new byte[rect.Width * rect.Height * 4];
                int iData = 0;
                for (int y = rect.Y; y < rect.Y + rect.Height; y += 1)
                    for (int x = rect.X; x < rect.X + rect.Width; x += 1)
                    {
                        Vector2 pos = CalculateTracerPixelPos(x, y, task.Resolution);
                        var color = tracer.GetPixel(pos);
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

        private Vector2 CalculateTracerPixelPos(int x, int y, int resolution)
        {
            int tempX = x - (x % resolution) + (resolution / 2);
            int tempY = y - (y % resolution) + (resolution / 2);
            return new Vector2((tempX + 0.5f) / WindowSize.X, (tempY + 0.5f) / WindowSize.Y);
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

        class CachedTracer : IRenderedImage
        {
            private readonly Point2 _size;
            private readonly IRenderedImage _tracer;

            private Color4[,] cache;
            private bool[,] cached;

            public CachedTracer(Point2 size, IRenderedImage tracer)
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


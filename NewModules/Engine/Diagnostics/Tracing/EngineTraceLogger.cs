using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace MHGameWork.TheWizards.Engine.Diagnostics.Tracing
{
    /// <summary>
    /// Responsible for logging engine traces and storing them to the disk on a different thread.
    /// </summary>
    public class EngineTraceLogger : ITraceLogger
    {
        private FileInfo traceFile = new FileInfo("EngineTrace.txt");

        public EngineTraceLogger()
        {
            if (traceFile.Exists) traceFile.Delete();

            using (var fs = traceFile.CreateText())
                fs.WriteLine("Starting log!");

            var t = new Thread(saveQueue);
            t.Name = "EngineTraceLogger";
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        /// Runs in the other thread
        /// </summary>
        private void saveQueue()
        {
            for (; ; )
            {
                using (var fs = traceFile.AppendText())
                {
                    string result;
                    while (queue.TryDequeue(out result))
                    {
                        fs.WriteLine(result);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private string lastLine;
        private int lastcount = 0;
        public void Log(string log)
        {

            if (lastLine == log)
                lastcount++;
            else
            {
                queue.Enqueue(lastLine + (lastcount == 1 ? "" : " - " + lastcount + " times"));
                lastcount = 1;
                lastLine = log;
            }
        }
    }
}
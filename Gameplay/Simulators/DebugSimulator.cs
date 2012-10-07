using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Profiling;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for enabling debug functionality in the engine
    /// </summary>
    public class DebugSimulator : ISimulator
    {
        private Textarea profilerText;



        public DebugSimulator()
        {
            profilerText = new Textarea();
            profilerText.Position = new SlimDX.Vector2(20, 20);
            profilerText.Size = new SlimDX.Vector2(700, 500);

            TW.Graphics.GameLoopProfilingPoint.AddProfileCompleteCallback(onProfileComplete);

            profileCompletePoint = Profiler.CreateElement("DebugSimulator.onProfileComplete");

            worker = new BackgroundWorker();
            worker.DoWork += saveProfileDataToDisk;

            profileFile = TWDir.Test + "\\ProfileOutput.txt";

        }

        private void saveProfileDataToDisk(object sender, DoWorkEventArgs e)
        {
            string[] results = (string[]) e.Argument;

            using (var fs = new StreamWriter(File.OpenWrite(profileFile)))
                foreach (var str in results)
                    fs.WriteLine(str);

        }


        private List<string> cachedResults = new List<string>();

        /// <summary>
        /// This gets called outside of the main gameloop, so its not profiled
        /// </summary>
        /// <param name="obj"></param>
        private void onProfileComplete(ProfilingPoint obj)
        {
            profileCompletePoint.Begin();

            if (cachedResults.Count > 100 && !worker.IsBusy)
            {
                var results = cachedResults.ToArray();
                cachedResults.Clear();
                worker.RunWorkerAsync(results);
            }


            var str = obj.GenerateProfileString(p => p.AverageSeconds > 0.0001f);

            profileCompletePoint.End();

            var overhead = profileCompletePoint.AverageSeconds * 1000;
            str += overhead.ToString("#0.#ms");

            lastresult = str;
            cachedResults.Add(str);


        }

        private float time;
        private string lastresult;
        private ProfilingPoint profileCompletePoint;
        private BackgroundWorker worker;
        private string profileFile;

        public void Simulate()
        {
            time += TW.Graphics.Elapsed;
            updateProfilerText();
        }

        private void updateProfilerText()
        {
            if (time < 1)
                return;

            time = 0;

            profilerText.Text = lastresult;
        }
    }
}

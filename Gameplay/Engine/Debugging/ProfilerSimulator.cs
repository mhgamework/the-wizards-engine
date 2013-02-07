using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Profiling;

namespace MHGameWork.TheWizards.Engine.Debugging
{
    /// <summary>
    /// Responsible for processing Profiler information in the engine
    /// </summary>
    public class ProfilerSimulator : ISimulator
    {
        private Textarea profilerText;

        private Data data = TW.Data.GetSingleton<Data>();


        public ProfilerSimulator()
        {


            TW.Graphics.GameLoopProfilingPoint.Ended += () => onProfileComplete(TW.Graphics.GameLoopProfilingPoint);

            profileCompletePoint = Profiler.CreateElement("ProfilerSimulator.onProfileComplete");

            worker = new BackgroundWorker();
            worker.DoWork += saveProfileDataToDisk;

            profileFile = TWDir.Test + "\\ProfileOutput.txt";

        }

        private void saveProfileDataToDisk(object sender, DoWorkEventArgs e)
        {
            string[] results = (string[])e.Argument;

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


            var targetName = "DeferredMeshRenderer.Draw";
            var targetPoint = obj.FindByName(targetName);
            targetPoint = null;
            float minDuration = 0.0005f;


            string str = "";
            if (targetPoint == null)
                targetPoint = obj;


            //minDuration = targetPoint.AverageSeconds / 10f;
            minDuration = 0.0001f;

            //str = targetPoint.GenerateProfileString(p => p.AverageSeconds > minDuration);

            profileCompletePoint.End();

            //var overhead = profileCompletePoint.AverageSeconds * 1000;
            //str += overhead.ToString("#0.#ms");

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

            if (data.Textarea == null)
            {
                profilerText = new Textarea();
                profilerText.Position = new SlimDX.Vector2(20, 20);
                profilerText.Size = new SlimDX.Vector2(700, 500);
                data.Textarea = profilerText;
            }
            profilerText = data.Textarea;

            time += TW.Graphics.Elapsed;
            updateProfilerText();
        }

        private void updateProfilerText()
        {
            if (time < 1)
                return;

            time = 0;

            //profilerText.Text = lastresult;
        }

        [ModelObjectChanged]
        public class Data : EngineModelObject
        {
            public Textarea Textarea { get; set; }
        }
    }
}

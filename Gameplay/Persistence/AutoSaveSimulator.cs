using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// This class saves the model every interval
    /// </summary>
    public class AutoSaveSimulator : ISimulator
    {
        private readonly string file;
        private readonly TimeSpan interval;
        private readonly ModelSerializer serializer;

        private DateTime lastSave;

        /// <summary>
        /// TODO: these constructor arguments should maybe be in the model??
        /// </summary>
        /// <param name="file"></param>
        /// <param name="interval"></param>
        /// <param name="?"></param>
        public AutoSaveSimulator(string file, TimeSpan interval, ModelSerializer serializer)
        {
            this.file = file;
            this.interval = interval;
            this.serializer = serializer;

            lastSave = DateTime.Now;
        }

        public void Simulate()
        {
            if (lastSave + interval > DateTime.Now)
                return;

            using (var fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fs))
                serializer.Serialize(TW.Model, writer);

            lastSave = DateTime.Now;

        }
    }
}

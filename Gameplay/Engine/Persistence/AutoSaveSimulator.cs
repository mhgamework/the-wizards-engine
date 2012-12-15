using System;
using System.IO;
using MHGameWork.TheWizards.Persistence;

namespace MHGameWork.TheWizards.Engine.Persistence
{
    /// <summary>
    /// This class saves the model every interval
    /// TODO: fix zis
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
                serializer.Serialize(TW.Data, writer);

            lastSave = DateTime.Now;

        }
    }
}

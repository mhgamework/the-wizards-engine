using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine.Persistence
{
    /// <summary>
    /// Responsible for persisting the state of the engine to a file and back
    /// </summary>
    public class EngineStatePersistor
    {
        private string filename;

        public EngineStatePersistor()
        {
            filename = "EngineState.txt";
        }

        public void SaveEngineState()
        {
            foreach (IModelObject obj in TW.Data.Objects)
                TW.Data.ModelSerializer.QueueForSerialization(obj);

            using (var fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fs))
            TW.Data.ModelSerializer.Serialize(writer);
        }
        public void LoadEngineState()
        {
            if (!File.Exists(filename)) return;
            using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
            using (var r = new StreamReader(fs))
                TW.Data.ModelSerializer.Deserialize(r);
        }
    }
}

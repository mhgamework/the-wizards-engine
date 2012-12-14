using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for storing and accessing persistent data in the wizards (Database)
    /// </summary>
    [ModelObjectChanged]
    public class Datastore : EngineModelObject
    {
        private ModelSerializer modelSerializer;
        public List<IModelObject> Objects { get; set; }

        public Datastore()
        {
            Objects = new List<IModelObject>();


            var stringSerializer = StringSerializer.Create();
            stringSerializer.AddConditional(new FilebasedAssetSerializer());
            modelSerializer = new Persistence.ModelSerializer(stringSerializer);
        }

        /// <summary>
        /// Removes all objects in the datastore (also deletes the objects!!)
        /// </summary>
        public void Clear()
        {
            foreach (var obj in Objects)
            {
                TW.Data.RemoveObject(obj);
            }
        }
        public void LoadFromFile(FileInfo file)
        {
            Clear();

            Datastore obj;
            using (var fs = file.OpenRead())
            using (var wr = new StreamReader(fs))
                modelSerializer.DeserializeAttributes(this, new SectionedStreamReader(wr));
        }

        public void SaveToFile(FileInfo file)
        {
            using (var fs = file.OpenWrite())
            using (var wr = new StreamWriter(fs))
                modelSerializer.Serialize(TW.Data, wr);
        }
    }
}

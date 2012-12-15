using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine.Persistence
{
    /// <summary>
    /// Responsible for storing and accessing persistent data in the wizards (Database)
    /// </summary>
    [ModelObjectChanged]
    public class Datastore : EngineModelObject
    {
        public List<IModelObject> Objects { get; set; }

        public Datastore()
        {
            Objects = new List<IModelObject>();
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

            
            List<IModelObject> ret;
            using (var fs = file.OpenRead())
            using (var wr = new StreamReader(fs))
                ret = TW.Data.ModelSerializer .Deserialize(wr);

            var copyStore = ((Datastore) ret[0]);
            Objects = copyStore.Objects;
            TW.Data.Objects.Remove(copyStore);
        }

        public void SaveToFile(FileInfo file)
        {
            TW.Data.ModelSerializer.QueueForSerialization(this);
            using (var fs = file.OpenWrite())
            using (var wr = new StreamWriter(fs))
                TW.Data.ModelSerializer.Serialize(wr);

        }

        public void Persist(IModelObject ent)
        {
            if (Objects.Contains(ent))
                return;
            Objects.Add(ent);
        }
    }
}

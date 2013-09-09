using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;

namespace MHGameWork.TheWizards.Engine.CodeLoading
{
    /// <summary>
    /// Responsible for reloading the engines gameplay code, while preserving the state of all engine data
    /// </summary>
    public class PersistentCodeLoader : ICodeLoader
    {
        private TWEngine twEngine;

        public PersistentCodeLoader(TWEngine twEngine)
        {
            this.twEngine = twEngine;
        }


        public void Reload()
        {
            reloadGameplayDll();
            twEngine.CreateSimulators(); // reinitialize!
        }

        private void reloadGameplayDll()
        {
            //TODO: broke the persistance scope here, not sure whether it should be readded
            var persistentModels = TW.Data.Objects;
            //var persistentModels = TW.Data.Objects.Where(o => TW.Data.PersistentModelObjects.Contains(o));
            MemoryStream mem = null;
            try
            {
                mem = TW.Data.ModelSerializer.SerializeToStream(persistentModels);
                File.WriteAllBytes("temp.txt", mem.ToArray());
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Serialize engine data");
            }


            twEngine.ClearAllGameplayData();

            twEngine.updateActiveGameplayAssembly();

            try
            {
                if (mem != null)
                {
                    var objects = deserializeData(mem);
                    // The deserialized objects are added to TW.Data automatically
                    //foreach (var obj in objects)
                    //    TW.Data.AddObject(obj);
                }
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Deserialize engine data");
                return;
            }

            twEngine.ClearAllEngineState();


        }

        [PersistanceScope]
        private List<IModelObject> deserializeData(MemoryStream memoryStream)
        {
            TW.Data.InPersistenceScope = true;

            var ret = TW.Data.ModelSerializer.Deserialize(new StreamReader(memoryStream));

            TW.Data.InPersistenceScope = false;

            return ret;
        }

    }
}
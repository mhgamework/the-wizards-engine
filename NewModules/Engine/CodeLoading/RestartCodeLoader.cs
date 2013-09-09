using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine.CodeLoading
{
    /// <summary>
    /// Responsible for reloading the engines gameplay code, clears all old data.
    /// </summary>
    public class RestartCodeLoader : ICodeLoader
    {
        private TWEngine twEngine;

        public RestartCodeLoader(TWEngine twEngine)
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
            twEngine.ClearAllGameplayData();

            twEngine.updateActiveGameplayAssembly();

            twEngine.ClearAllEngineState();
        }

    }
}
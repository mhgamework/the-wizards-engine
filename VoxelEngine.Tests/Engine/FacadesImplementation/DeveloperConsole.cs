using MHGameWork.TheWizards.GodGame.Internal;

namespace VoxelEngine.Tests.Engine.FacadesImplementation
{
    public class DeveloperConsole : IDeveloperConsole
    {
        private DeveloperConsoleUI consoleUi;

        public DeveloperConsole( DeveloperConsoleUI consoleUi )
        {
            this.consoleUi = consoleUi;
        }

        public void WriteLine(string line)
        {
            consoleUi.WriteLine(line);
        }
    }
}
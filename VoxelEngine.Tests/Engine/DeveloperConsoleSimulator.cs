using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal;
using SlimDX.DirectInput;

namespace VoxelEngine.Tests.Engine
{
    public class DeveloperConsoleSimulator : ISimulator
    {
        private DeveloperConsoleUI consoleUi;
        private bool visible = false;

        public DeveloperConsoleSimulator(ICommandProvider commandProvider)
        {
            consoleUi = new DeveloperConsoleUI(commandProvider);

        }

        public void Simulate()
        {
            tryToggleConsole();
            consoleUi.Update();
        }


        private void tryToggleConsole()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Grave))
            {
                if (consoleUi.Visible)
                {
                    consoleUi.Hide();
                    TW.Graphics.ReleaseExclusiveKeyboardAccess();
                }
                else
                {
                    consoleUi.Show();
                    TW.Graphics.RequestExclusiveKeyboardAccess();
                }
            }
        }
    }
}
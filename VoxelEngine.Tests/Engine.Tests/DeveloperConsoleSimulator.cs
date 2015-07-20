using MHGameWork.TheWizards.GodGame.Internal;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.Tests
{
    public class DeveloperConsoleSimulator : ISimulator
    {
        public DeveloperConsoleUI ConsoleUi { get; private set; }
        private bool visible = false;

        public DeveloperConsoleSimulator(ICommandProvider commandProvider)
        {
            ConsoleUi = new DeveloperConsoleUI(commandProvider);

        }

        public void Simulate()
        {
            tryToggleConsole();
            ConsoleUi.Update();
        }


        private void tryToggleConsole()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Grave))
            {
                if (ConsoleUi.Visible)
                {
                    ConsoleUi.Hide();
                    TW.Graphics.ReleaseExclusiveKeyboardAccess();
                }
                else
                {
                    ConsoleUi.Show();
                    TW.Graphics.RequestExclusiveKeyboardAccess();
                }
            }
        }
    }
}
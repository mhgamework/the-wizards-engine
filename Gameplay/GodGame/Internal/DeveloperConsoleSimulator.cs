using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class DeveloperConsoleSimulator : ISimulator
    {
        private readonly PlayerInputSimulator inputSimulator;
        private DeveloperConsoleUI console;
        private bool consoleVisible = false;
        public DeveloperConsoleSimulator(PlayerInputSimulator inputSimulator, ICommandProvider provider)
        {
            this.inputSimulator = inputSimulator;
            console = new DeveloperConsoleUI(provider);
        }

        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Grave))
            {
                consoleVisible = !consoleVisible;
                if (consoleVisible)
                    console.Show();
                else
                    console.Hide();

                inputSimulator.UserInputDisabled = consoleVisible;
                TW.Graphics.SpectaterCamera.EnableUserInput = !consoleVisible;

                console.Update();

            }
            console.Update();
        }
    }
}
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Responsible for enabling/disabling and updating the developer console
    /// </summary>
    public class DeveloperConsoleService : ISimulator
    {
        private readonly UserInputProcessingService inputProcessingService;
        private DeveloperConsoleUI console;
        private bool consoleVisible = false;
        public DeveloperConsoleService(UserInputProcessingService inputProcessingService, ICommandProvider provider)
        {
            this.inputProcessingService = inputProcessingService;
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

                inputProcessingService.UserInputDisabled = consoleVisible;
                TW.Graphics.SpectaterCamera.EnableUserInput = !consoleVisible;

                console.Update();

            }
            console.Update();
        }
    }
}
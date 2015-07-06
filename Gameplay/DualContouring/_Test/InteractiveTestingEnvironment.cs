using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DualContouring._Test
{
    /// <summary>
    /// Engine class for providing an interactive testing environment
    /// Configuration class (=provides a convenient grouping of features)
    /// Provides
    ///     - World rendering
    ///     - Console
    /// </summary>
    public class InteractiveTestingEnvironment : ICommandProvider
    {
        private DeveloperConsoleUI console;

        public InteractiveTestingEnvironment()
        {
            console = new DeveloperConsoleUI(this);

        }

        public void LoadIntoEngine(TWEngine engine)
        {
            engine.AddSimulator(Update, "InteractiveEnvironmentUpdate");
        }

        private void Update()
        {
            tryToggleConsole();
            console.Update();
        }

        private void tryToggleConsole()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Grave))
            {
                if (console.Visible)
                {
                    console.Hide();
                    TW.Graphics.ReleaseExclusiveKeyboardAccess();
                }
                else
                {
                    console.Show();
                    TW.Graphics.RequestExclusiveKeyboardAccess();
                }
            }
        }

        string ICommandProvider.Execute(string command)
        {
            return "Unknown command";
        }

        string ICommandProvider.AutoComplete(string partialCommand)
        {
            return "Unkown";
        }
    }
}
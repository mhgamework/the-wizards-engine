using System;
using System.Runtime.InteropServices;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using NSubstitute;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [EngineTest]
    [TestFixture]
    public class DeveloperConsoleTest
    {
        private TWEngine engine;
        private DeveloperConsoleUI consoleUi;

        [SetUp]
        public void Setup()
        {
            engine = EngineFactory.CreateEngine();
            consoleUi = new DeveloperConsoleUI(createCommandProvider());

            var visible = true;
            consoleUi.Show();

            engine.AddSimulator(new BasicSimulator(() =>
            {
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.Grave))
                {
                    visible = !visible;
                    if (visible) consoleUi.Show();
                    else consoleUi.Hide();
                }
                consoleUi.Update();


            }));

            TW.Graphics.SpectaterCamera.EnableUserInput = false;
        }

        [Test]
        public void TestConsoleCommands()
        {


        }


        [Test]
        public void TestConsoleWriteInitial()
        {
            consoleUi.WriteLine("Hello");
            consoleUi.WriteLine("These");
            consoleUi.WriteLine("Are");
            consoleUi.WriteLine("5");
            consoleUi.WriteLine("lines!");
        }

        [Test]
        public void TestConsoleWrite()
        {
            engine.AddSimulator(new BasicSimulator(() =>
                {
                    consoleUi.WriteLine(TW.Graphics.TotalRunTime.ToString());
                }));

        }


        private ICommandProvider createCommandProvider()
        {
            return new TestCommandProvider();
        }

        private class TestCommandProvider : ICommandProvider
        {
            public string Execute(string command)
            {
                var parts = command.Split(' ');
                if (parts.Length > 0 && parts[0].ToLower() == "play")
                    return ">" + command + "\nPlaying the wizards!!!";

                

                return ">" + command + "\nUnknown command '" + command + "'";
            }

            public string AutoComplete(string partialCommand)
            {
                return partialCommand;
            }
        }
    }
}
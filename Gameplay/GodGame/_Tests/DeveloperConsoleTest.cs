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


        /// <summary>
        /// Attempt to correctly convert directinput to ascii, but no success
        /// </summary>
        [Test]
        public void TestPressKeys()
        {
            engine.AddSimulator(new BasicSimulator(() =>
            {
                TW.Graphics.Keyboard.PressedKeys.ForEach(k =>
                    {
                        string directInputName = Enum.GetName(typeof(Key), k);
                        consoleUi.WriteLine("RawKey: " + directInputName + " ----- Code: " + (uint)k + " ----- MappedKey: " + convertKey((uint)k));
                    });
            }));

        }

        /// <summary>
        /// Doesnt seem to work yet!
        /// </summary>
        /// <param name="scancode"></param>
        /// <returns></returns>
        private static string convertKey(uint scancode)
        {
            var layout = GetKeyboardLayout(0);
            var State = new byte[256];

            if (GetKeyboardState(State) == false)
                return "Unkown";
            var vk = MapVirtualKeyEx(scancode, 1, layout);

            var builder = new StringBuilder();

            ToAsciiEx(vk, scancode, State, builder, 0, layout);

            return builder.ToString();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        static extern int ToAsciiEx(uint uVirtKey, uint uScanCode, byte[] lpKeyState,
           [Out] StringBuilder lpChar, uint uFlags, IntPtr hkl);

        private ICommandProvider createCommandProvider()
        {
            return new TestCommandProvider();
        }

        private class TestCommandProvider : ICommandProvider
        {
            public string Execute(string command)
            {
                return "Unknown command";
            }

            public string AutoComplete(string partialCommand)
            {
                return partialCommand;
            }
        }
    }
}
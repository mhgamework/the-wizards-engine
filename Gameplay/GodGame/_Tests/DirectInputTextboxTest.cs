using System;
using System.Runtime.InteropServices;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using NSubstitute;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [EngineTest]
    [TestFixture]
    public class DirectInputTextboxTest
    {
        private TWEngine engine;
        private DirectInputTextBox textBox;
        private Textarea textarea;

        [SetUp]
        public void Setup()
        {
            engine = EngineFactory.CreateEngine();
            textBox = new DirectInputTextBox();
            textarea = new Textarea();
            textarea.Position = new Vector2(100, 100);

            engine.AddSimulator(new BasicSimulator(() =>
                {
                    textBox.ProcessUserInput(TW.Graphics.Keyboard);
                    textarea.Text = textBox.Text;

                }));

            TW.Graphics.SpectaterCamera.EnableUserInput = false;
        }

        [Test]
        public void TestPressKeys()
        {
            string txt = "";
            engine.AddSimulator(new BasicSimulator(() =>
                {
                    var keys = TW.Graphics.Keyboard.PressedKeys;
                    if (keys.Any())
                    {
                        string directInputName = Enum.GetName(typeof(Key), keys.First());
                        txt = "RawKey: " + directInputName + " ----- Code: " + (uint)keys.First();
                    }

                    textarea.Text = txt + "\nTextbox: " + textarea.Text;
                }));

        }

        /// <summary>
        /// Attempt to correctly convert directinput to ascii, but no success
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

    }
}
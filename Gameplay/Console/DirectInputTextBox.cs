using System;
using System.Linq;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Input;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// This class represents a (invisible) text box. It uses input from a TWKeyboard to process user input on the textbox
    /// Didn't get this working:http://www.gamedev.net/page/resources/_/technical/directx-and-xna/converting-scan-codes-to-ascii-r842
    /// No using manual mapping from keys to string, only a-z and 0-9 and space
    /// Windows says: do not use directinput for text input, use windows messages
    /// </summary>
    public class DirectInputTextBox
    {
        public string Text { get; set; }

        private TWKeyboard keyboard;

        public DirectInputTextBox()
        {
          
        }
        public void ProcessUserInput(TWKeyboard keyboard)
        {
            this.keyboard = keyboard;
            if (tryBackspace()) return;
            if (tryDelete()) return;
            if (tryCharacter()) return;

        }

        private bool tryDelete()
        {
            if (keyboard.IsKeyPressed(Key.Delete))
            {
                Text = "";
                return true;
            }
            return false;
        }

        private bool tryCharacter()
        {
            var chars = keyboard.PressedKeys.Where(isCharacter).Where(k => keyboard.IsKeyPressed(k));
            if (!chars.Any()) return false;

            Text += chars.Select(toCharacter).Aggregate((acc, el) => acc + el);
            return true;
        }

        private bool isCharacter(Key arg)
        {
            return toCharacter(arg) != null;
        }

        private string toCharacter(Key arg)
        {
            if (arg == Key.Semicolon) arg = Key.M;
            else if (arg == Key.M) arg = Key.Semicolon;
            else if (arg == Key.A) arg = Key.Q;
            else if (arg == Key.Q) arg = Key.A;
            else if (arg == Key.Z) arg = Key.W;
            else if (arg == Key.W) arg = Key.Z;
            

            var name = Enum.GetName(typeof(Key), arg);
            if ("abcdefghijklmnopqrstuvwxyz".ToUpper().Contains(name))
                return isShiftPressed() ? name.ToUpper() : name.ToLower();
            if (name.Contains("NumberPad"))
            {
                var digit = name.Substring("NumberPad".Length);
                if ("1234567890".Contains(digit)) return digit;
            }
            if (name.Contains("D"))
            {
                var digit = name.Substring("D".Length);
                if ("1234567890".Contains(digit) && isShiftPressed()) return digit;
            }
            if (arg == Key.D5&& !isShiftPressed())
                return "(";
            if (arg == Key.Minus&& !isShiftPressed())
                return ")";
            if (arg == Key.Comma && isShiftPressed())
                return ".";
            if (arg == Key.Semicolon && !isShiftPressed())
                return ",";
            if (arg == Key.Semicolon && isShiftPressed())
                return "?";


            if (arg == Key.NumberPadPeriod)
                return ".";
            if (arg == Key.NumberPadPlus)
                return "+";
            if (arg == Key.NumberPadMinus)
                return "-";
            if (arg == Key.NumberPadStar)
                return "*";
            if (arg == Key.NumberPadSlash)
                return "/";
            
            if (arg == Key.D8 && !isShiftPressed())
                return "?";

            if (arg == Key.Space) return " ";
            return null;
        }

        private static bool isShiftPressed()
        {
            return TW.Graphics.Keyboard.IsKeyDown(Key.LeftShift) || TW.Graphics.Keyboard.IsKeyDown(Key.RightShift);
        }

        private bool tryBackspace()
        {
            if (!keyboard.IsKeyPressed(Key.Backspace) || Text.Length <= 0)
                return false;

            Text = Text.Substring(0, Text.Length - 1);
            return true;
        }
    }
}
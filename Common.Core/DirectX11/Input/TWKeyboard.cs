using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DirectX11.Input
{
    public class TWKeyboard
    {
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        public TWKeyboard()
        {
            prevKeyboardState = new KeyboardState();
            keyboardState = new KeyboardState();
        }


        public void UpdateKeyboardState(KeyboardState nKeyboardState)
        {
            /*foreach (var key in nKeyboardState.PressedKeys)
            {
                Console.WriteLine(key);
            }*/
            prevKeyboardState = keyboardState;
            keyboardState = nKeyboardState;

            if (prevKeyboardState == null) prevKeyboardState = keyboardState;
        }


        /// <summary>
        /// Return true when the given key was pressed down last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyPressed(Key key)
        {

            if (wasKeyUp(key) && currentKeyDown(key))
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true when the given key was (still) held down last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyHeld(Key key)
        {
            if (wasKeyDown(key) && currentKeyDown(key))
                return true;
            else return false;
        }


        /// <summary>
        /// Returns true if the key was released last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyReleased(Key key)
        {
            if (wasKeyDown(key) && currentKeyUp(key))
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true when the given key is currently down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Key key)
        {
            if (currentKeyDown(key))
                return true;
            return false;
        }

        private bool wasKeyUp(Key key)
        {
            return prevKeyboardState.ReleasedKeys.Contains(key);
        }
        private bool currentKeyUp(Key key)
        {
            return keyboardState.ReleasedKeys.Contains(key);
        }
        private bool wasKeyDown(Key key)
        {
            return prevKeyboardState.PressedKeys.Contains(key);
        }
        private bool currentKeyDown(Key key)
        {
            return keyboardState.PressedKeys.Contains(key);
        }


    }
}

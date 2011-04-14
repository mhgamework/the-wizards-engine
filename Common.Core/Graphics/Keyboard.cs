using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.ServerClient
{
    public class TWKeyboard
    {
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        public TWKeyboard()
        {

        }


        public void UpdateKeyboardState( KeyboardState nKeyboardState )
        {
            prevKeyboardState = keyboardState;
            keyboardState = nKeyboardState;
        }


        /// <summary>
        /// Return true when the given key was pressed down last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyPressed( Keys key )
        {
            if ( prevKeyboardState.IsKeyUp( key ) && keyboardState.IsKeyDown( key ) )
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true when the given key was (still) held down last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyHeld( Keys key )
        {
            if ( prevKeyboardState.IsKeyDown( key ) && keyboardState.IsKeyDown( key ) )
                return true;
            else return false;
        }


        /// <summary>
        /// Returns true if the key was released last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyReleased( Keys key )
        {
            if ( prevKeyboardState.IsKeyDown( key ) && keyboardState.IsKeyUp( key ) )
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true when the given key is currently down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown( Keys key )
        {
            if ( keyboardState.IsKeyDown( key ) )
                return true;
            else return false;
        }




    }
}

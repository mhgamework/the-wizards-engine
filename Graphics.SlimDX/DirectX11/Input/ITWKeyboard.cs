using System.Collections.Generic;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DirectX11.Input
{
    public interface TWKeyboard
    {
        /// <summary>
        /// Return true when the given key was pressed down last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsKeyPressed(Key key);

        /// <summary>
        /// Returns true when the given key was (still) held down last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsKeyHeld(Key key);

        /// <summary>
        /// Returns true if the key was released last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsKeyReleased(Key key);

        /// <summary>
        /// Returns true when the given key is currently down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsKeyDown(Key key);

        /// <summary>
        /// Raw data of the keys which are currently not held down
        /// </summary>
        IList<Key> ReleasedKeys { get; }

        /// <summary>
        /// Raw data of the keys which are currently held down
        /// </summary>
        IList<Key> PressedKeys { get; }

        /// <summary>
        /// Not sure what this is :)
        /// </summary>
        IList<Key> AllKeys { get; }
    }
}
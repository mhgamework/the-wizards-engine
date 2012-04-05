using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards._XNA.Scripting.API
{
    public class Input
    {
        public IXNAGame Game;

        public Input(IXNAGame game)
        {
            Game = game;
        }

        public bool IsKeyDown(Keys key)
        {
            return Game.Keyboard.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return Game.Keyboard.IsKeyPressed(key);
        }
        public bool IsLeftButtonPressed()
        {
            return Game.Mouse.LeftMouseJustPressed;
        }
        public bool IsRightButtonPressed()
        {
            return Game.Mouse.RightMouseJustPressed;
        }
        public bool IsLeftButtonReleased()
        {
            return Game.Mouse.LeftMouseJustReleased;
        }
        public bool IsRightButtonReleased()
        {
            return Game.Mouse.RightMouseJustReleased;
        }
    }
}

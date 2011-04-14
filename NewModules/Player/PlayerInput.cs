using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// This class Handles input for a specific player. 
    /// It uses a PlayerController to apply user input to the player.
    /// 
    /// For the moment this is just an offline mode class. I am unsure at this time whether to make
    /// a seperate online mode PlayerInput class, or just make this an online-offline class
    /// 
    /// At the moment, this class uses absolute key bindings, later on the user should be able to change the keys
    /// 
    /// 
    /// </summary>
    public class PlayerInput
    {
        private PlayerController controller;
        private float mouseSensitivity;

        public PlayerInput( PlayerController _controller )
        {
            controller = _controller;
            mouseSensitivity = 0.03f;

        }

        public void Update( IXNAGame _game )
        {
            if ( _game.Keyboard.IsKeyDown( Keys.Z ) )
            {
                controller.DoMoveForward(_game.Elapsed);
            }
            if ( _game.Keyboard.IsKeyDown( Keys.S ) )
            {
                controller.DoMoveBackwards( _game.Elapsed );
            }
            if ( _game.Keyboard.IsKeyDown( Keys.Q ) )
            {
                controller.DoStrafeLeft( _game.Elapsed );
            }
            if ( _game.Keyboard.IsKeyDown( Keys.D ) )
            {
                controller.DoStrafeRight( _game.Elapsed );
            }
            if (_game.Mouse.RelativeX != 0)
            {
                //TODO: mouse sensitivity
                controller.DoRotateHorizontal( _game.Mouse.RelativeX * -mouseSensitivity );
            }
            if ( _game.Mouse.RelativeY != 0 )
            {
                controller.DoRotateVertical( _game.Mouse.RelativeY * -mouseSensitivity );
            }
        }
    }
}

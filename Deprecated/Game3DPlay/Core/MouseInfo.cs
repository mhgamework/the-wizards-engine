using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core
{
    public class MouseInfo
    {
        public MouseInfo()
        {
            _speed = 0.1F;
        }

        private MouseState prevMouseState;
        private MouseState mouseState;

        private float _relativeX;

        public float RelativeX
        {
            get { return _relativeX; }
            set { _relativeX = value; }
        }
        /// <summary>
        /// DEPRECATED. Relative coords were inverted at an earlier stage of the engine.
        /// </summary>
        public float OppositeRelativeX
        {
            get { return -_relativeX; }
            //set { _relativeX = value; }
        }

        private float _relativeY;

        public float RelativeY
        {
            get { return _relativeY; }
            set { _relativeY = value; }
        }
        /// <summary>
        /// DEPRECATED. Relative coords were inverted at an earlier stage of the engine.
        /// </summary>
        public float OppositeRelativeY
        {
            get { return -_relativeY; }
            //set { _relativeY = value; }
        }

        private float _relativeScrollWheel;
        public float RelativeScrollWheel
        {
            get { return _relativeScrollWheel; }
            //set { _relativeY = value; }
        }

        public Point CursorPosition
        {
            get
            {
                //TODO: check if engine.CursorEnabled == true
                return new Point( mouseState.X, mouseState.Y );
            }
        }

        private float _speed;

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public bool LeftMouseJustPressed
        {
            get { return ( prevMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed ); }
        }
        public bool LeftMouseJustReleased
        {
            get { return ( prevMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released ); }
        }


        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>
        public void SetRelativePos( int relativeX, int relativeY )
        {
            _relativeX = relativeX * Speed;
            _relativeY = relativeY * Speed;



        }

        public void UpdateMouseState( MouseState nMouseState, MouseState nPrevState )
        {
            prevMouseState = nPrevState;
            mouseState = nMouseState;

            _relativeX = mouseState.X - prevMouseState.X;
            _relativeY = mouseState.Y - prevMouseState.Y;
            _relativeX *= Speed;
            _relativeY *= Speed;

            _relativeScrollWheel = mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue;
        }
        public void UpdateMouseState( MouseState nMouseState )
        {
            UpdateMouseState( nMouseState, mouseState );

        }



        public MouseState MouseState
        {
            get { return mouseState; }
        }




    }
}

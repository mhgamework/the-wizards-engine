using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    /// <summary>
    /// TODO: when in cursorEnabled mode, clicking outside the window should probably
    /// not indicate a mousedown?
    /// </summary>
    public class TWMouse
    {
        IXNAGame game;

        float speed;
        private MouseState neutralMouseState;
        private MouseState prevMouseState;
        private MouseState mouseState;
        private float relativeX;
        private float relativeY;
        private float relativeScrollWheel;
        private float prevScrollWheel;
        private bool gameInActive;

        private bool cursorEnabled;

        private Point? savedCursorPosition;

        public TWMouse(IXNAGame nGame)
        {
            game = nGame;
            speed = 0.1f;
            relativeX = 0;
            relativeY = 0;
            relativeScrollWheel = 0;
        }


        public bool RightMouseJustPressed
        {
            get
            {
                return (prevMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed);
            }
        }
        public bool RightMouseJustReleased
        {
            get
            {
                return (prevMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released);
            }
        }
        public bool RightMousePressed
        {
            get
            {
                return mouseState.RightButton == ButtonState.Pressed;
            }
        }

        public bool LeftMouseJustPressed
        {
            get
            {
                return (prevMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed);
            }
        }
        public bool LeftMouseJustReleased
        {
            get
            {
                return (prevMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released);
            }
        }
        public bool LeftMousePressed
        {
            get
            {
                return mouseState.LeftButton == ButtonState.Pressed;
            }
        }

        public void UpdateMouseState()//MouseState nMouseState, MouseState nPrevState )
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            prevScrollWheel = prevMouseState.ScrollWheelValue;

            if (CursorEnabled)
            {
                //prevMouseState = nPrevState;
                if (!game.IsCursorInWindow() || !game.IsActive)
                {
                    // The cursor is not on the window, or the window does not have focus

                    // Disable all NEW mouse presses

                    var left = mouseState.LeftButton;
                    var right = mouseState.RightButton;

                    if (prevMouseState.LeftButton == ButtonState.Released)
                        left = ButtonState.Released;
                    if (prevMouseState.RightButton == ButtonState.Released)
                        right = ButtonState.Released;


                    mouseState = new MouseState(mouseState.X, mouseState.Y, mouseState.ScrollWheelValue,
                        left, mouseState.MiddleButton, right,
                        mouseState.XButton1, mouseState.XButton2);
                }
            }
            else
            {
                // Enable First Person style mouse

                if (!game.IsActive)
                {
                    // Save that the game is inactive, so we can determine the moment when it gets active again.
                    gameInActive = true;
                    // Game is inactive, disable mouse
                    mouseState = prevMouseState;

                }
                else
                {
                    // Since we cannot change the ScrollWheel mouse state in setNeutralMouseState, we have to memorize it manually
                    prevScrollWheel = prevMouseState.ScrollWheelValue;


                    SetNeutralMouseState();
                    prevMouseState = new MouseState(neutralMouseState.X, neutralMouseState.Y, neutralMouseState.ScrollWheelValue, prevMouseState.LeftButton, prevMouseState.MiddleButton, prevMouseState.RightButton, prevMouseState.XButton1, prevMouseState.XButton2);


                    if (gameInActive)
                    {
                        // The game was inactive in previous update, so the last mouseState is invalid.
                        // We make sure the mouse changes during disabled time don't apply.
                        mouseState = prevMouseState;
                    }
                    gameInActive = false;
                }
            }






            relativeX = mouseState.X - prevMouseState.X;
            relativeY = mouseState.Y - prevMouseState.Y;
            relativeX *= Speed;
            relativeY *= Speed;


            relativeScrollWheel = mouseState.ScrollWheelValue - prevScrollWheel;



        }
        //public void UpdateMouseState( MouseState nMouseState )
        //{
        //    UpdateMouseState( nMouseState, mouseState );

        //}

        /// <summary>
        /// Sets the cursor to neutral mouse state (center of the window)
        /// </summary>
        private void SetNeutralMouseState()
        {
            Mouse.SetPosition(game.ClientSize.X >> 1, game.ClientSize.Y >> 1);
            neutralMouseState = Mouse.GetState();
        }



        public Point CursorPosition
        {
            get
            {
                if (!cursorEnabled) throw new InvalidOperationException("The cursor is currently disabled.");
                return new Point(mouseState.X, mouseState.Y);
            }
        }
        public Vector2 CursorPositionVector
        {
            get
            {
                if (!cursorEnabled) throw new InvalidOperationException("The cursor is currently disabled.");
                return new Vector2(mouseState.X, mouseState.Y);
            }
        }
        public float RelativeY
        {
            get { return relativeY; }
        }
        public float RelativeX
        {
            get { return relativeX; }
        }
        public MouseState MouseState
        {
            get { return mouseState; }
        }
        public float RelativeScrollWheel
        {
            get { return relativeScrollWheel; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        /// <summary>
        /// Specifies whether to use the system cursor or not.
        /// </summary>
        public bool CursorEnabled
        {
            get
            {
                return cursorEnabled;
            }
            set
            {
                if (cursorEnabled == value) return;

                if (cursorEnabled == true)
                {
                    // Cursor is about to be disabled. Save the cursor position
                    // to restore it when the cursor is reenabled
                    savedCursorPosition = CursorPosition;

                    // Set the cursor to the neutral state to remove dirty data
                    SetNeutralMouseState();
                    mouseState = neutralMouseState;
                }
                else
                {
                    //Cursor is about to be enabled. Check if a previous position was saved and restore it.
                    if (savedCursorPosition.HasValue)
                        Microsoft.Xna.Framework.Input.Mouse.SetPosition(savedCursorPosition.Value.X, savedCursorPosition.Value.Y);
                    mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                }

                cursorEnabled = value;

            }
        }


    }
}

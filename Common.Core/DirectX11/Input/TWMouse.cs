using System;
using System.Drawing;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DirectX11.Input
{
    /// <summary>
    /// TODO: when in cursorEnabled mode, clicking outside the window should probably
    /// not indicate a mousedown?
    /// </summary>
    public class TWMouse
    {

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

        public TWMouse()
        {
            speed = 0.1f;
            relativeX = 0;
            relativeY = 0;
            relativeScrollWheel = 0;
        }


        public bool RightMouseJustPressed
        {
            get
            {
                return (!prevRightPressed() && rightPressed());
            }
        }


        public bool RightMouseJustReleased
        {
            get
            {
                return (prevRightPressed() && !rightPressed());
            }
        }
        public bool RightMousePressed
        {
            get
            {
                return rightPressed();
            }
        }

        public bool LeftMouseJustPressed
        {
            get
            {
                return (!prevLeftPressed() && leftPressed());
            }
        }


        public bool LeftMouseJustReleased
        {
            get
            {
                return (prevLeftPressed() && !leftPressed());
            }
        }
        private bool prevLeftPressed()
        {
            return prevMouseState.IsPressed(0);
        }
        private bool leftPressed()
        {
            return mouseState.IsPressed(0);
        }
        private bool rightPressed()
        {
            return mouseState.IsPressed(1);
        }

        private bool prevRightPressed()
        {
            return prevMouseState.IsPressed(1);
        }
        public bool LeftMousePressed
        {
            get
            {
                return leftPressed();
            }
        }

        public void UpdateMouseState(MouseState state)
        {
            prevMouseState = mouseState;
            mouseState = state;
            if (prevMouseState == null) prevMouseState = mouseState;
            prevScrollWheel = prevMouseState.Z; //TODO: check

            if (CursorEnabled)
            {
                //prevMouseState = nPrevState;
                if (!IsCursorInWindowTODO() || !GameIsActiveTODO)
                {
                    // The cursor is not on the window, or the window does not have focus

                    // Disable all NEW mouse presses
                    var left = mouseState.GetButtons()[0];
                    var right = mouseState.GetButtons()[1];

                    var prevLeft = prevMouseState.GetButtons()[0];
                    var prevRight = prevMouseState.GetButtons()[1];

                    if (prevLeft == false)
                        left = false;
                    if (prevRight == false)
                        right = false;


                    //TODO:
                    /*mouseState = new MouseState(mouseState.X, mouseState.Y, mouseState.ScrollWheelValue,
                        left, mouseState.MiddleButton, right,
                        mouseState.XButton1, mouseState.XButton2);*/
                }
            }
            else
            {
                // Enable First Person style mouse

                if (!GameIsActiveTODO)
                {
                    // Save that the game is inactive, so we can determine the moment when it gets active again.
                    gameInActive = true;
                    // Game is inactive, disable mouse
                    mouseState = prevMouseState;

                }
                else
                {
                    // Since we cannot change the ScrollWheel mouse state in setNeutralMouseState, we have to memorize it manually
                    prevScrollWheel = prevMouseState.Z; //TODO: check


                    SetNeutralMouseState();
                    //TODO: prevMouseState = new MouseState(neutralMouseState.X, neutralMouseState.Y, neutralMouseState.ScrollWheelValue, prevLeftPressed(), prevMouseState.MiddleButton, prevRightPressed(), prevMouseState.XButton1, prevMouseState.XButton2);

                    
                    if (gameInActive || savedCursorPosition.HasValue)
                    {
                        // The game was inactive in previous update, so the last mouseState is invalid.
                        // We make sure the mouse changes during disabled time don't apply.
                        mouseState = prevMouseState;
                    }
                    gameInActive = false;
                }
            }






            //relativeX = mouseState.X - prevMouseState.X;
            //relativeY = mouseState.Y - prevMouseState.Y;
            relativeX = mouseState.X;
            relativeY = mouseState.Y;
            relativeX *= Speed;
            relativeY *= Speed;



            relativeScrollWheel = state.Z;

            //TODO: This should prob be removed later on
            if (!CursorEnabled && savedCursorPosition.HasValue)
            {
                // cursor was enabled last frame, dont process movement
                relativeX = 0;
                relativeY = 0;
                savedCursorPosition = null;

            }

        }

        private bool IsCursorInWindowTODO()
        {
            throw new NotImplementedException();
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
            //TODO:
            /*Mouse.SetPosition(game.ClientSize.X >> 1, game.ClientSize.Y >> 1);
            neutralMouseState = Mouse.GetState();*/
        }



        public Point CursorPosition
        {
            get
            {
                if (!cursorEnabled) throw new InvalidOperationException("The cursor is currently disabled.");
                return new Point(mouseState.X, mouseState.Y);
            }
        }
        /*public Vector2 CursorPositionVector
        {
            get
            {
                if (!cursorEnabled) throw new InvalidOperationException("The cursor is currently disabled.");
                return new Vector2(mouseState.X, mouseState.Y);
            }
        }*/
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

                if (cursorEnabled)
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
                    //TODO:
                    /*if (savedCursorPosition.HasValue)
                        Microsoft.Xna.Framework.Input.Mouse.SetPosition(savedCursorPosition.Value.X, savedCursorPosition.Value.Y);
                    mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();*/
                }

                cursorEnabled = value;

            }
        }



        public bool GameIsActiveTODO { get { return true; } }
    }
}

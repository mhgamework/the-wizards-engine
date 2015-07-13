using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Input
{
    /// <summary>
    /// TODO: when in cursorEnabled mode, clicking outside the window should probably
    /// not indicate a mousedown?
    /// </summary>
    public class TWMouse
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out System.Drawing.Point lpPoint);


        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);


        float speed;
        private MouseState neutralMouseState;
        private MouseState prevMouseState;
        private MouseState mouseState;
        private float relativeX;
        private float relativeY;
        private float relativeScrollWheel;
        private float prevScrollWheel;
        private bool gameInActive;

        private Point currentCursorPos;

        private bool cursorEnabled;

        private Point? savedCursorPosition;

        public TWMouse()
        {
            speed = 0.1f;
            relativeX = 0;
            relativeY = 0;
            relativeScrollWheel = 0;
        }


        public bool LeftMousePressed { get { return leftPressed(); } }
        public bool RightMousePressed { get { return rightPressed(); } }


        public bool RightMouseJustPressed { get { return (!prevRightPressed() && rightPressed()); } }
        public bool RightMouseJustReleased { get { return (prevRightPressed() && !rightPressed()); } }
        public bool LeftMouseJustPressed { get { return (!prevLeftPressed() && leftPressed()); } }
        public bool LeftMouseJustReleased { get { return (prevLeftPressed() && !leftPressed()); } }


        private bool prevLeftPressed() { return prevMouseState.IsPressed(0); }
        private bool prevRightPressed() { return prevMouseState.IsPressed(1); }
        private bool leftPressed() { return mouseState.IsPressed(0); }
        private bool rightPressed() { return mouseState.IsPressed(1); }



        public void UpdateMouseState(MouseState state)
        {
            prevMouseState = mouseState;
            mouseState = state;
            if (prevMouseState == null) prevMouseState = mouseState;
            prevScrollWheel = prevMouseState.Z; //TODO: check

            if (CursorEnabled)
                updateMouseStateCursor();
            else
                updateMouseStateNoCursor();


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

        private void updateMouseStateNoCursor()
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

        private void updateMouseStateCursor()
        {
            //prevMouseState = nPrevState;
            if (!IsCursorInWindow() || !GameIsActiveTODO)
                mirrorOldMouseState();

            GetCursorPos(out currentCursorPos);
        }

        /// <summary>
        /// Makes the new mouse state mirror the previous mouse state (thus freezing mouse input)
        /// </summary>
        private void mirrorOldMouseState()
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

        /// <summary>
        /// Set this to the current window bounds for this mouse helper!
        /// </summary>
        public Rectangle WindowBounds { get; set; }
        private bool IsCursorInWindow()
        {
            return WindowBounds.Contains(CursorPosition.X, CursorPosition.Y);
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

            //mouseState = neutralMouseState;
        }



        public Point CursorPosition
        {
            get
            {
                if (!cursorEnabled) throw new InvalidOperationException("The cursor is currently disabled.");
                return currentCursorPos;
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
                    storeCursorState(); // For restoring later!
                    SetNeutralMouseState();
                }
                else
                {
                    restoreCursorState();
                }

                cursorEnabled = value;

            }
        }

        private void restoreCursorState()
        {
            //TODO:
            /*if (savedCursorPosition.HasValue)
                Microsoft.Xna.Framework.Input.Mouse.SetPosition(savedCursorPosition.Value.X, savedCursorPosition.Value.Y);
            mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();*/
        }

        private void storeCursorState()
        {
            savedCursorPosition = CursorPosition;
        }


        public bool GameIsActiveTODO { get { return true; } }
    }
}

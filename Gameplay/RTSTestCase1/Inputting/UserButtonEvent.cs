namespace MHGameWork.TheWizards.RTSTestCase1.Inputting
{
    /// <summary>
    /// Responsible for encapsulating a button like user input.
    /// </summary>
    public class UserButtonEvent
    {
        /// <summary>
        /// True when button is down, false when up
        /// </summary>
        public bool Down { get; private set; }
        /// <summary>
        /// Button pressed last frame
        /// </summary>
        public bool Pressed { get; private set; }
        /// <summary>
        /// Button released last frame
        /// </summary>
        public bool Released { get; private set; }


        public void SetState(bool down)
        {
            Released = Down & !down;
            Pressed = !Down & down;
            Down = down;
        }

        public void Reset()
        {
            Down = false;
            Pressed = false;
            Released = false;
        }
    }
}
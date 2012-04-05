using System;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public struct PlayerInputPacket : INetworkPacket
    {
        //public int TickNumber;
        public float HorizontalLookAngle;
        public PlayerInput Input;

        [Flags]
        public enum PlayerInput : byte
        {
            None = 0,
            Forward = 1,
            Backward = 2,
            Left = 4,
            Right = 8,
            Jump = 16,
            Crouch = 32 // Not implemented
        }
    }
}

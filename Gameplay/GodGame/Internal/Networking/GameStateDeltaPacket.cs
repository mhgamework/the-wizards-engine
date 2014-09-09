using DirectX11;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// A network packet holding user input data produced by an IUserInputHandler
    /// </summary>
    public struct GameStateDeltaPacket : INetworkPacket
    {
        public int[] CoordsX;
        public int[] CoordsY;
        public string[] Types;
    }
}
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.GodGame.Internal.Networking.Packets
{
    /// <summary>
    /// A network packet holding user input data
    /// </summary>
    public struct UserInputPacket : INetworkPacket
    {
        public byte[] data;
    }
}
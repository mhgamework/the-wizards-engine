using DirectX11;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// A network packet holding user input data produced by an IUserInputHandler
    /// </summary>
    public struct UserInputHandlerPacket : INetworkPacket
    {
        public string Method;
        public int VoxelCoordX;
        public int VoxelCoordY;

        public int Key;

        public UserInputHandlerPacket(string method)
            : this()
        {
            Method = method;
        }

        public UserInputHandlerPacket(string method, Point2 voxelCoord)
            : this()
        {
            Method = method;
            VoxelCoordX = voxelCoord.X;
            VoxelCoordY = voxelCoord.Y;
        }
    }
}
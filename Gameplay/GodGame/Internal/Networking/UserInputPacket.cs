using DirectX11;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// A network packet holding user input data produced by an IUserInputHandler
    /// </summary>
    public struct UserInputPacket : INetworkPacket
    {
        public string Method;
        public int VoxelCoordX;
        public int VoxelCoordY;

        public UserInputPacket(string method)
            : this()
        {
            Method = method;
        }

        public UserInputPacket(string method, Point2 voxelCoord)
        {
            Method = method;
            VoxelCoordX = voxelCoord.X;
            VoxelCoordY = voxelCoord.Y;
        }
    }
}
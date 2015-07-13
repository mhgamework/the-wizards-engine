using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    public struct ErrorPacket : INetworkPacket
    {
        public string Description;

        public ErrorPacket(string description)
        {
            Description = description;
        }
    }
}
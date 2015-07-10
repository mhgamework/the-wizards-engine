using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    public struct DataPacket : INetworkPacket
    {
        public string Text;
        public int Number;

        public DataPacket(string _text, int _number)
        {
            Text = _text;
            Number = _number;
        }
    }
}
using System.Linq;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.GodGame.Networking.Facade
{
    public class RemoteMethodTransporter
    {
        private StringSerializer serializer;
        private IClientPacketTransporter<RemoteMethodPacket> transporter;


        public void Send(string method, params object[] args)
        {
            var p = new RemoteMethodPacket()
                {
                    Arguments = args.Select(serializer.Serialize).ToArray(),
                    Method = method
                };

            transporter.Send(p);
        }

        public struct RemoteMethodPacket : INetworkPacket
        {
            public string Method;
            public string[] Arguments;
        }
    }
}
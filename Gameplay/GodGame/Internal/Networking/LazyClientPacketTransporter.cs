using System;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.GodGame.Networking
{
    /// <summary>
    /// A clientpackettransporter which redirects calls to a proxied transporter. This class can be used if the
    /// proxy transporter is to connected after it is dependency injected
    /// TODO: probably somewhat haxor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyClientPacketTransporter<T> : IClientPacketTransporter<T> where T : INetworkPacket
    {
        private readonly Func<ClientPacketManagerNetworked.ClientPacketTransporterNetworked<T>> target;

        public LazyClientPacketTransporter(Func<ClientPacketManagerNetworked.ClientPacketTransporterNetworked<T>> target)
        {
            this.target = target;

        }

        public void Send(T packet)
        {
            if (target() == null) throw new InvalidOperationException("Not connected yet!");
            target().Send(packet);
        }

        public T Receive()
        {
            if (target() == null) throw new InvalidOperationException("Not connected yet!");

            return target().Receive();
        }

        public bool PacketAvailable
        {
            get
            {
                if (target() == null) return false;
                return target().PacketAvailable;
            }
        }
    }
}
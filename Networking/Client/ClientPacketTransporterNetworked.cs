using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace MHGameWork.TheWizards.Networking.Client
{
    public partial class ClientPacketManagerNetworked
    {
        public class ClientPacketTransporterNetworked<T> : IClientPacketTransporterNetworked, IClientPacketTransporter<T> where T : INetworkPacket
        {
            private INetworkPacketFactory<T> factory;


            private BasicPacketTransporter<T> internalTransporter;

            /// <summary>
            /// Internal use!
            /// </summary>
            public int WaitingReceiversCount { get; set; }

            private string uniqueName;
            public string UniqueName
            {
                get { return uniqueName; }
            }

            public PacketFlags Flags { get; private set; }

            public int NetworkID { get; private set; }

            public INetworkPacketFactory<T> Factory
            {
                get { return factory; }

            }

            public void SetNetworkID(int id)
            {
                NetworkID = id;
            }


            private ClientPacketManagerNetworked manager;

            public ClientPacketTransporterNetworked(PacketFlags flags, int networkId, INetworkPacketFactory<T> factory, ClientPacketManagerNetworked manager, string _uniqueName)
            {
                this.Flags = flags;
                NetworkID = networkId;
                this.factory = factory;
                this.manager = manager;
                this.uniqueName = _uniqueName;
                internalTransporter = new BasicPacketTransporter<T>(p => manager.SendPacket(this, p));
            }

            #region IClientPacketTransporter<T> Members

            public void Send(T packet)
            {
                internalTransporter.Send(packet);
            }


            public int WaitingReceivers
            {
                get { return internalTransporter.WaitingReceivers; }
            }

            public T Receive()
            {
                return internalTransporter.Receive();
            }

            /// <summary>
            /// If there are receivers active, this property might always be false.
            /// </summary>
            public Boolean PacketAvailable
            {
                get { return internalTransporter.PacketAvailable; }
            }


            /// <summary>
            /// Internal use only!
            /// Should be thread safe!
            /// </summary>
            void IClientPacketTransporterNetworked.QueueReceivedPacket(BinaryReader br)
            {
                T packet = factory.FromStream(br);

                internalTransporter.QueueReceivedPacket(packet);
            }

            #endregion
        }
    }
}
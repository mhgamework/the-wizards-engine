﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.Networking.Server
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServerPacketTransporterNetworked<T> : IServerPacketTransporterNetworked, IServerPacketTransporter<T> where T : INetworkPacket
    {
        private List<IClientPacketTransporter<T>> transporters = new List<IClientPacketTransporter<T>>();

        private Dictionary<IClient, IClientPacketTransporter<T>> transportersDict =
            new Dictionary<IClient, IClientPacketTransporter<T>>();

        private Queue<PacketInfo> receiveQueue = new Queue<PacketInfo>();

        public void SendAll(T packet)
        {
            for (int i = 0; i < transporters.Count; i++)
            {
                transporters[i].Send(packet);
            }
        }
        


        private bool receiveMode;
        /// <summary>
        /// This is true when some object has requested a clienttransporter. This is to prevent enabling receivemode
        /// </summary>
        private bool transportersPublished;
        /// <summary>
        /// This will cause this serverPT to collect all packets from the clienttransporters. Clienttransporters can not be used directly this way, they are managed
        /// by this object.
        /// </summary>
        public void EnableReceiveMode()
        {
            if (transportersPublished)
                throw new InvalidOperationException(
                    "Some internal ClientPacketTransporters of this transporter have been released. Receivemode can't be enabled anymore");

            receiveMode = true;

            foreach (KeyValuePair<IClient, IClientPacketTransporter<T>> clientPacketTransporter in transportersDict)
            {
                setupReceiveJob(clientPacketTransporter.Key);
            }

        }
        /*public void DisableReceiveMode()
        {
            throw new InvalidOperationException();
        }*/

        public T Receive(out IClient client)
        {
            if (!receiveMode)
                throw new InvalidOperationException("To use this method, receivemode must be enabled on this object!");
            lock (receiveQueue)
            {
                while (receiveQueue.Count == 0)
                    Monitor.Wait(receiveQueue);
                var info = receiveQueue.Dequeue();

                client = info.Client;

                return info.Packet;
            }
        }

        public bool PacketAvailable
        {
            get
            {
                if (!receiveMode)
                    throw new InvalidOperationException("To use this method, receivemode must be enabled on this object!");
                lock (receiveQueue)
                {
                    return receiveQueue.Count > 0;
                }
            }
        }

        public IClientPacketTransporter<T> GetTransporterForClient(IClient client)
        {
            if (receiveMode)
                throw new InvalidOperationException(
                    "ClientTransporters are used internally when ReceiveMode is enabled!");

            return getTransporterForClientInternal(client);
        }
        private IClientPacketTransporter<T> getTransporterForClientInternal(IClient client)
        {
            // check if this client is ready.
            if (!client.IsReady) throw new InvalidOperationException("This client is not yet ready!");
         
            transportersPublished = true;
            return transportersDict[client];
        }


        public void AddClientTransporter(IClient client, IClientPacketTransporter<T> transporter)
        {
            var t = transporter;
            transporters.Add(t);
            transportersDict.Add(client, t);

            if (receiveMode)
                setupReceiveJob(client);

        }

        private void setupReceiveJob(IClient client)
        {
            var t = new Thread(() => receiveJob(client));
            t.Name = "ServerPacketTransporterNetworkedReceive";
            t.IsBackground = true;
            t.Start();
        }

        private void receiveJob(IClient client)
        {
            var transporter = getTransporterForClientInternal(client);
            for (; ; )
            {
                var p = transporter.Receive();
                var info = new PacketInfo { Client = client, Packet = p };

                lock (receiveQueue)
                {
                    receiveQueue.Enqueue(info);
                    Monitor.Pulse(receiveQueue);
                }
            }
        }


        private class PacketInfo
        {
            public IClient Client;
            public T Packet;
        }

        void IServerPacketTransporterNetworked.AddClientTransporter(IClient client, ClientPacketManagerNetworked.IClientPacketTransporterNetworked transporter)
        {
            AddClientTransporter(client, (IClientPacketTransporter<T>)transporter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards;
using Microsoft.Xna.Framework;
using TreeGenerator.TreeEngine;

namespace TreeGenerator.EngineSynchronisation
{
    public class ServerTreeSyncer
    {

        private IServerPacketTransporter<TreePacket> transporter;

        private List<EngineTree> trees = new List<EngineTree>();
        private IServerPacketTransporter<DataPacket> requestAllTransporter;

        public ServerTreeSyncer(IServerPacketManager packetManager)
        {
            var gen = new MHGameWork.TheWizards.Networking.NetworkPacketFactoryCodeGenerater(TWDir.GenerateRandomCacheFile("", "dll"));
            var factory = gen.GetFactory<TreePacket>();
            transporter = packetManager.CreatePacketTransporter("ServerTreeSyncer", factory, MHGameWork.TheWizards.Networking.PacketFlags.TCP);
            requestAllTransporter = packetManager.CreatePacketTransporter("ServerTreeSyncerRequestAll",
                                                                           new DataPacket.Factory(),
                                                                          MHGameWork.TheWizards.Networking.
                                                                              PacketFlags.TCP);
            requestAllTransporter.EnableReceiveMode();

            gen.BuildFactoriesAssembly();
        }


        public void Update()
        {
            while (requestAllTransporter.PacketAvailable)
            {
                IClient client;
                requestAllTransporter.Receive(out client);

                for (int i = 0; i < trees.Count; i++)
                {
                    transporter.SendAll(new TreePacket(trees[i]));
                }
            }
        }
        public void AddTree(EngineTree tree)
        {

            trees.Add(tree);
            transporter.SendAll(new TreePacket(tree));
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Networking.Packets;
using Microsoft.Xna.Framework;
using TreeGenerator.LodEngine;
using TreeGenerator.TreeEngine;

namespace TreeGenerator.EngineSynchronisation
{
    public class ClientTreeSyncer
    {
        IClientPacketTransporter<TreePacket> transporter;
        private TreeLodEngine treeLodEngine;
        private ClientAssetTreeTypeFactory treeFac;
        private TreeLodEntity treeLodEntity;
        private TreeStructureGenerater treeStructureGenerater;
        private IClientPacketTransporter<DataPacket> requestAllTransporter;

        public ClientTreeSyncer(IClientPacketManager packetManager, TreeLodEngine treeLodEngine, ClientAssetTreeTypeFactory treeFac)
        {
            this.treeLodEngine = treeLodEngine;
            this.treeFac = treeFac;
            var gen = new MHGameWork.TheWizards.Networking.NetworkPacketFactoryCodeGenerater(TWDir.generateRandomCacheFile("", "dll"));
            var factory = gen.GetFactory<TreePacket>();

            transporter = packetManager.CreatePacketTransporter("ServerTreeSyncer", factory, MHGameWork.TheWizards.Networking.PacketFlags.TCP);
            requestAllTransporter = packetManager.CreatePacketTransporter("ServerTreeSyncerRequestAll", new DataPacket.Factory(), MHGameWork.TheWizards.Networking.PacketFlags.TCP);

            gen.BuildFactoriesAssembly();
            treeStructureGenerater = new TreeStructureGenerater();
        }

        public void Update()
        {

            while (transporter.PacketAvailable)
            {
                var p = transporter.Receive();
                //for next design get's the appropriate treetype 
                var treeType = treeFac.GetTreeType(p.Guid);
                treeType.Asset.Synchronize();
                var data = treeType.GetData();
                ((ClientTextureAsset)data.TextureBark).ClientAsset.Synchronize();
                for (int i = 0; i < data.Levels.Count; i++)
                {
                    for (int j = 0; j < data.Levels[i].LeafType.Count; j++)
                    {
                        ((ClientTextureAsset) data.Levels[i].LeafType[j].Texture).ClientAsset.Synchronize();
                    }
                }
                


                treeLodEntity = treeLodEngine.CreateTreeLodEntity(treeStructureGenerater.GenerateTree(treeType.GetData(), p.Seed));
                treeLodEntity.WorldMatrix = Matrix.CreateRotationY(p.Rotation);
                treeLodEntity.WorldMatrix *= Matrix.CreateTranslation(p.PosX, p.PosY, p.PosZ);

            }
        }
        public void RequestAllTrees()
        {
            requestAllTransporter.Send(new DataPacket { Data = new byte[0] });
        }


    }
}

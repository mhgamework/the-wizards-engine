using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Files;

namespace MHGameWork.TheWizards.Assets
{
    /// <summary>
    /// Make this abstract and create a non-networked implementation
    /// </summary>
    public class ClientAssetSyncer
    {
        public DirectoryInfo AssetsDirectory { get; private set; }
        private IClientPacketTransporter<AssetRequestPacket> assetContentsRequester;
        private IClientPacketTransporter<AssetContentPacket> assetContent;
        private IClientPacketTransporter<AssetRequestFilePacket> assetRequestFile;
        private ClientFileTransporter<AssetRequestFilePacket> fileTransporter;
        private SHA1 sha;
        private List<ClientAsset> assets = new List<ClientAsset>();

        public ClientAssetSyncer(IClientPacketManager pm, DirectoryInfo assetsDirectory)
        {
            AssetsDirectory = assetsDirectory;
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.generateRandomCacheFile("", "dll"));

            assetContentsRequester = pm.CreatePacketTransporter("AssetContentRequest", gen.GetFactory<AssetRequestPacket>(),
                                                                PacketFlags.TCP);
            assetContent = pm.CreatePacketTransporter("AssetContent", new AssetContentPacket.Factory(),
                                                      PacketFlags.TCP);
            assetRequestFile = pm.CreatePacketTransporter("AssetRequestFile", gen.GetFactory<AssetRequestFilePacket>(),
                                                          PacketFlags.TCP);
            fileTransporter = new ClientFileTransporter<AssetRequestFilePacket>("AssetFileTransporter", pm, TWDir.Cache.CreateSubdirectory("ClientAssetsSyncer").FullName);

            gen.BuildFactoriesAssembly();

            sha = SHA1.Create();


        }

        private ConcurrentQueue<ClientAsset> assetsQueue = new ConcurrentQueue<ClientAsset>();


        public void Start()
        {
            var t = new Thread(contentJob);
            t.Name = "ClientAssetSyncerContentJob";
            t.IsBackground = true;
            t.Start();


            t = new Thread(fileJob);
            t.Name = "ClientAssetSyncerFileJob";
            t.IsBackground = true;

            t.Start();


        }

        public void fileJob()
        {
            for (; ; )
            {

                var info = fileTransporter.ReceiveFile();
                tempCount--;
                var asset = GetAsset(info.Packet.GUID);
                var fileComponent = asset.GetFileComponent(info.Packet.FileIndex);
                File.Copy(info.CachedFilePath, fileComponent.GetFullPath(), true);
                File.Delete(info.CachedFilePath);

                updateFileHash(fileComponent);

                requestNextUpdateItem();

            }
        }
        public void contentJob()
        {
            for (; ; )
            {
                var p = assetContent.Receive();
                var asset = GetAsset(p.GUID);

                asset.applyAssetContentPacket(p);

                for (int i = 0; i < asset.FileComponentCount; i++)
                {
                    updateFileHash(asset.GetFileComponent(i));
                }

                requestNextUpdateItem();

            }

        }

        private ClientAsset currentSyncingAsset;

        private int tempCount;

        private void requestNextUpdateItem()
        {

            ClientAsset asset;

            if (currentSyncingAsset == null)
            {
                // Start sycing a new asset

                assetsQueue.TryDequeue(out currentSyncingAsset);
                if (currentSyncingAsset == null) return;
                //currentSyncingAsset.IsComponentsUpToDate = false;
            }

            if (!currentSyncingAsset.IsComponentsUpToDate)
            {
                assetContentsRequester.Send(new AssetRequestPacket { GUID = currentSyncingAsset.GUID });
                return;

            }



            asset = currentSyncingAsset;

            for (int i = 0; i < asset.FileComponentCount; i++)
            {
                var file = asset.GetFileComponent(i);
                if (file.IsAvailable())
                    continue;

                var fileRequest = new AssetRequestFilePacket { GUID = currentSyncingAsset.GUID, FileIndex = i };
                assetRequestFile.Send(fileRequest);
                tempCount++;
                if (tempCount > 1) throw new InvalidOperationException();
                return;
            }


            // Asset ready
            asset.setAvailable(true);
            currentSyncingAsset = null;

            lock (this)
                Monitor.PulseAll(this);

            // Request next

            requestNextUpdateItem();
        }


        public void ScheduleForSync(ClientAsset asset)
        {
            assetsQueue.Enqueue(asset);
            if (currentSyncingAsset != null) return;

            // Nothing is being synced at the moment, start syncing process
            requestNextUpdateItem();

        }
        public void WaitForSynchronized(ClientAsset asset)
        {
            lock (this)
                while (!asset.IsAvailable)
                    Monitor.Wait(this);
        }


        public ClientAsset GetAsset(Guid guid)
        {
            var a = assets.Find(o => o.GUID.Equals(guid));
            if (a == null)
            {
                a = new ClientAsset(this, guid);
                assets.Add(a);
            }

            return a;
        }


        private void updateFileHash(ClientAssetFile file)
        {
            var path = file.GetFullPath();
            if (!File.Exists(path))
            {
                file.Hash = null;
                return;
            }
            using (var fs = File.OpenRead(path))
            {
                var hash = sha.ComputeHash(fs);
                file.Hash = hash;
            }

        }

    }


    public class AssetRequestPacket : INetworkPacket
    {
        public Guid GUID;
    }
    public class AssetContentPacket : INetworkPacket
    {
        public Guid GUID;
        public FileComponent[] Files;

        public class Factory : INetworkPacketFactory<AssetContentPacket>
        {
            public AssetContentPacket FromStream(BinaryReader reader)
            {
                var p = new AssetContentPacket();
                p.GUID = new Guid(reader.ReadBytes(16));
                var count = reader.ReadInt32();
                p.Files = new FileComponent[count];
                for (int i = 0; i < count; i++)
                {
                    var c = new FileComponent();
                    c.Path = reader.ReadString();
                    c.Hash = reader.ReadBytes(reader.ReadInt32());
                    p.Files[i] = c;

                }
                return p;
            }

            public void ToStream(BinaryWriter writer, AssetContentPacket packet)
            {
                writer.Write(packet.GUID.ToByteArray());
                writer.Write(packet.Files.Length);
                for (int i = 0; i < packet.Files.Length; i++)
                {
                    var f = packet.Files[i];
                    writer.Write(f.Path);
                    writer.Write(f.Hash.Length);
                    writer.Write(f.Hash);
                }
            }
        }

        public struct FileComponent
        {
            public string Path;
            public byte[] Hash;
            public AssetFileMode Mode;
        }

    }
    public class AssetRequestFilePacket : INetworkPacket
    {
        public Guid GUID;
        public int FileIndex;
    }
    public class AssetFilePartPacket : INetworkPacket
    {
        public byte[] Part;
        public bool Complete;
    }

}

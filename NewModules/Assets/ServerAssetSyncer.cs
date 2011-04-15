using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Files;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Assets
{
    /// <summary>
    /// Make this abstract and create a non-networked implementation
    /// </summary>
    public class ServerAssetSyncer 
    {
        public DirectoryInfo AssetsDirectory { get; private set; }
        private IServerPacketTransporter<AssetRequestPacket> assetContentsRequester;
        private IServerPacketTransporter<AssetContentPacket> assetContent;
        private IServerPacketTransporter<AssetRequestFilePacket> assetRequestFile;

        private ServerFileTransporter<AssetRequestFilePacket> fileTransporter;

        private List<ServerAsset> assets = new List<ServerAsset>();
        private SHA1 sha;

        public ServerAssetSyncer(IServerPacketManager pm, DirectoryInfo assetsDirectory)
        {
            AssetsDirectory = assetsDirectory;
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.generateRandomCacheFile("", "dll"));

            assetContentsRequester = pm.CreatePacketTransporter("AssetContentRequest", gen.GetFactory<AssetRequestPacket>(),
                                                                PacketFlags.TCP);
            assetContentsRequester.EnableReceiveMode();
            assetContent = pm.CreatePacketTransporter("AssetContent", new AssetContentPacket.Factory(),
                                                      PacketFlags.TCP);
            assetRequestFile = pm.CreatePacketTransporter("AssetRequestFile", gen.GetFactory<AssetRequestFilePacket>(),
                                                          PacketFlags.TCP);
            fileTransporter = new ServerFileTransporter<AssetRequestFilePacket>("AssetFileTransporter", pm);
            assetRequestFile.EnableReceiveMode();

            gen.BuildFactoriesAssembly();
            sha = SHA1.Create();
        }

        public void Start()
        {
            fileTransporter.Start();

            var t = new Thread(contentJob);
            t.Name = "ServerAssetSyncerContentJob";
            t.IsBackground = true;
            t.Start();

            t = new Thread(fileJob);
            t.Name = "ServerAssetSyncerFileJob";
            t.IsBackground = true;
            t.Start();


        }

        public void contentJob()
        {
            for (; ; )
            {
                IClient client;
                var p = assetContentsRequester.Receive(out client);

                var asset = GetAsset(p.GUID);
                var retP = new AssetContentPacket();
                retP.GUID = p.GUID;
                retP.Files = new AssetContentPacket.FileComponent[asset.FileComponents.Count];
                for (int i = 0; i < asset.FileComponents.Count; i++)
                {
                    asset.FileComponents[i].UpdateHash(sha);

                    var f = new AssetContentPacket.FileComponent();
                    f.Path = asset.FileComponents[i].Path;
                    f.Hash = asset.FileComponents[i].Hash;
                    f.Mode = asset.FileComponents[i].Mode;
                    retP.Files[i] = f;

                }

                assetContent.GetTransporterForClient(client).Send(retP);
            }


        }

        public void fileJob()
        {
            for (; ; )
            {

                IClient client;
                var p = assetRequestFile.Receive(out client);

                var file = GetAsset(p.GUID).FileComponents[p.FileIndex];

                // Start sending
                fileTransporter.SendFileTo(client, p, file.GetFullPath());

            }


        }


        public ServerAsset CreateAsset(Guid guid)
        {
            if (GetAsset(guid) != null) throw new InvalidOperationException();

            var a = new ServerAsset(this, guid);
            assets.Add(a);

            return a; 
        }
        public ServerAsset CreateAsset()
        {
            var a = new ServerAsset(this, Guid.NewGuid());
            assets.Add(a);

            return a;

        }

        public ServerAsset GetAsset(Guid guid)
        {
            return assets.Find(a => a.GUID.Equals(guid));

        }



        public void SaveAssetInformation()
        {
            var node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "AssetsList");

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                var cNode = node.CreateChildNode("Asset");
                cNode.AddAttribute("Guid", asset.GUID.ToString());
                for (int j = 0; j < asset.FileComponents.Count; j++)
                {
                    var comp = asset.FileComponents[j];
                    var compNode = cNode.CreateChildNode("FileComponent");
                    compNode.AddAttribute("Path", comp.Path);
                    compNode.AddAttribute("Mode", comp.Mode.ToString());
                }
            }


            node.Document.Save(getAssetInformationFilePath());
        }
        public void LoadAssetInformation()
        {
            if (!File.Exists(getAssetInformationFilePath())) return;
            assets.Clear();
            var node = TWXmlNode.GetRootNodeFromFile(getAssetInformationFilePath());
            var nodeChildren = node.GetChildNodes();

            for (int i = 0; i < nodeChildren.Length; i++)
            {
                var cNode = nodeChildren[i];
                var guid = new Guid(cNode.GetAttribute("Guid"));
                var asset = new ServerAsset(this, guid);

                var compNodeChildren = cNode.GetChildNodes();

                for (int j = 0; j < compNodeChildren.Length; j++)
                {
                    var compNode = compNodeChildren[j];
                    var path = compNode.GetAttribute("Path");
                    var mode = (AssetFileMode)Enum.Parse(typeof(AssetFileMode), compNode.GetAttribute("Mode"));
                    var comp = new ServerAssetFile(asset, path, mode);
                    asset.FileComponents.Add(comp);

                }

                assets.Add(asset);
            }


        }


        private string getAssetInformationFilePath()
        {
            return AssetsDirectory + "\\_Assetslist.xml";
        }


    }
}

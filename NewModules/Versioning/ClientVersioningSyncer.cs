using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Files;
using MHGameWork.TheWizards.Networking.Packets;

namespace MHGameWork.TheWizards.Versioning
{
    public class ClientVersioningSyncer
    {
        private readonly VersioningSystem sys;
        private IClientPacketTransporter<RevisionPacket> revisionTransporter;
        private IClientPacketTransporter<BytePacket> commandTransporter;
        private IClientPacketTransporter<RevisionRequestPacket> requestRevisionTransporter;
        private ClientFileTransporter<RevisionFilePacket> fileTransporter;

        public ClientVersioningSyncer(VersioningSystem sys, IClientPacketManager pm)
        {
            this.sys = sys;
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.GenerateRandomCacheFile("CVS", "dll"));
            revisionTransporter = pm.CreatePacketTransporter("Versioning_Revision",
                                                                             gen.GetFactory<RevisionPacket>(), PacketFlags.TCP);
            requestRevisionTransporter = pm.CreatePacketTransporter("Versioning_RequestRevision",
                                                                    gen.GetFactory<RevisionRequestPacket>(), PacketFlags.TCP);
            commandTransporter = pm.CreatePacketTransporter("Versioning_CMD",
                                                            new BytePacket.Factory(), PacketFlags.TCP);

            fileTransporter = new ClientFileTransporter<RevisionFilePacket>("Versioning_File", pm, TWDir.Cache.CreateSubdirectory("Versioning").FullName);
            fileTransporter.StartReceiving();

            gen.BuildFactoriesAssembly();

        }



        public void Push()
        {

        }

        /// <summary>
        /// This gets all the revision from the remote master to 0000 (so only the master branch)
        /// </summary>
        /// <returns></returns>
        public Revision Fetch()
        {
            return retrieveServerMaster();


        }

        private Revision retrieveServerMaster()
        {
            commandTransporter.Send(new BytePacket((byte)SyncerCommands.RequestMaster));
            var p = revisionTransporter.Receive();

            return retrieveRevision(p.Guid);
        }

        private Revision retrieveRevision(Guid guid)
        {
            var rev = sys.FindRevision(guid);
            if (rev != null) return rev;

            requestRevisionTransporter.Send(new RevisionRequestPacket { Guid = guid });

            var p = revisionTransporter.Receive();

            rev = new Revision();
            rev.Guid = p.Guid;
            rev.Message = rev.Message;
            rev.Parent = retrieveRevision(p.Parent);
            rev.MergedParent = retrieveRevision(p.MergedParent);


            // Always 1 file in revision!!!
            for (; ; )
            {
                var fileP = fileTransporter.ReceiveFile();
                File.Move(fileP.CachedFilePath, sys.getFilePath(rev, fileP.Packet.Path));

                if (fileP.Packet.Complete) break;
            }



            return rev;
        }




    }

    public struct RevisionPacket : INetworkPacket
    {
        public Guid Guid;
        public Guid Parent;
        public Guid MergedParent;
        public string Message;

        public RevisionPacket(Revision rev)
        {
            Guid = rev.Guid;
            Parent = rev.Parent.Guid;
            MergedParent = rev.MergedParent.Guid;
            Message = rev.Message;
        }
    }
    public enum SyncerCommands : byte
    {
        None,
        RequestMaster
    }
    public struct RevisionRequestPacket : INetworkPacket
    {
        public Guid Guid;
    }
    public struct RevisionFilePacket : INetworkPacket
    {
        public string Path;
        public bool Complete;
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Files;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.Versioning
{
    public class ServerVersioningSyncer
    {
        private readonly VersioningSystem sys;
        private IServerPacketTransporter<RevisionPacket> revisionTransporter;
        private IServerPacketTransporter<BytePacket> commandTransporter;
        private IServerPacketTransporter<RevisionRequestPacket> requestRevisionTransporter;
        private ServerFileTransporter<RevisionFilePacket> fileTransporter;

        public ServerVersioningSyncer(VersioningSystem sys, IServerPacketManager pm)
        {
            this.sys = sys;
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.GenerateRandomCacheFile("CVS", "dll"));
            revisionTransporter = pm.CreatePacketTransporter("Versioning_Revision",
                                                                             gen.GetFactory<RevisionPacket>(), PacketFlags.TCP);
            requestRevisionTransporter = pm.CreatePacketTransporter("Versioning_RequestRevision",
                                                                    gen.GetFactory<RevisionRequestPacket>(), PacketFlags.TCP);
            requestRevisionTransporter.EnableReceiveCallbackMode(onRequestRevision);
            commandTransporter = pm.CreatePacketTransporter("Versioning_CMD",
                                                            new BytePacket.Factory(), PacketFlags.TCP);
            commandTransporter.EnableReceiveCallbackMode(onCommandReceived);

            fileTransporter = new ServerFileTransporter<RevisionFilePacket>("Versioning_File", pm);

            gen.BuildFactoriesAssembly();



        }

        private void onRequestRevision(IClient client, RevisionRequestPacket packet)
        {
            var rev = sys.FindRevision(packet.Guid);
            if (rev == null) throw new InvalidOperationException("Unexisting revision requested");

            revisionTransporter.GetTransporterForClient(client).Send(new RevisionPacket(rev));

            var di = new DirectoryInfo(sys.getRevisionRoot(rev));
            var e = di.EnumerateFiles("*", SearchOption.AllDirectories).GetEnumerator();

            FileInfo fi = e.Current;

            while (e.MoveNext())
            {
                var p = new RevisionFilePacket { Path = sys.getPathFromFileInfo(rev, fi), Complete = e.Current == null };


                fileTransporter.SendFileTo(client, p, fi.FullName);
                fi = e.Current;

            }


        }

        private void onCommandReceived(IClient client, BytePacket packet)
        {
            var cmd = (SyncerCommands)packet.Data;
            if (cmd == SyncerCommands.RequestMaster)
            {
                var master = sys.CheckoutRevision;
                var p = new RevisionPacket(master);

                revisionTransporter.GetTransporterForClient(client).Send(p);
            }
            else
            {
                throw new InvalidOperationException();
            }
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
            throw new NotImplementedException();
            /*commandTransporter.Send(new BytePacket((byte)SyncerCommands.RequestMaster));
            var p = revisionTransporter.Receive();

            return retrieveRevision(p.Guid);*/
        }

        private Revision retrieveRevision(Guid guid)
        {
            throw new NotImplementedException();

            /*var rev = sys.FindRevision(guid);
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



            return rev;*/
        }




    }



}

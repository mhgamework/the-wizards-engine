using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.Networking.Files
{
    public sealed class ClientFileTransporter<T> where T : INetworkPacket
    {
        public string UniqueName { get; private set; }
        private string downloadCachePath;


        private object receiveLock = new object();

        internal IClientPacketTransporter<FileStartPacket> startTransporter;
        internal IClientPacketTransporter<FilePartPacket> partTransporter;
        internal IClientPacketTransporter<T> endTransporter;

        private int i;


        public ClientFileTransporter(string uniqueName, IClientPacketManager pm, string downloadCachePath)
        {
            this.downloadCachePath = downloadCachePath;
            UniqueName = uniqueName;
            i = (new Random()).Next(0, 1000);

            var gen = new NetworkPacketFactoryCodeGenerater(Application.StartupPath + "\\ClientFileTrans" + uniqueName + i.ToString() + ".dll");

            startTransporter = pm.CreatePacketTransporter("SFTFileStart" + uniqueName, gen.GetFactory<FileStartPacket>(),
                                                              PacketFlags.TCP);
            endTransporter = pm.CreatePacketTransporter("SFTFileComplete" + uniqueName, gen.GetFactory<T>(),
                                                                 PacketFlags.TCP);
            partTransporter = pm.CreatePacketTransporter("SFTFilePart" + uniqueName, gen.GetFactory<FilePartPacket>(),
                                                             PacketFlags.TCP);
            gen.BuildFactoriesAssembly();

            StartReceiving();

        }

        public void StartReceiving()
        {
            var t = new Thread(receiveJob);
            t.Name = UniqueName + "_ReceiveJob";
            t.Start();
        }

        public bool FileAvailable { get { lock (this) return fileReceived; } }

        public ReceivedFileInfo<T> ReceiveFile()
        {
            lock (receiveLock)
            {
                while (!fileReceived)
                    Monitor.Wait(receiveLock);

                var info = new ReceivedFileInfo<T>();
                info.CachedFilePath = receivingFilePath;
                info.Packet = receivedPacket;


                fileReceived = false;
                Monitor.Pulse(receiveLock);
                return info;
            }
        }

        public void SendFile(T packet, string filePath)
        {
            lock (this)
            {
                if (sendingFile != null)
                    throw new InvalidOperationException("A file is already being sent!");

                startTransporter.Send(new FileStartPacket());


                sendingFilePacket = packet;

                sendingFile = new FileInfo(filePath);
                filePosition = 0;
                server.AddDownloadingClient(this);

            }
        }

        public bool CancelFile(T packet)
        {
            throw new NotImplementedException();
        }

        public bool IsSendingFile()
        {
            lock (this)
            {
                return sendingFile != null;
            }
        }

        private FileStream receivingStream;
        private string receivingFilePath;

        private bool fileReceived = false;
        private T receivedPacket;

        private void receiveJob()
        {
            for (; ; )
            {
                lock (receiveLock)
                {
                    while (fileReceived)
                        Monitor.Wait(receiveLock);


                    var start = startTransporter.Receive();
                    receivingFilePath = generateTemporaryFilename();

                    using (
                        receivingStream =
                        File.Open(receivingFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Delete))
                    {
                        for (; ; )
                        {
                            var part = partTransporter.Receive();

                            receivingStream.Write(part.Data, 0, part.Data.Length);

                            if (part.Canceled) break;
                            if (!part.Complete) continue;

                            var end = endTransporter.Receive();
                            receivedPacket = end;

                            fileReceived = true;
                            Monitor.Pulse(receiveLock);
                            break;
                        }

                    }
                }

            }
        }

        private Random random = new Random();

        private string generateTemporaryFilename()
        {
            string filePath;
            do
            {
                filePath = downloadCachePath + "\\" + random.Next(0, int.MaxValue) + ".tmp";
            } while (File.Exists(filePath));

            return filePath;
        }


        // Server component

        private ServerFileTransporter<T> server;

        public IClient Client;

        public TCPConnection Connection;
        private FileInfo sendingFile;
        private long filePosition;
        private FileStream fs;

        private T sendingFilePacket;

        internal ClientFileTransporter(ServerFileTransporter<T> server)
        {
            this.server = server;
        }



        internal byte[] getNextFilePart(int maxSize)
        {
            lock (this)
            {
                if (sendingFile == null) throw new InvalidOperationException();

                if (fs == null)
                {
                    try
                    {
                        fs = new FileStream(sendingFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                if (fs.Position != filePosition) fs.Seek(filePosition, SeekOrigin.Begin);

                if (fs.Length - fs.Position < maxSize) maxSize = (int)(fs.Length - fs.Position);

                var buffer = new byte[maxSize];
                fs.Read(buffer, 0, buffer.Length);
                //buffer = new byte[maxSize];
                filePosition = fs.Position;

                if (fs.Position == fs.Length)
                {
                    fs.Close();
                    fs = null;
                    sendingFile = null;
                }

                return buffer;
            }
        }

        internal int sendNextFilePart(int sendSize)
        {
            var buffer = getNextFilePart(sendSize);
            partTransporter.Send(new FilePartPacket { Data = buffer, Complete = !IsSendingFile() });

            if (!IsSendingFile())
            {
                // complete
                endTransporter.Send(sendingFilePacket);
            }
            return buffer.Length;
        }
    }
}

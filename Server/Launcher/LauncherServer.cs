using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using Launcher;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.Server.Launcher
{
    /// <summary>
    /// TODO: when sending small files, send multiple packets
    /// TODO: remove disconnected clients from the list
    /// </summary>
    public class LauncherServer
    {
        private int bytesPerSec = 160 * 1024;
        private readonly int port;

        private Dictionary<EndPoint, Client> clientsDict = new Dictionary<EndPoint, Client>();
        private List<Client> clients = new List<Client>();
        private TCPConnectionListener listener;

        private LauncherPacketParser packetParser = new LauncherPacketParser();

        private HashedFileList fileList;
        private DirectoryInfo cacheDir;

        public LauncherServer(int port, HashedFileList _fileList)
        {
            this.port = port;
            fileList = _fileList;
            cacheDir = TWDir.Cache.CreateSubdirectory("LauncherServer");
        }

        private void BuildZipCache()
        {
            Console.WriteLine("Building ZIP cache...");
            //Create and cache zip
            for (int i = 0; i < fileList.Files.Count; i++)
            {
                
                var iFile = fileList.Files[i];
                zipToCache(iFile);
                iFile.ZippedSize = getZipCacheFI(iFile).Length;
                
            }

            Console.WriteLine("Cache created!");
        }

        /// <summary>
        /// Only set this before starting the server
        /// </summary>
        public int BytesPerSec
        {
            get { return bytesPerSec; }
            set { bytesPerSec = value; }
        }

        public void Start()
        {
            BuildZipCache();
            listener = new TCPConnectionListener(port);

            listener.ClientConnected += listener_ClientConnected;

            listener.Listening = true;

            var t = new Thread(sendFileJob);
            t.IsBackground = true;
            t.Name = "LauncherServerFileJob";
            t.Start();



        }

        void listener_ClientConnected(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
        {
            var conn = new TCPConnection(e.CL);

            lock (clients)
            {
                var client = new Client();
                client.Connection = conn;
                clientsDict.Add(e.CL.Client.RemoteEndPoint, client);
                clients.Add(client);
            }


            conn.PacketRecievedAsync += conn_PacketRecievedAsync;
            conn.Receiving = true;
        }

        void conn_PacketRecievedAsync(object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e)
        {
            var client = clientsDict[e.EP];
            var conn = client.Connection;

            var type = packetParser.ParseClientPacketType(e.Dgram);
            switch (type)
            {
                case LauncherClientPacketTypes.RequestFileList:
                    var dgram = packetParser.CreateSPacketList(fileList);
                    conn.SendPacket(dgram, TCPPacketBuilder.TCPPacketFlags.None);
                    break;
                case LauncherClientPacketTypes.RequestFile:
                    var relativePath = packetParser.ParseCRequestFile(e.Dgram);

                    //Find this file
                    for (int i = 0; i < fileList.Files.Count; i++)
                    {
                        var iFile = fileList.Files[i];
                        if (iFile.RelativePath != relativePath) continue;
                        //File is downloadable, start downloading

                       

                        client.SendFile(getZipCacheFI(iFile));

                        //Add a downloading client
                        lock (clients)
                        {
                            numberDownloadingClients++;
                            Monitor.Pulse(clients);
                        }

                        break;
                    }

                    break;
            }
        }
        private FileInfo getFileInfo(HashedFileList.File iFile)
        {
            return new FileInfo(fileList.LocalRoot.FullName + "\\" + iFile.RelativePath);
        }
        private FileInfo zipToCache(HashedFileList.File iFile)
        {
            FileInfo fOut = getZipCacheFI(iFile);
            fOut.Directory.Create();
            ZipOutputStream zipOut = new ZipOutputStream(fOut.Create());

            FileInfo fi = getFileInfo(iFile);
            ZipEntry entry = new ZipEntry(fi.Name);
            FileStream sReader = fi.OpenRead();
            byte[] buff = new byte[Convert.ToInt32(sReader.Length)];
            sReader.Read(buff, 0, (int)sReader.Length);
            entry.DateTime = fi.LastWriteTime;
            entry.Size = sReader.Length;
            sReader.Close();
            zipOut.PutNextEntry(entry);
            zipOut.Write(buff, 0, buff.Length);
            zipOut.Finish();
            zipOut.Close();

            return fOut;
        }
        private FileInfo getZipCacheFI(HashedFileList.File iFile)
        {
            return new FileInfo(cacheDir + "\\" + iFile.RelativePath + ".zip");
        }

        private int numberDownloadingClients = 0;

        private void sendFileJob()
        {
            int sendInterval = 100;
            int sendSize = BytesPerSec / (1000 / sendInterval);


            int i = 0;
            for (; ; )
            {

                Client client;
                lock (clients)
                {
                    // Wait until clients are downloading
                    while (numberDownloadingClients == 0)
                        Monitor.Wait(clients);

                    //Adjust index
                    if (i >= clients.Count) i = 0;
                    //Client found!
                    client = clients[i];
                    i++;

                }

                if (!client.IsSendingFile()) continue;

                var buffer = client.GetNextFilePart(sendSize);
                client.Connection.SendPacket(packetParser.CreateSFilePart(buffer), TCPPacketBuilder.TCPPacketFlags.None);

                if (client.IsSendingFile() == false)
                {
                    client.Connection.SendPacket(packetParser.CreateSFileComplete(),
                                                     TCPPacketBuilder.TCPPacketFlags.None);
                    lock (clients)
                    {
                        numberDownloadingClients--;
                    }
                }

                Thread.Sleep(sendInterval); // limit upload rate

            }
        }


        public void Stop()
        {
            listener.Listening = false;
            listener.Dispose();
        }


        public class Client
        {
            public TCPConnection Connection;
            private FileInfo sendingFile;
            private long filePosition;
            private FileStream fs;

            public void SendFile(FileInfo file)
            {
                lock (this)
                {
                    if (sendingFile != null)
                        throw new InvalidOperationException("A file is in the process of downloading!");

                    sendingFile = file;
                    filePosition = 0;
                }


            }

            public bool IsSendingFile()
            {
                lock (this)
                {
                    return sendingFile != null;
                }
            }


            public byte[] GetNextFilePart(int maxSize)
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
        }
    }
}

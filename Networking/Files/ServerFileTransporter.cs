using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.Networking.Files
{
    public class ServerFileTransporter<T> where T : INetworkPacket
    {
        //private int bytesPerSec = 160 * 1024;
        private int bytesPerSec = 250 * 1024;

        private Dictionary<EndPoint, ClientFileTransporter<T>> clientsDict = new Dictionary<EndPoint, ClientFileTransporter<T>>();
        private List<ClientFileTransporter<T>> downloadingClients = new List<ClientFileTransporter<T>>();
        //private TCPConnectionListener listener;

        //private LauncherPacketParser packetParser = new LauncherPacketParser();

        //private HashedFileList fileList;
        private IServerPacketTransporter<FilePartPacket> filePartTransporter;
        private IServerPacketTransporter<FileStartPacket> fileStartTransporter;
        private IServerPacketTransporter<T> fileCompleteTransporter;

        private Dictionary<IClient, ClientFileTransporter<T>> clientFIleTransporterMap =
            new Dictionary<IClient, ClientFileTransporter<T>>();

        public ServerFileTransporter(string uniqueName, IServerPacketManager pm)
        {
            var gen = new NetworkPacketFactoryCodeGenerater(Application.StartupPath + "\\ServerFileTrans" + uniqueName + ".dll");

            fileStartTransporter = pm.CreatePacketTransporter("SFTFileStart" + uniqueName, gen.GetFactory<FileStartPacket>(),
                                                              PacketFlags.TCP);
            fileCompleteTransporter = pm.CreatePacketTransporter("SFTFileComplete" + uniqueName, gen.GetFactory<T>(),
                                                                 PacketFlags.TCP);
            filePartTransporter = pm.CreatePacketTransporter("SFTFilePart" + uniqueName, gen.GetFactory<FilePartPacket>(),
                                                             PacketFlags.TCP);
            gen.BuildFactoriesAssembly();
            //fileList = _fileList;
            //cacheDir = TWDir.Cache.CreateSubdirectory("LauncherServer");
        }


        /*private void BuildZipCache()
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
        }*/

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
            //BuildZipCache();
            /*listener = new TCPConnectionListener(port);

            listener.ClientConnected += listener_ClientConnected;

            listener.Listening = true;*/

            var t = new Thread(sendFileJob);
            t.IsBackground = true;
            t.Name = "LauncherServerFileJob";
            t.Start();


        }

        public ClientFileTransporter<T> GetClientTransporter(IClient client)
        {
            ClientFileTransporter<T> ret;
            if (!clientFIleTransporterMap.TryGetValue(client, out ret))
            {
                ret = new ClientFileTransporter<T>(this);
                clientFIleTransporterMap[client] = ret;
                ret.startTransporter = fileStartTransporter.GetTransporterForClient(client);
                ret.partTransporter = filePartTransporter.GetTransporterForClient(client);
                ret.endTransporter = fileCompleteTransporter.GetTransporterForClient(client);

                //ret.StartReceiving(); //TODO: only suppport sending ATM
            }
            return ret;
        }


        private FileInfo getFileInfo(HashedFileList.File iFile)
        {
            /*return new FileInfo(fileList.LocalRoot.FullName + "\\" + iFile.RelativePath);*/
            throw new NotImplementedException();
        }
        private FileInfo zipToCache(HashedFileList.File iFile)
        {
            /*FileInfo fOut = getZipCacheFI(iFile);
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

            return fOut;*/
            throw new NotImplementedException();
        }
        private FileInfo getZipCacheFI(HashedFileList.File iFile)
        {
            throw new NotImplementedException();
            /*return new FileInfo(cacheDir + "\\" + iFile.RelativePath + ".zip");*/
        }


        private void sendFileJob()
        {
            int sendInterval = 100;
            int sendSize = (int)(BytesPerSec / (1000f / sendInterval));


            int i = 0;
            int bytesSent = 0;
            for (; ; )
            {

                ClientFileTransporter<T> client;
                lock (downloadingClients)
                {
                    // Wait until clients are downloading
                    while (downloadingClients.Count == 0)
                    {
                        Monitor.Wait(downloadingClients);
                        bytesSent = 0;
                    }

                    //Adjust index
                    if (i >= downloadingClients.Count) i = 0;
                    //Client found!
                    client = downloadingClients[i];
                    i++;

                }

                if (!client.IsSendingFile()) continue;

                bytesSent += client.sendNextFilePart(sendSize);



                // WARNING: this lock really needs to be before the IsSendingFile check!
                lock (downloadingClients)
                {
                    if (client.IsSendingFile() == false)
                    {
                        downloadingClients.Remove(client);
                        //numberDownloadingClients--;
                    }
                }
                if (bytesSent >= sendSize)
                    Thread.Sleep(sendInterval); // limit upload rate
                bytesSent = 0;
            }
        }


        public void Stop()
        {
        }


        public void AddDownloadingClient(ClientFileTransporter<T> cl)
        {
            lock (downloadingClients)
            {
                downloadingClients.Add(cl);
                Monitor.Pulse(downloadingClients);
            }
        }



        public void SendFileTo(IClient client, T identifierPacket, string filePath)
        {
            var t = GetClientTransporter(client);
            t.SendFile(identifierPacket, filePath);
        }



    }

    /// <summary>
    /// Temp
    /// </summary>
    internal class HashedFileList
    {
        public class File
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.SharpZipLib.Zip;
using Launcher;
using MHGameWork.TheWizards.Common.Networking;
using TCPConnection = MHGameWork.TheWizards.Networking.TCPConnection;
using TCPPacketBuilder = MHGameWork.TheWizards.Networking.TCPPacketBuilder;

namespace MHGameWork.TheWizards.Launcher
{
    /// <summary>
    /// TODO: Report server disconnects
    /// </summary>
    public class LauncherMain
    {
        private volatile Dispatcher dispatcher;
        private LauncherPacketParser packetParser = new LauncherPacketParser();
        private readonly string rootPath;
        private MainWindowViewModel vm;
        private Application app;
        private DirectoryInfo cachePath;

        private List<IPEndPoint> serverList = new List<IPEndPoint>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath">The rootpath of The Wizards folder. This is currently where the binaries should be located</param>
        public LauncherMain(string rootPath)
        {
            this.rootPath = rootPath;
            cachePath = TWDir.Cache.CreateSubdirectory("Launcher");
        }

        public LauncherMain(string ip, int port, string rootPath)
            : this(rootPath)
        {
            AddServer(new System.Net.IPEndPoint(IPAddress.Parse(ip), port));

        }

        /// <summary>
        /// NON THREAD SAFE, CALL BEFORE RUN
        /// </summary>
        /// <param name="endpoint"></param>
        public void AddServer(IPEndPoint endpoint)
        {
            serverList.Add(endpoint);
        }

        private void updateClient()
        {
            if (serverList.Count == 0)
            {
                setStatus("No servers to connect to!!");
                return;
            }

            var ready = new AutoResetEvent(false);


            TCPConnection conn = null;

            for (int i = 0; i < serverList.Count; i++)
            {
                var server = serverList[i];

                if (i == 0)
                    setStatus("Connecting to launcher server (" + server + ")...");
                else
                    setStatus(string.Format("Connection failed({0}). Connecting to different server ({1})...", serverList[i - 1], server));

                conn = new TCPConnection();
                conn.ConnectError += delegate(object sender, TCPConnection.ConnectErrorEventArgs e)
                {
                    setStatus("Connection failed: (" + e.Ex.Message + ")");
                    Console.WriteLine(e.Ex.Message);
                    ready.Set();
                };
                conn.NetworkErrorAsync += delegate(object sender, BaseConnection.NetworkErrorEventArgs e) { throw e.Ex; };


                conn.ConnectedToServer += delegate { ready.Set(); };
                conn.Connect(server);

                ready.WaitOne();
                if (conn.TCP.Connected)
                    break;
                else
                {
                    conn.Dispose();
                    conn = null;
                }

            }
            if (conn == null)
            {
                setStatus("Unable to connect to a launcher server!");
                return;
            }


            conn.Receiving = true;
            conn.SendPacket(packetParser.CreateCRequestFileList(), TCPPacketBuilder.TCPPacketFlags.None);

            HashedFileList list = null;

            conn.PacketRecievedAsync += delegate(object sender, BaseConnection.PacketRecievedEventArgs e)
            {
                if (packetParser.ParseServerPacketType(e.Dgram) != LauncherServerPacketTypes.FileList)
                    return;

                list = packetParser.ParseSFileList(e.Dgram);



                ready.Set();

                /*Assert.AreEqual(LauncherServerPacketTypes.FilePart,
                packetParser.ParseServerPacketType(e.Dgram));
                var part = packetParser.ParseSFilePart(e.Dgram);
                fs.Write(part, 0, part.Length);*/

            };

            setStatus("Retrieving File List...");
            ready.WaitOne();

            var updateFiles = new List<HashedFileList.File>();
            long updateSize = 0;
            var p = new SHA1CryptoServiceProvider();


            // Check corrupt/outdated files
            for (int i = 0; i < list.Files.Count; i++)
            {
                setStatus(string.Format("Checking local files ({0}/{1})...", i + 1, list.Files.Count));
                var file = list.Files[i];

                byte[] hash;

                FileInfo fi = createFileInfo(file);
                if (fi.Exists)
                {
                    using (var tfs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    { hash = p.ComputeHash(tfs); }

                    if (hash.SequenceEqual(file.HashSHA1))
                        continue;
                }
                updateFiles.Add(file);
                updateSize += file.ZippedSize;

            }

            long downloaded = 0;
            int[] downloadingFile = { 0 }; // trick (access to modified closure problem)


            var fileCompleteEvent = new AutoResetEvent(false);
            FileStream fs = null;

            conn.PacketRecievedAsync += delegate(object sender, BaseConnection.PacketRecievedEventArgs e)
            {

                if (packetParser.ParseServerPacketType(e.Dgram) == LauncherServerPacketTypes.FileComplete)
                {
                    Console.WriteLine("Complete");
                    fileCompleteEvent.Set();
                    return;
                }
                if (packetParser.ParseServerPacketType(e.Dgram) == LauncherServerPacketTypes.FilePart)
                {

                    var part = packetParser.ParseSFilePart(e.Dgram);
                    fs.Write(part, 0, part.Length);

                    downloaded += part.Length;

                    setStatus(getDownloadingStatusText(downloaded, updateSize, downloadingFile, updateFiles));

                    dispatcher.Invoke(new Action(delegate
                                                     {
                                                         vm.Progress = (double)downloaded / updateSize * 100;
                                                     }));



                }



            };

            setStatus("Starting download...");

            try
            {
                for (int i = 0; i < updateFiles.Count; i++)
                {
                    downloadingFile[0] = i;

                    var file = updateFiles[i];

                    var fi = createCacheZipFileInfo(file);
                    fi.Directory.Create();

                    fs = new FileStream(fi.FullName, FileMode.Create);

                    Console.WriteLine("Start");
                    conn.SendPacket(packetParser.CreateCRequestFile(file.RelativePath), TCPPacketBuilder.TCPPacketFlags.None);

                    fileCompleteEvent.WaitOne();

                    fs.Close();
                    fs = null;

                    // Unpack

                    setStatus(getDownloadingStatusText(downloaded, updateSize, downloadingFile, updateFiles) +
                              " - Extracting ...");

                    var zip = new FastZip();
                    var target = createFileInfo(file);
                    zip.ExtractZip(fi.FullName, target.Directory.FullName, FastZip.Overwrite.Always,
                        delegate { return true; }
                        , "", "",
                                   true);


                }

            }
            catch (Exception ex)
            {
                if (fs != null)
                    fs.Close();

                setStatus("Error while attemtping to update files! (" + ex.Message + ")");
                return;
            }

            setStatus("The Wizards is Up to Date!");


            dispatcher.Invoke(new Action(delegate
            {
                vm.Progress = (double)downloaded / updateSize * 100;
                vm.PlayEnabled = true;
            }));



            /*if (!ready.WaitOne(5000))
                throw new Exception("Timeout");*/


            /* fs.Close();


             var p = new SHA1CryptoServiceProvider();

             byte[] hash1;
             byte[] hash2;

             using (var fs1 = new FileStream("ServerClient.exe", FileMode.Open, FileAccess.Read, FileShare.Read))
             { hash1 = p.ComputeHash(fs1); }

             using (var fs2 = new FileStream(targetFilePath, FileMode.Open))
             { hash2 = p.ComputeHash(fs2); }

             Assert.AreEqual(hash1, hash2);*/
        }

        private string getDownloadingStatusText(long downloaded, long updateSize, int[] downloadingFile, List<HashedFileList.File> updateFiles)
        {
            return string.Format("Downloading {0} / {1} Kb, File ({2}/{3})...",
                                 downloaded / 1024,
                                 updateSize / 1024,
                                 downloadingFile[0] + 1,
                                 updateFiles.Count);
        }

        private FileInfo createCacheZipFileInfo(HashedFileList.File file)
        {
            return new FileInfo(cachePath + "\\" + file.RelativePath + ".zip");
        }

        private FileInfo createFileInfo(HashedFileList.File file)
        {
            return new FileInfo(rootPath + "\\" + file.RelativePath);
        }

        private void setStatus(string status)
        {
            dispatcher.Invoke(new Action(delegate { vm.Status = status; }));
        }



        public void Run()
        {
            var window = new MainWindow();
            vm = new MainWindowViewModel(new RunTWCommand(this));
            window.DataContext = vm;

            app = new Application();

            dispatcher = app.Dispatcher;


            ThreadPool.QueueUserWorkItem(delegate { updateClient(); });

            app.Run(window);


        }

        private class RunTWCommand : ICommand
        {
            private readonly LauncherMain main;

            public RunTWCommand(LauncherMain main)
            {
                this.main = main;
            }

            public void Execute(object parameter)
            {
                main.vm.Status = "Starting The Wizards";
                System.Threading.Thread.Sleep(1000);
                Process.Start(main.rootPath + "\\Binaries\\ServerClient.exe");

                main.app.Shutdown();

            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}

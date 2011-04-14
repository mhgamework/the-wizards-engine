using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;
using Launcher;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.TheWizards.Server.Launcher;
using MHGameWork.TheWizards.Tests.Launcher;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using TCPConnection = MHGameWork.TheWizards.Networking.TCPConnection;
using TCPPacketBuilder = MHGameWork.TheWizards.Networking.TCPPacketBuilder;

namespace MHGameWork.TheWizards.Tests.Server
{
    /// <summary>
    /// This tests the launcher server using dummy launcher code
    /// </summary>
    [TestFixture]
    public class LauncherServerTest
    {
        [Test]
        public void TestConnect()
        {
            LauncherServer server = new LauncherServer(15015, new HashedFileList());
            server.Start();

            var conn = new TCPConnection();
            var connected = new AutoResetEvent(false);

            conn.ConnectedToServer += delegate
                                      {
                                          connected.Set();
                                          conn.SendPacket(new byte[] { 10, 11 }, TCPPacketBuilder.TCPPacketFlags.None);

                                      };
            conn.ConnectError += delegate(object sender, TCPConnection.ConnectErrorEventArgs e) { throw e.Ex; };


            conn.Connect("127.0.0.1", 15015);


            if (!connected.WaitOne(2000))
                throw new Exception("Connection timed out");

        }


        [Test]
        public void TestRequestFileList()
        {
            var list = LauncherTest.CreateTestFileList();

            var server = new LauncherServer(15015, list);
            var packetParser = new LauncherPacketParser();
            server.Start();

            var ready = new AutoResetEvent(false);


            var conn = new TCPConnection();

            conn.ConnectedToServer += delegate
            {
                conn.SendPacket(packetParser.CreateCRequestFileList(), TCPPacketBuilder.TCPPacketFlags.None);
                conn.Receiving = true;


            };
            conn.ConnectError += delegate(object sender, TCPConnection.ConnectErrorEventArgs e) { throw e.Ex; };
            conn.NetworkErrorAsync += delegate(object sender, BaseConnection.NetworkErrorEventArgs e) { throw e.Ex; };
            conn.PacketRecievedAsync += delegate(object sender, BaseConnection.PacketRecievedEventArgs e)
                                        {
                                            Assert.AreEqual(LauncherServerPacketTypes.FileList,
                                                            packetParser.ParseServerPacketType(e.Dgram));
                                            var list2 = packetParser.ParseSFileList(e.Dgram);

                                            ready.Set();

                                        };

            conn.Connect("127.0.0.1", 15015);

            if (!ready.WaitOne(2000))
                throw new Exception("Timeout");

            server.Stop();
        }

        [Test]
        public void TestRequestFile()
        {
            var list = LauncherTest.CreateTestFileList();

            var server = new LauncherServer(15015, list);
            var packetParser = new LauncherPacketParser();
            server.Start();

            var ready = new AutoResetEvent(false);


            var conn = new TCPConnection();

            conn.ConnectedToServer += delegate
            {
                conn.Receiving = true;
                conn.SendPacket(packetParser.CreateCRequestFile("Binaries\\ServerClient.exe"), TCPPacketBuilder.TCPPacketFlags.None);
            };

            conn.ConnectError += delegate(object sender, TCPConnection.ConnectErrorEventArgs e) { throw e.Ex; };
            conn.NetworkErrorAsync += delegate(object sender, BaseConnection.NetworkErrorEventArgs e) { throw e.Ex; };

            string targetFilePath = TWDir.Test + "\\TestRequestFile\\ServerClient.exe.zip";
            (new FileInfo(targetFilePath)).Directory.Create();

            var fs = new FileStream(targetFilePath, FileMode.Create);

            conn.PacketRecievedAsync += delegate(object sender, BaseConnection.PacketRecievedEventArgs e)
            {
                if (packetParser.ParseServerPacketType(e.Dgram) == LauncherServerPacketTypes.FileComplete)
                {
                    ready.Set();
                    return;
                }

                Assert.AreEqual(LauncherServerPacketTypes.FilePart,
                                packetParser.ParseServerPacketType(e.Dgram));
                var part = packetParser.ParseSFilePart(e.Dgram);
                fs.Write(part, 0, part.Length);

            };

            conn.Connect("127.0.0.1", 15015);

            ready.WaitOne();
            /*if (!ready.WaitOne(5000))
                throw new Exception("Timeout");*/


            fs.Close();

            server.Stop();

            var zip = new FastZip();
            var target = new FileInfo(targetFilePath);
            zip.ExtractZip(targetFilePath, target.Directory.FullName, FastZip.Overwrite.Always,
                delegate { return true; }
                , "", "",
                           true);


            var p = new SHA1CryptoServiceProvider();

            byte[] hash1;
            byte[] hash2;

            using (var fs1 = new FileStream("ServerClient.exe", FileMode.Open, FileAccess.Read, FileShare.Read))
            { hash1 = p.ComputeHash(fs1); }

            using (var fs2 = new FileStream(targetFilePath.Substring(0, targetFilePath.Length - 4), FileMode.Open))
            { hash2 = p.ComputeHash(fs2); }

            Assert.AreEqual(hash1, hash2);

        }


        [Test]
        public void TestLauncherServer()
        {
            var list = new HashedFileList();
            list.LocalRoot = new System.IO.DirectoryInfo(System.Windows.Forms.Application.StartupPath);
            list.LocalRoot = list.LocalRoot.Parent;

            list.AddFolder(new System.IO.DirectoryInfo(list.LocalRoot.FullName + "\\Binaries"));
            list.AddFolder(new System.IO.DirectoryInfo(list.LocalRoot.FullName + "\\GameData"), true);


            var server = new LauncherServer(15014, list);
            server.BytesPerSec = 300 * 1024;
            server.Start();

            var app = new Application();
            app.Run();

            server.Stop();
        }

    }
}

using System.IO;
using MHGameWork.TheWizards.Networking.Files;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Networking
{
    [TestFixture]
    public class NetworkingFilesTest
    {
        [Test]
        public void TestTransportFile()
        {
            var path = TWDir.Test + "\\TestAsset.txt";
            var strm = new StreamWriter(File.OpenWrite(path));
            strm.Write("This is a test asset!");
            strm.Close();

            var spm = new SimpleServerPacketManager();
            var cpm = spm.CreateClient();

            var server = new ServerFileTransporter<DataPacket>("TestTrans", spm);
            server.Start();
            var client = new ClientFileTransporter<DataPacket>("TestTrans", cpm, TWDir.Test.CreateSubdirectory("TestTransportFileCache").FullName);

            var ct = server.GetClientTransporter(spm.Clients[0]);



            ct.SendFile(new DataPacket(), path);

            var info = client.ReceiveFile();



            // Test TestAsset file

            var lines = File.ReadAllLines(info.CachedFilePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("This is a test asset!", lines[0]);
        }
    }
}

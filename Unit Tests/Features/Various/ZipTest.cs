using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class ZipTest
    {
        [Test]
        public void TestZip()
        {
            string sPath = "ServerClient.exe";
            var destFile = @"..\Test\Zip\ServerClient.exe.zip";
            FileInfo fOut = new FileInfo(destFile);
            fOut.Directory.Create();
            ZipOutputStream zipOut = new ZipOutputStream(fOut.Create());

            FileInfo fi = new FileInfo(sPath);
            ZipEntry entry = new ZipEntry(fi.Name);
            FileStream sReader = File.OpenRead(sPath);
            byte[] buff = new byte[Convert.ToInt32(sReader.Length)];
            sReader.Read(buff, 0, (int)sReader.Length);
            entry.DateTime = fi.LastWriteTime;
            entry.Size = sReader.Length;
            sReader.Close();
            zipOut.PutNextEntry(entry);
            zipOut.Write(buff, 0, buff.Length);
            zipOut.Finish();
            zipOut.Close();


            //Load
            ZipInputStream zipIn = new ZipInputStream(File.OpenRead(destFile));
            FileStream streamWriter = File.Create(@"..\Test\Zip\ServerClient.exe");
            entry = zipIn.GetNextEntry();
            long size = entry.Size;
            byte[] data = new byte[size];
            while (true)
            {
                size = zipIn.Read(data, 0, data.Length);
                if (size > 0) streamWriter.Write(data, 0, (int)size);
                else break;
            }
            streamWriter.Close();

            Console.WriteLine("Done!!");
        }
    }
}

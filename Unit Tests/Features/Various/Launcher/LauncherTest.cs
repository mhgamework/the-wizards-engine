using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Launcher;
using MHGameWork.TheWizards.Launcher;
using MHGameWork.TheWizards.Server.Launcher;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Launcher
{
    [TestFixture]
    public class LauncherTest
    {
        [Test]
        public void TestForm()
        {
            var application = new System.Windows.Application();
            var w = new MainWindow();
            var viewModel = new MainWindowViewModel();

            viewModel.Progress = 20;
            viewModel.Status = "DAN - Doing Absolutely Nothing!";

            w.DataContext = viewModel;



            application.Run(w);
        }

        [Test]
        public void TestHashedFileList()
        {
            HashedFileList list = CreateTestFileList();


            var di = new DirectoryInfo("..\\Test");
            di.Create();

            XmlSerializer serializer = new XmlSerializer(typeof(HashedFileList));

            using (var fs = new FileStream("..\\Test\\TestHashedFileList.xml", FileMode.Create))
            {
                serializer.Serialize(fs, list);
            }
            HashedFileList list2;
            using (var fs = new FileStream("..\\Test\\TestHashedFileList.xml", FileMode.Open))
            {
                list2 = serializer.Deserialize(fs) as HashedFileList;
            }



        }

        public static HashedFileList CreateTestFileList()
        {
            var diBinaries = new System.IO.DirectoryInfo(Application.StartupPath);
            var list = new HashedFileList();
            list.LocalRoot = diBinaries.Parent;

            list.AddFolder(diBinaries);
            return list;
        }


        [Test]
        public void TestLauncherLocal()
        {
            var list = LauncherTest.CreateTestFileList();

            list.Files.Remove(list.Files.Find(o => o.RelativePath == "Unit Tests.dll"));

            var server = new LauncherServer(15015, list);
            server.BytesPerSec = 300 * 1024;
            server.Start();

            Directory.CreateDirectory("..\\Test\\TestLauncher");

            var main = new LauncherMain("127.0.0.1", 15015, "..\\Test\\TestLauncher");
            main.Run();


            server.Stop();
        }

        [Test]
        public void TestLauncher()
        {
            MHGameWork.TheWizards.Launcher.Program.Main();
        }


    }
}

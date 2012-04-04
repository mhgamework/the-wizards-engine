using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using Launcher;
using MHGameWork.TheWizards.Main;
using MHGameWork.TheWizards.Server.Launcher;
using MHGameWork.TheWizards.Utilities;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests
{
    [TestFixture]
    public class MainTest
    {
        [Test]
        public void RunClient()
        {
            var client = new TheWizardsClient();
            client.Run();
        }
        [Test]
        public void RunServer()
        {
            var server = new TheWizardsServer();
            server.Start();
        }
        [Test]
        public void RunLauncherServer()
        {
            var list = new HashedFileList();
            list.LocalRoot = new System.IO.DirectoryInfo(System.Windows.Forms.Application.StartupPath);
            list.LocalRoot = list.LocalRoot.Parent;

            list.AddFolder(new System.IO.DirectoryInfo(list.LocalRoot.FullName + "\\Binaries"));
            list.AddFolder(new System.IO.DirectoryInfo(list.LocalRoot.FullName + "\\GameData\\Core"), true);


            var server = new LauncherServer(15014, list);
            server.BytesPerSec = 300 * 1024;
            server.Start();

            var app = new Application();
            app.Run();

            server.Stop();
        }

        [Test]
        public void RunServerClient()
        {
            var server = new TheWizardsServerCore();
            server.Start();

            var client = new TheWizardsClient();
            client.AttachServerCore(server);

            client.Run();
        }

        [Test]
        public void RunServer2Clients()
        {
            var server = new TheWizardsServerCore();
            server.Start();


            var client1 = new TheWizardsClient();
            var client2 = new TheWizardsClient();

            client1.AttachServerCore(server);

            client1.EnableSingleStepMode();
            client2.EnableSingleStepMode();

            new Thread(client1.Run) { Name = "Client 1" }.Start();
            client1.WaitUntilInitialized();

            new Thread(client2.Run) { Name = "Client 2" }.Start();
            client2.WaitUntilInitialized();

            while (true)
            {
                client1.StepSingle();
                client2.StepSingle();
            }

        }
    }
}

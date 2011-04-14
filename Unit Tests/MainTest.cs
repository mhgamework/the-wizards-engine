using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Main;
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
        public void TestClient()
        {
            var p = TestRunner.RunTestInOtherProcess("MHGameWork.TheWizards.Tests.MainTest.RunServer");
            System.Threading.Thread.Sleep(5000);
            RunClient();
            if (!p.HasExited)
                p.Kill();


        }

        [Test]
        public void TestServer()
        {
            var server = new TheWizardsServer();
            Process p = null;
            ThreadPool.QueueUserWorkItem(delegate
            {
                while (!server.IsReady)
                    System.Threading.Thread.Sleep(500);

                p = TestRunner.RunTestInOtherProcess("MHGameWork.TheWizards.Tests.MainTest.RunClient");
            });

            server.Start();

            if (p != null) p.Kill();

        }


    }
}

using System;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    /// <summary>
    /// Test some of the windsor utilities
    /// </summary>
    [TestFixture]
    public class WindsorTest
    {
        [Test]
        public void TestDebugWriting()
        {
            var container = new WindsorContainer();
            container.Install(new PrototypeInstaller());

            var tools = new WindsorDebugTools();
            var str = tools.GenerateDependenciesString(container); //.Mark();

            Console.WriteLine(str);
        }
    }
}
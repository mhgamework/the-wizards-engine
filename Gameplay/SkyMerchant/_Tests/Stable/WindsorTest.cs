using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using MHGameWork.TheWizards.SkyMerchant._Tests.Development;
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

        [Test]
        public void TestDisablePropertyWiring()
        {
            var container = new WindsorContainer();
            container.Register(Classes.FromThisAssembly().InSameNamespaceAs<TestPropertyComponent>().WithServiceSelf().DisablePropertyWiring());
            var comp = container.Resolve<TestPropertyComponent>();
            Assert.IsNull(comp.Component);
        }
    }
}
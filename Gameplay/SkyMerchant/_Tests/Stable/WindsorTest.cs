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
    [Category("RunsAutomated")]
    public class WindsorTest
    {
        private WindsorDebugTools tools;

        public WindsorTest()
        {
            tools = new WindsorDebugTools();
        }

        [Test]
        public void TestDebugWriting()
        {
            var container = new WindsorContainer();
            container.Install(new PrototypeInstaller());

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

        [Test]
        public void TestWindsorLogging()
        {
            var container = new WindsorContainer();
            tools.EnableCreationLogging(container);
            container.Register(Component.For<WindsorTest>());
            container.Register(Component.For<Object>().LifestyleTransient());
            container.Resolve<WindsorTest>();
            container.Resolve<WindsorTest>();
            container.Resolve<Object>();
            container.Resolve<Object>();

            //Test that console now shows output for this creation, but only once

            /**
            WINDSOR Created MHGameWork.TheWizards.SkyMerchant._Tests.Stable.WindsorTest
            WINDSOR Created System.Object
            WINDSOR Created System.Object
             **/


        }
    }
}
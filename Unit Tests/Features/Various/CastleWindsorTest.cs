using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class CastleWindsorTest
    {
        [Test]
        public void Test()
        {
            //application starts...
            var container = new WindsorContainer();

            // adds and configures all components using WindsorInstallers from executing assembly
            container.Install(new RepositoriesInstaller());

            // instantiate and configure root component and all its dependencies and their dependencies and...
            var king = container.Resolve<IMyClass>();

            var plain = container.Resolve<MyPlainClass>();

            // clean up, application exits
            container.Dispose();

        }

        public class RepositoriesInstaller : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore store)
            {

                // This registers every class in this namespace as a component, for the interfaces matching the component's name (eg I**) and for the component itself
                container.Register(Classes.FromThisAssembly()
                                    .Where(Component.IsInSameNamespaceAs<MyClass>())
                                    .WithService.Self().WithService.DefaultInterfaces()
                                    .LifestyleTransient());
                

               
                
            }
        }

        public interface IMyClass
        {

        }

        public class MyClass : IMyClass
        {
            
        }

        public class MyPlainClass
        {
            
        }
    }
}
using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._Tests.Stable;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Installs the game objects (using the component based system). Each game object gets a set of components.
    /// </summary>
    public class GameObjectsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Register(Component.For<Physical>().ImplementedBy<Physical>().Forward<IPositionComponent>().LifestyleTransient());
            container.Register(Component.For<Physical>().ImplementedBy<Physical>().LifestyleScoped<ObjectPartScope>());
            container.Register(Component.For<IWorldObject>().ImplementedBy<WorldObject>().LifestyleScoped<ObjectPartScope>());

            container.Register(
                Component.For<IPositionComponent>()
                         .ImplementedBy<WorldObjectPhysicalPart>()
                         .LifestyleScoped<ObjectPartScope>());
            //container.Register(Component.For<IPositionComponent>().ImplementedBy<WorldObjectPhysicalPart>().UsingFactoryMethod(
            //    delegate(IKernel k)
            //    {
            //        var ph = k.Resolve<Physical>();
            //        var world = new WorldObject(ph);
            //        return new WorldObjectPhysicalPart(world, ph);
            //    }).LifestyleTransient());
            //container.Register(Component.For<IPositionComponent>().ImplementedBy<Physical>().LifestyleTransient());

            // Override!!!
            container.Register(Component.For<RobotPlayerPart>().LifestyleSingleton());

            // Register all object parts
            container.Register(
                Classes.FromThisAssembly().InSameNamespaceAs<IslandPart>().WithServiceSelf().LifestyleTransient());


        }
    }

    public class ObjectPartScope : IScopeAccessor
    {
        public void Dispose()
        {
            
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            return null;
        }
    }

}
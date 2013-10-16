using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.GameObjects;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using MHGameWork.TheWizards.SkyMerchant.DataStructures;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    /// <summary>
    /// Installs the game objects (using the component based system). Each game object gets a set of components.
    /// </summary>
    public class GameObjectsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (!container.Kernel.GetFacilities().Any(f => f is TypedFactoryFacility))
                container.AddFacility<TypedFactoryFacility>();

            // Register types for component construction
            container.Register(Component.For<GameObjectScopeManager>());
            container.Register(Component.For<GameObjectsRepository>().Forward<IGameObjectsRepository>());
            container.Register(Component.For<IGameObjectComponentTypedFactory>().AsFactory());

            // Register the *Component implementations in the GameObjects namespace
            container.Register(
                Classes.FromThisAssembly()
                       .Where(t => t.Namespace == typeof (GameObject).Namespace && t.Name.EndsWith("Component"))
                       .WithService.AllInterfaces());

            // Register the SkyPhysical
            container.Register(Component.For<SkyPhysical>());

            // Set lifetimescope for all IGameObjectComponent's
            container.Register(Classes.FromThisAssembly().BasedOn<IGameObjectComponent>().WithServiceSelf().LifestyleScoped<GameObjectScope>());

            //Legacy: Register all object parts
            container.Register(
                Classes.FromThisAssembly().InSameNamespaceAs<IslandPart>().WithServiceSelf().LifestyleScoped<GameObjectScope>());




            //container.Register(Component.For<Physical>().ImplementedBy<Physical>().Forward<IPositionComponent>().LifestyleTransient());
            //container.Register(Classes.FromThisAssembly().BasedOn<IGameComponent>().WithServiceAllInterfaces().LifestyleScoped<Accessor>());
            //container.Register(
            //    Component.For<Physical>().ImplementedBy<Physical>().LifestyleCustom<PerGameObjectLifestyleManager>());
            //container.Register(Component.For<IWorldObject>().ImplementedBy<WorldObject>().LifestyleScoped<ObjectPartScope>());

            //container.Register(
            //    Component.For<IPositionComponent>()
            //             .ImplementedBy<WorldObjectPhysicalPart>()
            //             .LifestyleScoped<ObjectPartScope>());
            //container.Register(Component.For<IPositionComponent>().ImplementedBy<WorldObjectPhysicalPart>().UsingFactoryMethod(
            //    delegate(IKernel k)
            //    {
            //        var ph = k.Resolve<Physical>();
            //        var world = new WorldObject(ph);
            //        return new WorldObjectPhysicalPart(world, ph);
            //    }).LifestyleTransient());
            //container.Register(Component.For<IPositionComponent>().ImplementedBy<Physical>().LifestyleTransient());

            // Override!!!
            //container.Register(Component.For<RobotPlayerPart>().LifestyleSingleton());

            

        }
    }
    /// <summary>
    /// TODO: this is a messy implementation (using the singleton). Thing about maybe using .Resolve arguments or find a better way
    /// </summary>
    public class GameObjectScope : IScopeAccessor
    {
        private Dictionary<IGameObject, ILifetimeScope> scopes = new Dictionary<IGameObject, ILifetimeScope>();
        public GameObjectScope()
        {
        }

        public void Dispose()
        {
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            if (GameObjectScopeManager.ActiveGameObjectScope == null) throw new InvalidOperationException("A game object scope must be set before objects can be created");
            
            return scopes.GetOrCreate(GameObjectScopeManager.ActiveGameObjectScope, () => new DefaultLifetimeScope());
        }
    }
}
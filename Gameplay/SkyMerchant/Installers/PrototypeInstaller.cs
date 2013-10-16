using System;
using Castle.MicroKernel;
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
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Installers
{
    public class PrototypeInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<NonOptionalPropertiesFacility>();
            if (!container.Kernel.GetFacilities().Any(f => f is TypedFactoryFacility))
                container.AddFacility<TypedFactoryFacility>();


            container.Register(Component.For<PrototypeTest>());

            container.Register(
                Component.For<ITypedFactory>().AsFactory(),
                //Component.For<IslandMeshFactory>(),
                Component.For<VoxelMeshBuilder>(),
                Component.For<TraderPart.IItemFactory>().ImplementedBy<SimpleItemFactory>()
                );



            var randomCount = 0;
            container.Register(
                Component.For<Random>()
                        .LifestyleTransient()
                        .UsingFactoryMethod((k, ctx) => new Random(randomCount++))
                );

            container.Register(
                Component.For<TWEngine>().UsingFactoryMethod(EngineFactory.CreateEngine)
                );




            container.Register(
                Component.For<ISimulationEngine>().ImplementedBy<SimpleSimulationEngine>(),
                Component.For<RobotPlayerNormalMovementPart.IUserMovementInput>().ImplementedBy<SimpleUserMovementInput>()
                );

            container.Register(
                Component.For<CustomCamera>());

            container.Register(
                Component.For<EnemyBehaviourFactory>().LifestyleTransient());

            // Register all components as a service in itself or similarly named interfaces
            container.Register(Classes.FromThisAssembly().InNamespace("MHGameWork.TheWizards.SkyMerchant").WithServiceSelf().WithServiceDefaultInterfaces());
            container.Register(Classes.FromThisAssembly().InNamespace("MHGameWork.TheWizards.SkyMerchant.Prototype").WithServiceSelf().WithServiceDefaultInterfaces());
            container.Register(Classes.FromThisAssembly().InNamespace("MHGameWork.TheWizards.SkyMerchant._Engine").WithServiceSelf().WithServiceDefaultInterfaces());
        }
    }
}
using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using MHGameWork.TheWizards.SkyMerchant._Windsor;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class PrototypeInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<NonOptionalPropertiesFacility>();
            container.AddFacility<TypedFactoryFacility>();

            container.Register(
                Component.For<PrototypeTest>(),
                Component.For<PrototypeWorldGenerator>(),
                Component.For<ObjectsFactory>(),
                Component.For<ITypedFactory>().AsFactory(),
                Component.For<IslandMeshFactory>(),
                Component.For<VoxelMeshBuilder>()



                );

            container.Register(
                Component.For<Random>()
                        .LifestyleTransient()
                        .UsingFactoryMethod((k, ctx) => new Random(0))
                );

            container.Register(
                Component.For<TWEngine>().UsingFactoryMethod(EngineFactory.CreateEngine)
                );


            container.Register(
                Classes.FromThisAssembly().InSameNamespaceAs<IslandPart>().WithServiceSelf().LifestyleTransient());

            container.Register(
                Component.For<Physical>().LifestyleTransient());
        }
    }
}
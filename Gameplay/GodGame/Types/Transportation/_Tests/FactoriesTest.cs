using Autofac;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Configuration.MainConfigurations;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Generic;
using MHGameWork.TheWizards.GodGame._Tests;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation._Tests
{
    [TestFixture]
    [EngineTest]
    public class FactoriesTest
    {
        private Internal.Model.World world;
        private VoxelTypesFactory types;
        private ItemTypesFactory items;
        private GenericVoxelType<BasicFactory> basicFactory;
        private GenericVoxelType<ConstantFactory> constantFactory;
        private GameVoxel v55;
        private GameVoxel v56;
        private GameVoxel v57;
        private GameVoxel v46;
        private GameVoxel v36;
        private GenericVoxelType<Pusher> pusher;
        private GenericVoxelType<Puller> puller;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<OfflineGameModule>();
            builder.RegisterType<Internal.Model.World>().SingleInstance().AsSelf()
                   .OnActivating(args =>
                   {
                       args.Instance.Initialize(10, 10);

                       var land = args.Context.Resolve<LandType>();
                       args.Instance.ForEach((v, p) => v.ChangeType(land));
                   });


            var cont = builder.Build();

            var offline = cont.Resolve<GodGameOffline>();
            offline.AddSimulatorsToEngine(EngineFactory.CreateEngine());
            world = offline.World;
            types = cont.Resolve<VoxelTypesFactory>();
            items = cont.Resolve<ItemTypesFactory>();

            basicFactory = new GenericVoxelType<BasicFactory>(v => new BasicFactory(v));
            constantFactory = new GenericVoxelType<ConstantFactory>(v => new ConstantFactory(v));
            pusher = new GenericVoxelType<Pusher>(v => new Pusher(v));
            puller = new GenericVoxelType<Puller>(v => new Puller(v));

            v55 = world.GetVoxel(new Point2(5, 5));
            v56 = world.GetVoxel(new Point2(5, 6));
            v57 = world.GetVoxel(new Point2(5, 7));

            v46 = world.GetVoxel(new Point2(4, 6));
            v36 = world.GetVoxel(new Point2(3, 6));


        }

        private GameVoxel v(int x, int y)
        {
            return world.GetVoxel(new Point2(x, y));
        }

        [Test]
        public void TestBasicAndConstant()
        {
            

            v55.ChangeType(constantFactory);
            var fact = ((IVoxel)v55).GetPart<ConstantFactory>();
            fact.Rate = 1;
            fact.ItemsToGenerate = new[] { items.CropType };

            v57.ChangeType(basicFactory);
            var fact2 = ((IVoxel)v57).GetPart<BasicFactory>();
            fact2.EfficiencySpeedMultiplier = 1;
            fact2.Input = new[]
                                                                {
                                                                    items.CropType, items.CropType,
                                                                    items.CropType
                                                                };
            fact2.Output = new[] { items.PigmentType };


            v56.ChangeType(types.Get<WarehouseType>());

            v46.ChangeType(pusher);

            v(3, 6).ChangeType(types.Get<RoadType>());
            v(2, 6).ChangeType(puller);
            v(1, 6).ChangeType(types.Get<WarehouseType>());

        }

    }
}
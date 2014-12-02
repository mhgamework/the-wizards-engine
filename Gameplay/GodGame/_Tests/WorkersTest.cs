using System.Collections.Generic;
using Autofac;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Configuration.MainConfigurations;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.Types.Towns;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    public class WorkersTest
    {
        [Test]
        public void TestScenario()
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

            builder.RegisterType<ScenarioGame>().SingleInstance();

            var cont = builder.Build();

            var offline = cont.Resolve<ScenarioGame>();


        }

        [Test]
        public void TestRecordScenario()
        {
            runTestGame();
        }


        private static GodGameOffline runTestGame()
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

            var engine = EngineFactory.CreateEngine();
            offline.AddSimulatorsToEngine(engine);

            return offline;
        }

        private class ScenarioGame
        {
            private readonly Internal.Model.World world;
            private readonly TownCenterType townCenterType;
            private readonly TownCenterService townCenterService;
            private readonly MinerType minerType;

            public ScenarioGame(GodGameOffline offline, TWEngine engine, Internal.Model.World world,
                TownCenterType townCenterType, TownCenterService townCenterService,
                MinerType minerType)
            {
                this.world = world;
                this.townCenterType = townCenterType;
                this.townCenterService = townCenterService;
                this.minerType = minerType;
                offline.AddSimulatorsToEngine(engine);

                int skipFrames = 0;
                var scenarioTasks = Run().GetEnumerator();
                bool complete = false;
                engine.AddSimulator(() =>
                    {
                        skipFrames--;
                        if (skipFrames > 0) return;
                        if (!scenarioTasks.MoveNext())   complete = true;

                        if (complete) return;

                        skipFrames = scenarioTasks.Current;


                    }, "ScenarioSimulator");
            }

            public IEnumerable<int> Run()
            {
                world.GetVoxel(new Point2(4, 4)).ChangeType(townCenterType);
                yield return 0;
                world.GetVoxel(new Point2(2,2)).ChangeType(minerType);
                yield return 0;
                var town = townCenterService.GetTownForVoxel(world.GetVoxel(new Point2(4, 4)));
                town.AddVoxel(world.GetVoxel(new Point2(4, 5)));
                yield return 0;
            }
        }
    }
}
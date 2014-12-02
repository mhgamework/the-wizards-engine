using Autofac;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Configuration.MainConfigurations;
using MHGameWork.TheWizards.GodGame.Types;
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

            var cont = builder.Build();

            var offline = cont.Resolve<GodGameOffline>();

            var engine = EngineFactory.CreateEngine();
            offline.AddSimulatorsToEngine(engine);
        }
    }
}
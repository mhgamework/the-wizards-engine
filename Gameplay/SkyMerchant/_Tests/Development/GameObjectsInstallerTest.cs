using Castle.Windsor;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class GameObjectsInstallerTest
    {
        private IWindsorContainer container;

        public GameObjectsInstallerTest()
        {
            var ctx = new TW.Context();
            ctx.Data = new DataWrapper(new EngineTraceLogger());
            TW.SetContext(ctx);

            container = new WindsorContainer();
            container.Install(
                new GameObjectsInstaller(),
                new PrototypeInstaller(),
                new WorldingInstaller(),
                new EngineInstaller());
        }

        [Test]
        public void TestCreateIsland()
        {
            var fact = container.Resolve<ObjectsFactory>();
            var island = fact.CreateIsland();

            Assert.NotNull(island.IslandMeshFactory);
            Assert.NotNull(island.Physical);
            Assert.NotNull(island.Physics);
        }

        [Test]
        public void TestCreatePhysicalPart()
        {
            var ph = container.Resolve<IPhysicalPart>();



        }
    }
}
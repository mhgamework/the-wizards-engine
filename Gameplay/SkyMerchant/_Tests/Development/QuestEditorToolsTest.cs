using System;
using Castle.Core.Internal;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Facilities;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    [EngineTest]
    [TestFixture]
    public class QuestEditorToolsTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestIslandTool()
        {
            var tool = new IslandToolItem(createObjectsFactory(), new Random());
            tool.OnSelected();

            engine.AddSimulator(new BasicSimulator(tool.Update));
            engine.AddSimulator(new BasicSimulator(() => TW.Data.Objects.OfType<IslandPart>().ForEach(i => i.FixPhysical())));
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestMeshSpawnerItem()
        {
            var tool = new MeshSpawnerItem("Core\\Barrel01", createObjectsFactory());
            tool.OnSelected();

            engine.AddSimulator(new BasicSimulator(tool.Update));
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private ObjectsFactory createObjectsFactory()
        {
            var meshFactory = new IslandMeshFactory(new VoxelMeshBuilder());
            var typeFactory = Substitute.For<ITypedFactory>();
            typeFactory.CreateIsland().Returns(i => new IslandPart() { IslandMeshFactory = meshFactory });
            typeFactory.CreatePhysical().Returns(i => new Physical());
            var ret = new ObjectsFactory(typeFactory);
            return ret;
        }
    }
}
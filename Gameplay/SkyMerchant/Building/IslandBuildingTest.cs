using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using NUnit.Framework;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Building
{
    /// <summary>
    /// 
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class IslandBuildingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private IslandMeshFactory fact;
        private IslandPart islandPart;
        private SimpleIsland island;
        private SimpleRobotPlayer robot;
        private SimpleItemType cogType;

        [SetUp]
        public void Setup()
        {
            fact = new IslandMeshFactory(new VoxelMeshBuilder());
            islandPart = new IslandPart()
                {
                    IslandMeshFactory = fact,
                    Physical = new Physical(),
                    Physics = new BasicPhysicsPart(),
                    Seed = 0
                };
            island = new SimpleIsland(islandPart, fact);

            robot = new SimpleRobotPlayer(island);

            cogType = new SimpleItemType() { Name = "Cog", Mesh = TW.Assets.LoadMesh("SkyMerchant\\Cogs\\Cog01") };

            islandPart.FixPhysical();

            CreateItem(cogType);

        }

        [Test]
        public void Test()
        {
            engine.AddSimulator(new BasicSimulator(delegate
                {
                    if (robot.SelectedItem == null)
                    {
                        // If robot has no selected item, add a new one
                        robot.SelectedItem = CreateItem(cogType); //----->   Jasper was hier <--------
                        robot.SelectedItem.Physical.Visible = false;
                    }
                    if (!island.BuildMode)
                        foreach (var i in TW.Data.Objects.OfType<SimpleItem>().Where(i => i.Machine == null && robot.SelectedItem != i).ToArray())
                        {
                            // Delete all items not in machine when 
                            TW.Data.RemoveObject(i);
                            i.Physical.Visible = false;
                        }
                }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private SimpleItem CreateItem(IItemType type)
        {
            return new SimpleItem(island) { Type = type };
        }
    }
}
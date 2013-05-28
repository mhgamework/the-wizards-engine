using System;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    /// <summary>
    /// Provides some methods to construct test worlds
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class WorldBuilderTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();


        // Dependency
        EditorConfiguration controller = new EditorConfiguration();

        /// <summary>
        /// Build a testscene!!
        /// </summary>
        [Test]
        public void TestBuild()
        {
            setupMenu();

            engine.AddSimulator(new WorldInputtingSimulator(controller));
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());


            DI.Get<TestSceneBuilder>().Setup = delegate
                {
                    TW.Data.Get<CubedTerrain>().CreateGrid(20, 20);
                };
        }

        [Test]
        public void TestRenderTerrain()
        {
            DI.Get<TestSceneBuilder>().Setup = delegate
                {
                    TerrainCube c;
                    c = new TerrainCube() { Elevated = false };
                    c.Physical.WorldMatrix = Matrix.Translation(20, 0, 0);
                    c = new TerrainCube() { Elevated = true };
                    c.Physical.WorldMatrix = Matrix.Translation(0, 0, 0);
                };
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private void setupMenu()
        {
            var config = new EditorMenuConfiguration();

            var terrainSelectProvider = BoundingBoxSelectableProvider.Create(
                       TW.Data.Objects.OfType<TerrainCube>(),
                       t => t.Physical.GetBoundingBox(),
                       delegate(TerrainCube t)
                       {
                           t.Elevated = !t.Elevated;
                       });

            config.CreateItem("Terrain", delegate
                {
                    
                    terrainSelectProvider.Enabled = true;

                    controller.SelectableProvider = terrainSelectProvider;
                    controller.Placer = null;
                });

            config.CreateItem("Trees", delegate
                {
                    var placer = new WorldPlacer
                        (
                        getItems: () => TW.Data.Objects.OfType<Tree>(),
                        getPosition: tree => ((Tree)tree).Position,
                        setPosition: (tree, position) => ((Tree)tree).Position = position,
                        getBoundingBox: tree => ((Tree)tree).Physical.GetBoundingBox(),
                        createItem: () => new Tree(),
                        deleteItem: t => TW.Data.RemoveObject((Tree)t)
                        );
                    controller.Placer = placer;
                    controller.SelectableProvider = null;
                    terrainSelectProvider.Enabled = false;
                });

            controller.Menu = config;
        }

    }
}
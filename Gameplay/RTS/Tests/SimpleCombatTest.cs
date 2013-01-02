using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS.Various;
using NUnit.Framework;

namespace MHGameWork.TheWizards.RTS
{
    /// <summary>
    /// Contains test methods to test combat!
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class SimpleCombatTest
    {
        /// <summary>
        /// Allows building levels!
        /// </summary>
        [Test]
        public void LoadLevelBuilder()
        {
            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new GoblinCombatBuilderSimulator());
            engine.AddSimulator(new LightPlacementSimulator());
            engine.AddSimulator(new TerrainEditorSimulator());
            loadRenderers(engine);
        }

        [Test]
        public void TestBattle()
        {
            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new TerrainEditorSimulator());
            loadRenderers(engine);
        }



        private void loadRenderers(TWEngine engine)
        {
            engine.AddSimulator(new GoblinRendererSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new PointLightSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}

using System;
using System.Collections.Generic;
using Castle.Windsor;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Gameplay.Scripts;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.Scripting;
using MHGameWork.TheWizards.SkyMerchant.SimulationPausing;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using NUnit.Framework;
using Rhino.Mocks;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// Tests the scripting functionality
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class ScriptingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();


        /// <summary>
        /// Attaches a script to a WorldObject to make it rotate
        /// </summary>
        [Test]
        public void TestSimpleScript()
        {
            var ph = new Physical();
            ph.Mesh = TW.Assets.LoadMesh("Core\\Crate01");
            throw new NotImplementedException();
            //var obj = new WorldObject(ph);
            //obj.Scripts.Add(new RotateOnTheSpotScript(new SimpleSimulationEngine()));

            engine.AddSimulator(new ScriptsSimulator());
            engine.AddSimulator(new SkyMerchantRenderingSimulator());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}
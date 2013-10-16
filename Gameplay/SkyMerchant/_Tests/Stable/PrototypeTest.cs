using System;
using Castle.Windsor;
using DirectX11;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant.Lod;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using NUnit.Framework;
using System.Linq;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    [EngineTest]
    [TestFixture]
    public class PrototypeTest
    {

        /// <summary>
        /// Starts the prototype!
        /// </summary>
        [Test]
        public void TestDIPrototype()
        {
            var ctx = new TW.Context();
            ctx.Data = new DataWrapper(new EngineTraceLogger());
            ctx.Assets = new AssetsWrapper();
            TW.SetContext(ctx);

            var container = BootstrapContainer();

            container.Resolve<PrototypeTest>();
        }


        /// <summary>
        /// Starts the prototype!
        /// </summary>
        [Test]
        public void TestPlayPrototype()
        {
            var container = BootstrapContainer();

            var prototype = container.Resolve<PrototypeTest>();
            prototype.Run();
        }

      

        #region Injection
        [NonOptional]
        public TWEngine Engine { get; set; }
        [NonOptional]
        public PrototypeWorldGenerator PrototypeWorldGenerator { get; set; }

        [NonOptional]
        public PrototypeUserInterface UserInterface { get; set; }

        [NonOptional]
        public PlayerRobotSimulator PlayerRobotSimulator { get; set; }

        #endregion




        public PrototypeTest()
        {

        }

        private void Run()
        {
            DI.Get<TestSceneBuilder>().Setup = setupTest;
            Engine.AddSimulator(new BasicSimulator(simulate));
            Engine.AddSimulator(new SkyMerchantRenderingSimulator());
            Engine.AddSimulator(new PhysicalSimulator());
            Engine.AddSimulator(new WorldRenderingSimulator());

            Engine.Run();
        }

        private void simulate()
        {
            if (TW.Debug.LastException != null) return;



            foreach (var i in TW.Data.Objects.OfType<IslandPart>())
            {
                //i.SimulateFloatForce();
                //i.SimulateIslandAirDrag();
                //i.SimulateIslandShieldCollision();
                //i.SimulateIslandWrapping();
                //i.SimulateMovement();
            }

            foreach (var i in TW.Data.Objects.OfType<GenerationSourcePart>())
            {
                i.SimulateGeneration();
            }
            foreach (var i in TW.Data.Objects.OfType<ProximityChaseEnemyPart>())
            {
                i.SimulateBehaviour();
            }

            foreach (var i in TW.Data.Objects.OfType<TraderPart>().ToArray())
            {
                i.SimulateResourcesGeneration();
            }
            foreach (var i in TW.Data.Objects.OfType<PiratePart>().ToArray()) // To array because this removes modelobjects
            {
                i.SimulateBehaviour();
            }
            PlayerRobotSimulator.SimulateRobotNonUserInput();
            PlayerRobotSimulator.SimulateRobotUserInput();
            UserInterface.Update();
        }


        private void setupTest()
        {
            PrototypeWorldGenerator.GenerateWorld(150);

            PlayerRobotSimulator.ActivateRobotCamera();
        }


        public IWindsorContainer BootstrapContainer()
        {
            return new WindsorContainer()
                .Install(new GameObjectsInstaller())
                .Install(new PrototypeInstaller())
                .Install(new EngineInstaller())
                .Install(new WorldingInstaller())
                ;
        }
    }
}
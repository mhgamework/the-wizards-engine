using System;
using System.Threading;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Tests.Gameplay;
using NUnit.Framework;

namespace VoxelEngine.Tests
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CustomStaAttribute : RequiresSTAAttribute
    {

    }
    [EngineTest]
    [TestFixture]
    [CustomSta]

    public class EngineTestFixture
    {
        protected TWEngine engine;

        [SetUp]
        public void InitializeEngine()
        {
            engine = EngineFactory.CreateEngine();
            engine.DontLoadPlugin = true;

            engine.Initialize();

            EngineFactory.Instance = new EngineContext(engine);

            engine.AddSimulator(new EngineUISimulator());
        }

        [TearDown]
        public void RunEngine()
        {
            //engine.AddSimulator(new TestUISimulator());
            engine.AddSimulator(new UISimulator());

            TW.Graphics.Run();
        }

        private class EngineContext : EngineFactory.EngineFactoryContext
        {
            private TWEngine engine;

            public EngineContext(TWEngine engine)
            {
                this.engine = engine;
            }

            public override TWEngine CreateEngine()
            {
                return engine;
            }
        }
    }
}
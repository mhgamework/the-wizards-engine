using System;
using System.Linq;
using System.Threading;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Tests.Gameplay;
using NUnit.Framework;
using SlimDX.DirectInput;
using VoxelEngine.Tests.Engine;
using VoxelEngine.Tests.Engine.FacadesImplementation;

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
        private DeveloperConsole developerConsole;

        [SetUp]
        public void InitializeEngine()
        {
            engine = EngineFactory.CreateEngine();
            engine.DontLoadPlugin = true;

            engine.Initialize();


            // This is somewhat of the DI wiring part

            EngineFactory.Instance = new EngineContext(engine);

            engine.AddSimulator(new EngineUISimulator());
            addConsoleSim();
        }

        private void addConsoleSim()
        {
            var delegateCommandProvider = new DelegateCommandProvider();
            delegateCommandProvider.addCommand("helpall",
                                                () =>
                                                {
                                                    return "All Commands: " +
                                                           string.Join(", ", delegateCommandProvider.CommandNames.ToArray());
                                                });
            delegateCommandProvider.addCommand("help",
                                                partialCommand =>
                                                {
                                                    return "Commands containing '" + partialCommand + "': " +
                                                           string.Join(", ",
                                                                        delegateCommandProvider.CommandNames.Where(
                                                                            c => c.Contains(partialCommand)).ToArray());
                                                });
            var developerConsoleSimulator = new DeveloperConsoleSimulator(delegateCommandProvider);
            engine.AddSimulator(developerConsoleSimulator);

            developerConsole = new DeveloperConsole(developerConsoleSimulator.ConsoleUi, delegateCommandProvider);
        }

        [TearDown]
        public void RunEngine()
        {
            //engine.AddSimulator(new TestUISimulator());
            engine.AddSimulator(new UISimulator());

            TW.Graphics.Run();
        }

        public T Resolve<T>() where T : class
        {
            if (typeof(T) == typeof(IDeveloperConsole))
                return developerConsole as T;

            throw new Exception("Dependency not found!");
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
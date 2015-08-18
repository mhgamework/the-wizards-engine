using System;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Tests.Facades;
using MHGameWork.TheWizards.Engine.Tests.FacadesImplementation;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Internal;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Engine.Tests
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CustomStaAttribute : RequiresSTAAttribute
    {

    }

    [TestFixture]
    public class TWTestFixture
    {
        public DirectoryInfo TestDirectory { get { return TWDir.Test.CreateSubdirectory(TestContext.CurrentContext.Test.FullName); } }

    }

    /// <summary>
    /// Base test fixture that initializes the engine and provides TW testing functionality
    /// </summary>
    [EngineTest]

    [CustomSta]
    public class EngineTestFixture : TWTestFixture
    {
        protected TWEngine engine;
        private DeveloperConsole developerConsole;
        private TestInfoUserinterface testInfoUserinterface;


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

            testInfoUserinterface = new TestInfoUserinterface(TW.Graphics);
            engine.AddSimulator(new BasicSimulator(testInfoUserinterface.Update), "TestInfoUserInterface");
            //TestContext.CurrentContext.TestDirectory
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
            if (typeof(T) == typeof(TestInfoUserinterface))
                return testInfoUserinterface as T;

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
﻿using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.SkyMerchant.GameObjects;
using MHGameWork.TheWizards.SkyMerchant.Installers;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using NSubstitute;
using NUnit.Framework;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    [Category("RunsAutomated")]
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
                new EngineInstaller()
                );
            // Register all IGameObjectComponent's in the Gameplay assembly
            container.Register(Classes.FromThisAssembly().Where(t => typeof(IGameObjectComponent).IsAssignableFrom(t) && t.Namespace.StartsWith("MHGameWork.TheWizards.SkyMerchant._Tests"))
                .WithServiceSelf()
                .WithServiceAllInterfaces()
                .LifestyleScoped<GameObjectScope>()     // Scope to gameobjects!
                .DisablePropertyWiring()                // Disable property wiring!
                );
        }

        /// <summary>
        /// Tests instantiation of the components in the _GameplayInterfaces
        /// </summary>
        [Test]
        public void TestCreateCoreComponents()
        {
            testCreateComponent<IMeshRenderComponent>();
            testCreateComponent<IPositionComponent>();
            testCreateComponent<IRelativePositionComponent>();
        }
        private void testCreateComponent<T>() where T : IGameObjectComponent
        {
            var obj = createGameObject();
            var ph = obj.GetComponent<T>();

            var obj2 = createGameObject();
            var ph2 = obj2.GetComponent<T>();

            Assert.AreNotEqual(ph, ph2);
        }

        [Test]
        public void TestCreatePositionComponent()
        {
            var obj = createGameObject();
            var ph = obj.GetComponent<IPositionComponent>();

            var obj2 = createGameObject();
            var ph2 = obj2.GetComponent<IPositionComponent>();

            Assert.AreNotEqual(ph,ph2);
        }

        [Test]
        public void TestCreateMeshRenderComponent()
        {
            var obj = createGameObject();
            var ph = obj.GetComponent<IMeshRenderComponent>();
        }

        private IGameObject createGameObject()
        {
            var repo = container.Resolve<IGameObjectsRepository>();

            return repo.CreateGameObject();
        }

        [Test]
        public void TestSimpleScopedComponent()
        {
            var acc = container.Resolve<GameObjectScopeManager>();
            acc.SetActiveGameObject(Substitute.For<IGameObject>());

            var comp = container.Resolve<TestComponent>();

            Assert.AreEqual(comp, container.Resolve<TestComponent>());
            Assert.AreEqual(comp.SubComponentA.SubSubComponent,comp.SubComponentB.SubSubComponent);


            acc.SetActiveGameObject(Substitute.For<IGameObject>());

            var comp2 = container.Resolve<TestComponent>();
            Assert.AreNotEqual(comp,comp2);
            Assert.AreNotEqual(comp.SubComponentA,comp2.SubComponentA);
            Assert.AreNotEqual(comp.SubComponentB.SubSubComponent, comp2.SubComponentB.SubSubComponent);
        }

       

        /// <summary>
        /// Test that components are correctly associated with a game object, and dependencies are correctly injected.
        /// </summary>
        [Test]
        public void TestGameObjectsRepository()
        {
            var repo = container.Resolve<IGameObjectsRepository>();

            repo.CreateGameObject();
            var obj = repo.CreateGameObject();
            var obj2 = repo.CreateGameObject();

            var comp = obj.GetComponent<TestSubSubComponent>();

            Assert.AreEqual(comp, obj.GetComponent<TestSubSubComponent>());

            var comp2 = obj.GetComponent<TestSubComponentA>();
            Assert.AreEqual(comp, comp2.SubSubComponent);


            Assert.AreNotEqual(comp,obj2.GetComponent<TestSubSubComponent>());
        }


      
    }

    public class TestPropertyComponent : IGameObjectComponent
    {
        public TestSubComponentA Component { get; set; }
    }
    public class TestComponent : IGameObjectComponent
    {
        public TestSubComponentA SubComponentA { get; set; }
        public TestSubComponentB SubComponentB { get; set; }

        public TestComponent(TestSubComponentA subComponentA, TestSubComponentB subComponentB)
        {
            SubComponentA = subComponentA;
            SubComponentB = subComponentB;
        }
    }
    public class TestSubComponentA : IGameObjectComponent
    {
        public TestSubSubComponent SubSubComponent { get; set; }

        public TestSubComponentA(TestSubSubComponent subSubComponent)
        {
            SubSubComponent = subSubComponent;
        }
    }
    public class TestSubComponentB : IGameObjectComponent
    {
        public TestSubSubComponent SubSubComponent { get; set; }

        public TestSubComponentB(TestSubSubComponent subSubComponent)
        {
            SubSubComponent = subSubComponent;
        }
    }
    public class TestSubSubComponent : IGameObjectComponent
    {

    }
}
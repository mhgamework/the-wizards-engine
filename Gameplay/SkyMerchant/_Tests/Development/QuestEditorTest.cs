using System;
using System.Collections.Generic;
using Castle.Windsor;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using MHGameWork.TheWizards.SkyMerchant.SimulationPausing;
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
    /// 
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class QuestEditorTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();


        /// <summary>
        /// Places camera,picks up item, moves camera, drops item, moves camera
        /// Item should follow when picked up
        /// </summary>
        [Test]
        public void TestPickupMoveDrop()
        {
            AutomatedTestingExtensions.ActiveEngine = new SimpleAutomatedTestingEngine();

            var item = createTestItem().Mark();
            SpectaterCamera cam = getCamera();
            var mover = new WorldObjectMover(cam);

            addTestSimulation(delegate
                {
                    cam.CameraPosition = new Vector3(10, 10, 10);
                    cam.CameraDirection = Vector3.UnitX;
                    cam.UpdateCameraInfo();

                    AutomatedTestingExtensions.Check();

                    mover.Pickup(item);

                    AutomatedTestingExtensions.Check();

                    cam.CameraPosition = new Vector3(0, 10, 10);
                    cam.UpdateCameraInfo();

                    AutomatedTestingExtensions.Check();

                    cam.CameraDirection = Vector3.UnitZ;
                    cam.UpdateCameraInfo();

                    AutomatedTestingExtensions.Check();

                    mover.Drop();

                    cam.CameraPosition = new Vector3();
                    cam.CameraDirection = Vector3.UnitX;
                    cam.UpdateCameraInfo();

                    AutomatedTestingExtensions.Check();



                });

            addTestSimulation(delegate
                {
                    mover.Update();
                });

            addTestVisualization(delegate
                {
                    var color = new Color4(1, 0, 0);
                    if (mover.HoldingItem != null)
                        color = new Color4(0, 1, 0);
                    TW.Graphics.LineManager3D.AddViewFrustum(cam.ViewProjection, color);
                    // Render position of item
                    TW.Graphics.LineManager3D.AddBox(item.CalculateWorldBoundingBox(), new Color4(0, 0, 0));
                });


        }

        /// <summary>
        /// Picks up a set of items of different sizes. Items should appear at different distances from the camera, all inside the view frustum
        /// </summary>
        [Test]
        public void TestMoverSizeBasedDistance()
        {
            AutomatedTestingExtensions.ActiveEngine = new SimpleAutomatedTestingEngine();

            SpectaterCamera cam = getCamera();
            var mover = new WorldObjectMover(cam);

            var items = new List<IMutableSpatial>().Mark();
            foreach (var size in new[] { 0.1f, 1, 5, 10, 40 })
            {
                items.Add(new SimpleMutableSpatial(new BoundingBox(MathHelper.One * -size, MathHelper.One * size)));
            }

            foreach (var i in items)
            {
                mover.Pickup(i);
                mover.Update();
            }

            addTestSimulation(delegate
            {
                AutomatedTestingExtensions.Check();
            });

            addTestVisualization(delegate
            {
                TW.Graphics.LineManager3D.AddViewFrustum(cam.ViewProjection, new Color4(0, 1, 0));
                // Render position of item
                foreach (var i in items)
                    TW.Graphics.LineManager3D.AddBox(i.CalculateWorldBoundingBox(), new Color4(0, 0, 0));
            });


        }

        [Test]
        public void TestHotkeybar()
        {
            var bar = new Hotbar();
            var view = new HotbarTextView(bar, new Rendering2DComponentsFactory());
            var controller = new HotbarController(bar, view);


            bar.SetHotbarItem(0, CreateHotbarItem("Island"));
            bar.SetHotbarItem(1, CreateHotbarItem("Scripting"));
            bar.SetHotbarItem(5, CreateHotbarItem("Bridge"));
            bar.SetHotbarItem(8, CreateHotbarItem("Spawn drone"));


            addTestSimulation(controller.Update);
        }


        [Test]
        public void TestQuestEditorController()
        {
            //TODO: test this
            var container = new WindsorContainer();
            container.Install(new DefaultInventoryInstaller());
            container.Install(new QuestEditorInstaller());
            container.Install(new PrototypeInstaller());
            container.Install(new EngineInstaller());

            var controller = container.Resolve<QuestEditorController>();


            engine.AddSimulator(new BasicSimulator(controller.Update));
            engine.AddSimulator(new SkyMerchantRenderingSimulator());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }






        private IHotbarItem CreateHotbarItem(string name)
        {
            var ret = MockRepository.GenerateStub<IHotbarItem>();
            ret.Stub(i => i.Name).Return(name);

            ret.Stub(i => i.OnDeselected()).Do(new Action(() => Console.WriteLine("Deselected: " + name)));
            ret.Stub(i => i.OnSelected()).Do(new Action(() => Console.WriteLine("Selected: " + name)));
            return ret;
        }













        private void addTestVisualization(Action action)
        {
            engine.AddSimulator(new BasicSimulator(action));
        }

        private IMutableSpatial createTestItem()
        {
            return new SimpleMutableSpatial(new BoundingBox(-MathHelper.One, MathHelper.One));
        }

        private SpectaterCamera getCamera()
        {
            return new SpectaterCamera(TW.Graphics.Keyboard, TW.Graphics.Mouse, 0.5f, 100);
        }

        private void addTestSimulation(Action action)
        {
            var wrapper = StaticPauser.CreateWrapper(action, new SimpleThreadFactory());
            engine.AddSimulator(new BasicSimulator(wrapper.Execute));
        }




    }


    public static class AutomatedTestingExtensions
    {
        public static IAutomatedTestingEngine ActiveEngine { get; set; }

        public static T Mark<T>(this T obj) where T : class
        {
            ActiveEngine.Mark(obj);
            return obj;
        }

        public static void Check()
        {
            ActiveEngine.Check();
        }
    }

    public interface IAutomatedTestingEngine
    {
        void Mark<T>(T obj) where T : class;
        void Check();
    }

    /// <summary>
    /// Simply pauses on checks!
    /// </summary>
    public class SimpleAutomatedTestingEngine : IAutomatedTestingEngine
    {
        public void Mark<T>(T obj) where T : class
        {
        }

        public void Check()
        {
            //printMarks();
            var cont = false;
            while (!cont)
            {
                StaticPauser.Pause();
                cont = TW.Graphics.Keyboard.IsKeyPressed(Key.Return);
            }
        }
    }

    /// <summary>
    /// WARNING this only supports using it on a single thread!
    /// Supports nested pausables.
    /// </summary>
    public static class StaticPauser
    {
        private static object syncLock = new object();
        private static StaticPausingWrapper currentPauser;

        public static void Pause()
        {
            if (currentPauser == null) throw new InvalidOperationException("Not currently in pausable code! (Or calling from another thread??)");
            currentPauser.Pause();
        }
        public static PausingWrapper CreateWrapper(Action method, IThreadFactory factory)
        {
            return new StaticPausingWrapper(method, factory);
        }

        private class StaticPausingWrapper : PausingWrapper
        {
            public StaticPausingWrapper(Action pausableMethod, IThreadFactory simpleThreadFactory)
                : base(pausableMethod, simpleThreadFactory)
            {
            }
            public override void Execute()
            {
                lock (syncLock)
                {
                    var oldPauser = currentPauser;
                    currentPauser = this;
                    base.Execute();
                    currentPauser = oldPauser;

                }
            }
        }
    }
}
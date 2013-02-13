using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class RTSRenderingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            TestUtilities.CreateGroundPlane();
        }

        [Test]
        public void TestRenderAll()
        {
            createTrees();
            createRocks();
            createDroppedThing();

            engine.AddSimulator(new RTSEntitySimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        [Test]
        public void TestRenderTree()
        {
            createTrees();

            engine.AddSimulator(new BasicSimulator(delegate
            {
                TW.Data.EnsureAttachment<Tree, TreeRenderData>(o => new TreeRenderData(o));
                foreach (Tree t in TW.Data.GetChangedObjects<Tree>()) t.get<TreeRenderData>().Update();
            }));

            engine.AddSimulator(new WorldRenderingSimulator());
        }


        [Test]
        public void TestRenderRock()
        {
            createRocks();
            engine.AddSimulator(new BasicSimulator(delegate
                {
                    TW.Data.EnsureAttachment<Rock, RockRenderData>(o => new RockRenderData(o));
                    foreach (Rock t in TW.Data.GetChangedObjects<Rock>()) t.get<RockRenderData>().Update();
                }));
            engine.AddSimulator(new WorldRenderingSimulator());
        }



        [Test]
        public void TestRenderDroppedThing()
        {
            createDroppedThing();

            engine.AddSimulator(new BasicSimulator(delegate
            {
                TW.Data.EnsureAttachment<DroppedThing, DroppedThingRenderData>(o => new DroppedThingRenderData(o));
                foreach (DroppedThing t in TW.Data.GetChangedObjects<DroppedThing>()) t.get<DroppedThingRenderData>().Update();
            }));

            engine.AddSimulator(new WorldRenderingSimulator());
        }
        [Test]
        public void TestDroppedThingPhysX()
        {
            createDroppedThing();

            engine.AddSimulator(new BasicSimulator(delegate
            {
                TW.Data.EnsureAttachment<DroppedThing, DroppedThingRenderData>(o => new DroppedThingRenderData(o));
                foreach (DroppedThing t in TW.Data.GetChangedObjects<DroppedThing>()) t.get<DroppedThingRenderData>().Update();
            }));
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());

        }

        private static void createDroppedThing()
        {
            var type = new ResourceType() { Texture = TestUtilities.LoadWoodTexture() };

            new DroppedThing() { InitialPosition = new Vector3(2, 5, 2), Thing = new Thing { Type = type } };
        }
        private static void createTrees()
        {
            new Tree { Position = new Vector3(3, 0, 3), Size = 0 };
            new Tree { Position = new Vector3(6, 0, 3), Size = 5 };
            new Tree { Position = new Vector3(9, 0, 3), Size = 10 };
        }
        private static void createRocks()
        {
            new Rock { Position = new Vector3(3, 0, 8), Height = 0 };
            new Rock { Position = new Vector3(3, 0, 15), Height = 5 };
            new Rock { Position = new Vector3(3, 0, 22), Height = 10 };
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{

    [TestFixture]
    [EngineTest]
    public class PickupTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            TestUtilities.CreateGroundPlane();
        }

        [Test]
        public void TestPickupSimple()
        {
            var drop = new DroppedThing()
                {
                    Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },
                    InitialPosition = new Vector3(1, 1, 1)
                };

            var obj = new SimplePickupObject() { Position = new Vector3(1.5f, 1, 1), Holding = drop };

            engine.AddSimulator(new PickupSimulator());

            engine.AddSimulator(new RTSEntitySimulator());

            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }


        [ModelObjectChanged]
        public class SimplePickupObject : EngineModelObject, IPickupObject
        {
            public Vector3 Position { get; set; }
            public DroppedThing Holding { get; set; }
            public Actor GetHoldingActor()
            {
                try
                {
                    return Holding.get<Entity>().get<EntityPhysXUpdater.EntityPhysX>().getCurrentActor();
                }
                catch (NullReferenceException) { }
                return null;
            }
            public Vector3 GetHoldingPosition() { return Position; }
            public void DropHolding()
            {
                Holding = null;
            }
        }
    }

}

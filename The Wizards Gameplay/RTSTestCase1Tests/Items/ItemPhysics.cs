using System.Collections.Generic;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests.Items
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ItemPhysics
    {

        [Test, Sequential]
        public void TestMoveApart()
        {
            TestUtilities.SetupTWContext();

            var d1 = new DroppedThing();
            d1.Physical.WorldMatrix = Matrix.Translation(-0.1f, 0, 0);
            var d2 = new DroppedThing();
            d2.Physical.WorldMatrix = Matrix.Translation(0.1f, 0, 0);

            var p = new SimpleItemPhysicsUpdater(new SimpleWorldLocator());

            Assert.That(Vector3.Distance(d1.Physical.GetPosition(), d2.Physical.GetPosition()), Is.LessThan(0.20001f));

            p.Simulate(0.1f);

            Assert.That(Vector3.Distance(d1.Physical.GetPosition(),d2.Physical.GetPosition()), Is.GreaterThan(0.20001f));

        }

        [Test,Sequential]
        public void TestNoObjectCollision([Values(2, 10)] int count, [Values(1f, 30f)] float maxDist)
        {
            TestUtilities.SetupTWContext();

            var drops = new List<DroppedThing>();
            for (int i = 0; i < count; i++)
                drops.Add(new DroppedThing());

            var p = new SimpleItemPhysicsUpdater(new SimpleWorldLocator());

            var stabilizetime = 2; // sec
            var step = 0.01f;

            for (int i = 0; i < stabilizetime / step; i++) p.Simulate(step);

            foreach (var d in drops)
            {
                foreach (var d2 in drops)
                {
                    if (d == d2) continue;
                    Assert.That(Vector3.Distance(d.Physical.GetPosition(), d2.Physical.GetPosition()), Is.GreaterThan(0.3f));
                    Assert.That(Vector3.Distance(d.Physical.GetPosition(), d2.Physical.GetPosition()), Is.LessThan(maxDist));

                }
            }
        }



        [Test]
        public void TestGravity()
        {
            TestUtilities.SetupTWContext();

            var d = new DroppedThing();
            d.Physical.WorldMatrix = Matrix.Translation(0, 1, 0);

            var p = new SimpleItemPhysicsUpdater(new SimpleWorldLocator());

            var stabilizetime = 3; // sec
            var step = 0.01f;

            for (int i = 0; i < stabilizetime / step; i++)
            {
                p.Simulate(step);
            }

            Assert.That(d.Physical.GetPosition().Y, Is.LessThan(0.9f));
        }

    }
}
using MHGameWork.TheWizards.Simulation.ActionScheduling;
using NUnit.Framework;

namespace MHGameWork.TheWizards._Tests.Simulation
{
    [TestFixture]
    public class ActionSchedulerTest
    {
        [Test]
        public void TestSchedule()
        {
            var scheduler = new IActionScheduler();

            var value = 0;


            scheduler.SetCurrentTime(0);


            scheduler.SetTimeout(2, () => value = 5);
            scheduler.SetTimeout(3, () => value = 3);
            scheduler.SetTimeout(6, () => value++);
            scheduler.SetTimeout(6, () => value++);
            scheduler.SetTimeout(8, () =>
            {
                value = 10;
                scheduler.SetTimeout(1, () => value++);
            });


            scheduler.ExecuteActions(1);
            Assert.AreEqual(0, value);

            scheduler.ExecuteActions(1.5f);
            Assert.AreEqual(0, value);

            scheduler.ExecuteActions(2);
            Assert.AreEqual(5, value);

            scheduler.ExecuteActions(3);
            Assert.AreEqual(3, value);

            scheduler.ExecuteActions(4);
            Assert.AreEqual(3, value);

            scheduler.ExecuteActions(6);
            Assert.AreEqual(5, value);

            scheduler.ExecuteActions(6.5f);
            Assert.AreEqual(5, value);

            scheduler.ExecuteActions(8);
            Assert.AreEqual(10, value);

            scheduler.ExecuteActions(8.5f);
            Assert.AreEqual(10, value);

            scheduler.ExecuteActions(9);
            Assert.AreEqual(11, value);
        }
    }
}
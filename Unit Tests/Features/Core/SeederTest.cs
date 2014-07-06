using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core
{
    [TestFixture]
    public class SeederTest
    {
        private Seeder seeder;

        public SeederTest()
        {

        }

        [SetUp]
        public void Setup()
        {
            seeder = new Seeder(1);
        }

        [Test]
        public void TestPoissonSmall()
        {
            var count = 0;

            for (int i = 0; i < 100000; i++)
                count += seeder.PoissonSmall(30);

            Assert.AreEqual(count / 100000f, 30, 30 * 0.01f);

        }

        [Test]
        public void TestExecuteInterval()
        {
            testExecuteInterval(10000, 0.01f, 1);
            testExecuteInterval(10000, 0.1f, 1);
            testExecuteInterval(10000, 1f, 1);
            testExecuteInterval(10000, 0.1f, 10f);
            testExecuteInterval(10000, 0.1f, 0.3f);
        }

        /// <summary>
        /// TODO: provides a consistent lower result, because we only consider the event to occur 0 or 1 time in the implementation,
        /// a real poisson implementation should consider 2 or more events, which should fix the average.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="elapsed"></param>
        /// <param name="interval"></param>
        private void testExecuteInterval(float time, float elapsed, float interval)
        {
            int count = 0;
            for (int i = 0; i < time / elapsed; i++)
            {
                seeder.EachRandomInterval(interval, () => count++, elapsed);
            }

            var avg = count / time;
            Assert.AreEqual(interval, avg, interval * 0.05f);
        }
    }
}

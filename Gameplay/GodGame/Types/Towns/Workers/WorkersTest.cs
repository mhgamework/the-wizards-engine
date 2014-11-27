using System.Collections.Generic;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame.Types.Towns.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class WorkersTest
    {
        private WorkersService service;
        private ITown town1;
        private ITown town2;

        [SetUp]
        public void Setup()
        {
            town1 = new SimpleTown();
            town2 = new SimpleTown();

            service = new WorkersService();

        }


        private SimpleWorkerConsumer createConsumer(ITown town)
        {
            var ret = new SimpleWorkerConsumer();
            ((SimpleTown)town).Consumers.Add(ret);
            return ret;
        }
        private SimpleWorkerProducer createProducer(ITown town)
        {
            var ret = new SimpleWorkerProducer();
            ((SimpleTown)town).Producers.Add(ret);
            return ret;
        }


        [Test]
        public void TestNoProducers()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestNoConsumers()
        {
            createProducer(town1).ProvidedWorkersAmount = 10;
            service.UpdateWorkerDistribution(town1);
        }

        [Test]
        public void TestProduceZero()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 0;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestConsumeZero()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 0;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestProduceConsumeZero()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 0;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 0;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }

        [Test]
        public void TestSingleUnderProvision()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(5, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestSingleOverProvision()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 2;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(2, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestMultipleProducers()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 30;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            var prod2 = createProducer(town1);
            prod2.ProvidedWorkersAmount = 10;

            var prod3 = createProducer(town1);
            prod3.ProvidedWorkersAmount = 8;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(28, cons1.AllocatedWorkersCount);

        }
        [Test]
        public void TestMultipleConsumers()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 3;

            var cons2 = createConsumer(town1);
            cons2.RequestedWorkersCount = 7;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 20;

            service.UpdateWorkerDistribution(town1);

            Assert.AreEqual(3, cons1.AllocatedWorkersCount);
            Assert.AreEqual(7, cons2.AllocatedWorkersCount);

        }
        [Test]
        public void TestMultipleTowns()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 3;

            var cons2 = createConsumer(town2);
            cons2.RequestedWorkersCount = 5;

            createProducer(town1).ProvidedWorkersAmount = 2;

            createProducer(town2).ProvidedWorkersAmount = 20;

            service.UpdateWorkerDistribution(town1);
            service.UpdateWorkerDistribution(town2);

            Assert.AreEqual(2, cons1.AllocatedWorkersCount);
            Assert.AreEqual(5, cons2.AllocatedWorkersCount);
        }

    }

    public class SimpleTown : ITown
    {
        public List<IWorkerProducer> Producers = new List<IWorkerProducer>();
        public List<IWorkerConsumer> Consumers = new List<IWorkerConsumer>();

        IEnumerable<IWorkerProducer> ITown.Producers
        {
            get { return Producers; }
        }

        IEnumerable<IWorkerConsumer> ITown.Consumers
        {
            get { return Consumers; }
        }


    }
    public class SimpleWorkerProducer : IWorkerProducer
    {
        public int ProvidedWorkersAmount { get; set; }
    }
    public class SimpleWorkerConsumer : IWorkerConsumer
    {
        public int RequestedWorkersCount { get; set; }
        public int AllocatedWorkersCount { get; set; }
    }
}
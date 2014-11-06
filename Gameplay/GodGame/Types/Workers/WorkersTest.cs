using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using NSubstitute;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame.Types.Workers
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
            town1 = new Town();
            town2 = new Town();

            service = new WorkersService(new ITown[] { town1, town2 });

        }


        private WorkerConsumer createConsumer(ITown town)
        {
            var ret = new WorkerConsumer();
            ((Town)town).Consumers.Add(ret);
            return ret;
        }
        private WorkerProducer createProducer(ITown town)
        {
            var ret = new WorkerProducer();
            ((Town)town).Producers.Add(ret);
            return ret;
        }


        [Test]
        public void TestNoProducers()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            service.UpdateWorkerDistribution();

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestNoConsumers()
        {
            createProducer(town1).ProvidedWorkersAmount = 10;
            service.UpdateWorkerDistribution();
        }

        [Test]
        public void TestProduceZero()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 0;

            service.UpdateWorkerDistribution();

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestConsumeZero()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 0;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution();

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestProduceConsumeZero()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 0;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 0;

            service.UpdateWorkerDistribution();

            Assert.AreEqual(0, cons1.AllocatedWorkersCount);
        }

        [Test]
        public void TestSingleUnderProvision()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution();

            Assert.AreEqual(5, cons1.AllocatedWorkersCount);
        }
        [Test]
        public void TestSingleOverProvision()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 2;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution();

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

            service.UpdateWorkerDistribution();

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

            service.UpdateWorkerDistribution();

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

            service.UpdateWorkerDistribution();

            Assert.AreEqual(2, cons1.AllocatedWorkersCount);
            Assert.AreEqual(5, cons2.AllocatedWorkersCount);
        }

    }

    public class Town : ITown
    {
        public List<WorkerProducer> Producers = new List<WorkerProducer>();
        public List<WorkerConsumer> Consumers = new List<WorkerConsumer>();

        IEnumerable<WorkerProducer> ITown.Producers
        {
            get { return Producers; }
        }

        IEnumerable<WorkerConsumer> ITown.Consumers
        {
            get { return Consumers; }
        }


    }
}
using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class WorkersTest
    {
        private WorkersService service;
        private Town town1;
        private Town town2;

        [SetUp]
        public void Setup()
        {
            town1 = Substitute.For<Town>();
            town2 = Substitute.For<Town>();

            service = new WorkersService(new Town[] { town1, town2 });

        }


        private WorkerConsumer createConsumer(Town town)
        {
            throw new System.NotImplementedException();
        }
        private WorkerProducer createProducer(Town town)
        {
            throw new System.NotImplementedException();
        }


        [Test]
        public void TestSingleOverProvision()
        {
            var cons1 = createConsumer(town1);
            cons1.RequestedWorkersCount = 5;

            var prod1 = createProducer(town1);
            prod1.ProvidedWorkersAmount = 10;

            service.UpdateWorkerDistribution();

            Assert.AreEqual(5, cons1.AllocatedWorkersCount);
        }

        [Test]
        public void TestSingleUnderProvision()
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

            Assert.AreEqual(2, cons1.AllocatedWorkersCount);

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

    public interface Town
    {
        IEnumerable<WorkerProducer> Producers { get; }
        IEnumerable<WorkerConsumer> Consumers { get; }
    }

    public class WorkersService
    {
        private readonly IEnumerable<Town> allTowns;

        public WorkersService(IEnumerable<Town> allTowns)
        {
            this.allTowns = allTowns;
        }

        public WorkerConsumer CreateWorkerConsumer()
        {
            throw new System.NotImplementedException();
        }

        public WorkerProducer CreateWorkerProducer()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateWorkerDistribution()
        {
            foreach (var town in allTowns.ToArray())
            {
                UpdateWorkerDistribution(town);
            }
        }

        private void UpdateWorkerDistribution(Town town)
        {
            var producers = town.Producers.ToArray();
            var consumer = town.Consumers.ToArray();

            var totalWorkers = producers.Select(p => p.ProvidedWorkersAmount).Sum();

            var totalRequired = consumer.Select(c => c.RequestedWorkersCount).Sum();

            var availability = (float)totalRequired / totalWorkers;

            if (availability > 1) availability = 1;


            foreach (var c in consumer)
            {
                c.AllocatedWorkersCount = (int)Math.Floor(c.RequestedWorkersCount * availability);
            }

            var assigned = consumer.Select(c => c.AllocatedWorkersCount).Sum();
            var unassigned = totalRequired - assigned;
            if (unassigned < 0 || unassigned > consumer.Length) throw new InvalidOperationException("Algorithm error!");
            foreach (var c in consumer)
            {
                if (c.AllocatedWorkersCount > c.RequestedWorkersCount)
                    continue;
                if (unassigned == 0) break;
                c.AllocatedWorkersCount++;
                unassigned--;
            }

            if (totalWorkers != consumer.Select(c => c.AllocatedWorkersCount).Sum()) throw new InvalidOperationException("Algorithm error!");

        }
    }



    public class WorkerProducer
    {
        public int ProvidedWorkersAmount { get; set; }
    }

    public class WorkerConsumer
    {
        public int RequestedWorkersCount { get; set; }

        public int AllocatedWorkersCount { get; set; } //TODO: make set private
    }

}
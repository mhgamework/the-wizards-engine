using System;
using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types.Workers
{
    public class WorkersService
    {
        private readonly IEnumerable<ITown> allTowns;

        public WorkersService(IEnumerable<ITown> allTowns)
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

        private void UpdateWorkerDistribution(ITown town)
        {
            var producers = town.Producers.ToArray();
            var consumer = town.Consumers.ToArray();

            var totalWorkers = producers.Select(p => p.ProvidedWorkersAmount).Sum();

            var totalRequired = consumer.Select(c => c.RequestedWorkersCount).Sum();

            var availability = (float)totalWorkers / totalRequired;

            if (totalRequired == 0) availability = 0;
            if (availability > 1) availability = 1;

            foreach (var c in consumer)
            {
                c.AllocatedWorkersCount = (int)Math.Floor(c.RequestedWorkersCount * availability);
            }

            if (totalWorkers >= totalRequired) return;

            var assigned = consumer.Select(c => c.AllocatedWorkersCount).Sum();
            var unassigned = totalWorkers - assigned;
            if (unassigned == 0) return;
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
}
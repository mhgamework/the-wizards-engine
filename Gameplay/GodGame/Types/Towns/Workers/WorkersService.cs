﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types.Towns.Workers
{
    public class WorkersService
    {

        public WorkersService()
        {
        }

        public void UpdateWorkerDistribution(ITown town)
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
                c.AllocateWorkers((int)Math.Floor(c.RequestedWorkersCount * availability));
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
                c.AllocateWorkers(c.AllocatedWorkersCount + 1);
                unassigned--;
            }

            if (totalWorkers != consumer.Select(c => c.AllocatedWorkersCount).Sum()) throw new InvalidOperationException("Algorithm error!");

        }
    }
}
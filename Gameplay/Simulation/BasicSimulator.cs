﻿using System;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Simulation
{
    public class BasicSimulator : ISimulator
    {
        private readonly Action deleg;

        public BasicSimulator(Action deleg)
        {
            this.deleg = deleg;
        }

        public void Simulate()
        {
            deleg();
        }

    }
}
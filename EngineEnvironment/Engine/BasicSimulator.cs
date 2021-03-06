﻿using System;

namespace MHGameWork.TheWizards.Engine
{
    public class BasicSimulator : ISimulator
    {
        private readonly Action deleg;

        public BasicSimulator()
        {
            deleg = delegate {};
        }

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

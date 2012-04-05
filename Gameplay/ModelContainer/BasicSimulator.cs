using System;

namespace MHGameWork.TheWizards.ModelContainer
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

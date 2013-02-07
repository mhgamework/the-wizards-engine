using System;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Note that when this class is hotloaded, the delegate is removed!!!
    /// </summary>
    [Obsolete]
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

using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    /// <summary>
    /// Responsible for creating construction actions. This is part of the configuration layer of the application. (considering IoC)
    /// Sinds the dependencies of this object are only used when constructing objects at run-time they are made lazy.
    /// </summary>
    public class ConstructionFactory
    {
        private Dictionary<Type, Func<Island, IConstructionAction>> bindings = new Dictionary<Type, Func<Island,IConstructionAction>>();

        public ConstructionFactory(Lazy<DistributionHelper> distributionHelper, Lazy<RoundSimulator> roundSimulator)
        {

            Add(i => new NullConstructionAction());
            Add(i => new CrystalCliffsAction(i, distributionHelper.Value, roundSimulator.Value));
            Add(i => new EnergyNodeAction(i, distributionHelper.Value, roundSimulator.Value));
            Add(i => new ScrapStationAction(i, distributionHelper.Value));
            Add(i => new WorkshopAction(i, distributionHelper.Value));

        }


        public T CreateConstructionAction<T>(Island arg) where T : IConstructionAction
        {
            return (T)bindings[typeof(T)](arg);
        }

        private void Add<T>(Func<Island, T> createNew) where T : IConstructionAction
        {
            bindings.Add(typeof(T), i => createNew(i));
        }
    }
}
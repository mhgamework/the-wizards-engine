using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldSimulation.Actions;
using MHGameWork.TheWizards.WorldSimulation.Priorities;

namespace MHGameWork.TheWizards.WorldSimulation
{
    public class GoblinBehavior:ICreatureBehavior,IBellyFillable
    {
        private readonly Creature creature;
        private readonly BuildingBluePrints blueprint;

        private List<IPriority> priorities = new List<IPriority>();
        private List<PriorityItem> items = new List<PriorityItem>();
        private PriorityItem hungerP =new PriorityItem();
        private PriorityItem exploreP= new PriorityItem();
        private PriorityItem deathP = new PriorityItem();
        private PriorityItem housingP = new PriorityItem();
        private PriorityItem ReProductionP = new PriorityItem();
        public GoblinBehavior(Creature creature,BuildingBluePrints blueprint)
        {
            this.creature = creature;
            this.blueprint = blueprint;
            creature.SetProperty(Explore.TimeExploredProperty, 0);
            creature.SetProperty(ReProduction.TimeSinceLastFornication, 0);
            FoodLevel = 80;
            hungerP.Priority = new FillBelly(this);
            items.Add(hungerP);
            exploreP.Priority = new Explore();
            items.Add(exploreP);
            deathP.Priority = new Death();
            items.Add(deathP);
            housingP.Priority = new Housing(blueprint);
            items.Add(housingP);
            ReProductionP.Priority = new ReProduction();
            items.Add(ReProductionP);
        }
      
        
    
        private IPriority hightestPriority;


        public IPriority GetPriority(int i)
        {
            return items[i].Priority;
        }

        public List<PriorityItem> Priorities
        {
            get { return items; }
        }

        public int PriorityCount()
        {
            return items.Count;
        }

        public IPriority GetHighestPriority
        {
            get { return hightestPriority; }
        }

       
        

       
        public void UpdatePriorities(float elapsed)
        {
            //update priority variables
            FoodLevel -= elapsed;
            creature.SetProperty(Explore.TimeExploredProperty, creature.GetProperty(Explore.TimeExploredProperty) - elapsed);
            creature.SetProperty(ReProduction.TimeSinceLastFornication, creature.GetProperty(ReProduction.TimeSinceLastFornication) + elapsed);
            // update priorities
            //FillBelly
            hungerP.Level = 100-FoodLevel;
            if (FoodLevel > 50)
                hungerP.Level = 100 - 50 - (FoodLevel - 50)/5;
            //Explore
            exploreP.Level = 50 - creature.GetProperty(Explore.TimeExploredProperty) *0.1f;
            if (FoodLevel < 0)
                deathP.Level = 5000; 
            //Housing
            housingP.Level = 35 + FoodLevel-creature.Buildings.Count*40;

            //ReProduction
            if (creature.GetProperty(ReProduction.TimeSinceLastFornication) < 5)//note: 20 pregnacy time sort of
            {
                ReProductionP.Level = 0;
            }
            else
            {
                ReProductionP.Level = -55 + FoodLevel + creature.Buildings.Count*40 +creature.GetProperty(ReProduction.TimeSinceLastFornication)*0.5f;
            }
            //hightestPriority = items[0].Priority;
            //creature.CurrentPriority = items[0].Priority;
        }

        public ICreatureBehavior GetNewBehavior(Creature creature)
        {
           return  new GoblinBehavior(creature, blueprint);// note: not sure about this cheat
        }


        public float FoodLevel { get; set; }


    }
}

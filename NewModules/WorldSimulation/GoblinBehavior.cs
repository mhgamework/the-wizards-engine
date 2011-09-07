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
        private List<IPriority> priorities = new List<IPriority>();
        private List<PriorityItem> items = new List<PriorityItem>();
        private PriorityItem hungerP =new PriorityItem();
        private PriorityItem exploreP= new PriorityItem();
        private PriorityItem deathP = new PriorityItem();
        private PriorityItem housingP = new PriorityItem();
        public GoblinBehavior(Creature creature,BuildingBluePrints blueprint)
        {
            this.creature = creature;
            creature.SetProperty(Explore.TimeExploredProperty, 0);
            FoodLevel = 80;
            hungerP.Priority = new FillBelly(this);
            items.Add(hungerP);
            exploreP.Priority = new Explore();
            items.Add(exploreP);
            deathP.Priority = new Death();
            items.Add(deathP);
            housingP.Priority = new Housing(blueprint);
            items.Add(housingP);
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
            housingP.Level = 35 + FoodLevel;
            //hightestPriority = items[0].Priority;
            //creature.CurrentPriority = items[0].Priority;
        }
       

        public float FoodLevel { get; set; }
    }
}

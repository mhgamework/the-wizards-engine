﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldSimulation
{
    /// <summary>
    /// Responsible for making the creatures do the right actions
    /// </summary>
    public class Simulater
    {
        private List<Creature> creatures = new List<Creature>();
        private readonly List<Resource> resources;
        private readonly List<Building> buildings;
        private int persistance = 12;
        private List<Creature> newCreatures = new List<Creature>();

        public Simulater(List<Creature> creatures, List<Resource> resources)
        {
            this.creatures = creatures;
            this.resources = resources;
            this.buildings = new List<Building>();
        }
        public Simulater(List<Creature> creatures, List<Resource> resources, List<Building> buildings)
        {
            this.creatures = creatures;
            this.resources = resources;
            this.buildings = buildings;
        }

        public List<Resource> Resources
        {
            get { return resources; }
        }

        private void UpdatePriorities(float elapsed)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                var creature = creatures[i];
                if (creature.CurrentPriority != null)
                    creature.CurrentPriority.Apply(elapsed, creature, this);

                creature.Behaviour.UpdatePriorities(elapsed);

                var currentItem = creature.Behaviour.Priorities.Find(o => o.Priority == creature.CurrentPriority);
                if (currentItem != null)
                {

                    currentItem.Level += persistance;
                }

                creature.Behaviour.Priorities.Sort((a, b) => (int)(b.Level * 100 - a.Level * 100));// mind accuracy

                //var highest = creature.Behaviour.Priorities[0];

                //if (highest.Level - currentItem.Level < 10)
                //{
                //    creature.CurrentAction = currentItem.Priority.GetNextAction(creature, this);
                //    if (creature.CurrentAction != null)
                //    break;
                //}

                for (int j = 0; j < creature.Behaviour.PriorityCount(); j++)
                {
                    var priority = creature.Behaviour.Priorities[j];
                    IAction act=priority.Priority.GetNextAction(creature, this);
                if (act != null)
                {
                    if(creature.CurrentAction!=null)
                    if (creature.CurrentAction.GetType() != act.GetType())
                        creature.CurrentAction.ForcedEnd();
                    creature.CurrentAction = act;
                    creature.CurrentPriority = priority.Priority;
                    break;}
                    
                    
                }

               
            }
        }
        private void UpdateActions(float elapsed)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].CurrentAction.Apply(elapsed, creatures[i]);
                if (creatures[i].CurrentAction.Fullfilled())
                {
                    creatures[i].CurrentAction.End(); creatures[i].CurrentAction = null;
                }
            }
        }
        public void updateResources(float elapsed)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                Resources[i].IncrementResource(elapsed);
            }
        }
        public void Update(float elapsed)// note temp fast updating
        {
            if (elapsed > 1 / 30f) elapsed = 1 / 30f;
            UpdatePriorities(elapsed);
            UpdateActions(elapsed);
            updateResources(elapsed);
            RemoveDeath();
            AddNewBorn();
        }

        private void RemoveDeath()
        {
            int k = creatures.Count;
            int i = 0;
            while (i < k)
            {
                if (!creatures[i].Alive)
                {
                    creatures.Remove(creatures[i]);
                    k--;
                }
                else
                {
                    i++;
                }
            }
        }
        private void AddNewBorn()
        {
            for (int i = 0; i < newCreatures.Count; i++)
            {
                creatures.Add(newCreatures[i]);
            }
            newCreatures = new List<Creature>();//note not sure if the other get's deleted
        }
        public void AddBuilding(Building building)
        {
            Buildings.Add(building);
        }


        public List<Building> Buildings
        {
            get { return buildings; }
        }


        public void AddCreature(Creature baby)
        {
            newCreatures.Add(baby);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.WorldSimulation.Priorities;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
   public  class Construct:IAction
    {
       private readonly Creature creature;
       private readonly BuildingBluePrints buildingBluePrints;
       private readonly Housing house;
       private readonly Simulater sim;

       private float totalElapsed;
       private List<float> startValues = new List<float>();
       bool resourceCheck = true;
       public Construct(Creature creature, BuildingBluePrints buildingBluePrints, Housing house, Simulater sim)
       {
           this.creature = creature;
           this.buildingBluePrints = buildingBluePrints;
           this.house = house;
           this.sim = sim;
           if (!checkNeededResources(creature, buildingBluePrints))
           {       resourceCheck = false;
           return;
       }

    for (int i = 0; i < buildingBluePrints.RequirementTypes.Count; i++)
           {
               startValues.Add(creature.GetResource(buildingBluePrints.RequirementTypes[i]));
           }
       }

       public void Apply(float elapsed, Creature creature)
       {
           totalElapsed += elapsed;
           totalElapsed = MathHelper.Clamp(totalElapsed, 0, totalElapsed);
           totalElapsed = MathHelper.Clamp(totalElapsed, 0, buildingBluePrints.ConstructionTime);
           for (int i = 0; i < buildingBluePrints.RequirementTypes.Count; i++)
           {
               creature.SetResource(buildingBluePrints.RequirementTypes[i],MathHelper.Lerp(startValues[i],startValues[i]-buildingBluePrints.Requirements[buildingBluePrints.RequirementTypes[i]],totalElapsed/buildingBluePrints.ConstructionTime));
           }

       }

       private bool checkNeededResources(Creature creature, BuildingBluePrints buildingBluePrints)
       {
           
           for (int i = 0; i < buildingBluePrints.RequirementTypes.Count; i++)
           {
               if (creature.GetResource(buildingBluePrints.RequirementTypes[i]) < buildingBluePrints.Requirements[buildingBluePrints.RequirementTypes[i]])
                   return false;
           }
           return true;
       }

       public void End()
       {
           var building = new Building(house.HousingLocation, creature, buildingBluePrints);
           creature.Buildings.Add(building);
           sim.AddBuilding(building);
           house.HousingLocation = new Vector3(12.3f, 45.6f, 78.9f);
       }

      public bool Fullfilled()
       {
           if (totalElapsed >= buildingBluePrints.ConstructionTime)
               return true;
           return false;
       }

       public void ForcedEnd()
       {
           //throw new NotImplementedException();
       }

       public bool isValid()
       {
           return resourceCheck;
       }
    }
}

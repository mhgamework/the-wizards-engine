using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldSimulation.Actions;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation.Priorities
{
    public class Housing:IPriority//TODO: when resources are empty at this location move to the next 
    {
        public Vector3 HousingLocation= new Vector3(12.3f,45.6f,78.9f);//note defaultvalue
        private readonly BuildingBluePrints buildingBluePrints;

        public Housing(BuildingBluePrints buildingBluePrints)
        {
            this.buildingBluePrints = buildingBluePrints;
            
        }

        public void Apply(float elapsed, Creature creature, Simulater simulater)
        {
            
        }

        public IAction GetNextAction(Creature creature, Simulater simulater)
        {
            if ((creature.CurrentAction is MoveToResource || creature.CurrentAction is Construct || creature.CurrentAction is MoveToPosition||creature.CurrentAction is FindHouseLocation) && creature.CurrentPriority == this)
                return creature.CurrentAction;
            if (HousingLocation == new Vector3(12.3f, 45.6f, 78.9f))
                HousingLocation =creature.Seeder.NextVector3(new Vector3(-200, 0, -200).xna(), new Vector3(200, 0, 200).xna()).dx();
            for (int i = 0; i < buildingBluePrints.RequirementTypes.Count; i++)
            {
                var requirementType = buildingBluePrints.RequirementTypes[i];
                if (creature.GetResource(requirementType) < buildingBluePrints.Requirements[requirementType])
                    // needs to collect this resource
                {
                    Resource res = Resource.InResourceVicinity(creature.Position, simulater.Resources, requirementType);
                    if (res != null)
                        return new GatherResource(creature, buildingBluePrints.Requirements[requirementType], 5f,null, res, requirementType);
                    var act =new MoveToResource(requirementType, creature, simulater.Resources, 10,10f);
                    if (!act.isValid())
                        return null;//note velocity used
                    return act;
                }
                
            }
            if ((creature.Position - HousingLocation).Length() > 3f)//note distance to house location here
                return new MoveToPosition(creature, HousingLocation, 10f);
            var mtrAct = new Construct(creature,buildingBluePrints,this,simulater);
            if (!mtrAct.isValid())
                return null;//note velocity used
            return mtrAct;

          
        }
          private void setHousingLocation(Vector3 vec)
            {
                HousingLocation= vec;
            }
    }
}

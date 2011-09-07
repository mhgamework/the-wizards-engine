using System.Collections.Generic;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
    public class BuildingBluePrints
    {
        public List<ResourceTypes> RequirementTypes = new List<ResourceTypes>();
        public Dictionary<ResourceTypes, float> Requirements = new Dictionary<ResourceTypes, float>();
        public float Size { get; set; }
        public float ConstructionTime;
    }
}
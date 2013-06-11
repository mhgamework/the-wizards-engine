using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Building
{
    [ModelObjectChanged]
    public class BuildablePart : EngineModelObject, IObjectPart
    {
        public IBuildable Parent { get; set; }

        public float BuildProgress { get; set; }

        public List<ResourceType> RequiredResources { get; set; }

        public List<ResourceType> ResourcesRemaining { get; set; }

        public BuildablePart()
        {
            RequiredResources = new List<ResourceType>();
            ResourcesRemaining = new List<ResourceType>();
        }


        public void ResetBuild()
        {
            BuildProgress = 0;
            ResourcesRemaining.Clear();
            ResourcesRemaining.AddRange(RequiredResources);
        }

        public bool StillNeedsResource(ResourceType type)
        {
            return ResourcesRemaining.Contains(type);
        }

        public void ProvideResource(ResourceType type)
        {
            if (!ResourcesRemaining.Contains(type)) throw new InvalidOperationException("This resource is not required anymore!");

            ResourcesRemaining.Remove(type);

            BuildProgress = 1 - ResourcesRemaining.Count / (float)RequiredResources.Count;
        }
    }
}
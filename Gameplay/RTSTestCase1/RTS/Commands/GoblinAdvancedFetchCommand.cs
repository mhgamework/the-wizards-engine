using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class GoblinAdvancedFetchCommand :EngineModelObject,IGoblinCommand
    {
        public void Update(Goblin goblin)
        {
            updater.Update(goblin, SourcePosition, 81, TargetPosition, ResourceType);

        }

        public string Description { get { return "Fetch from A to B"; }  }
        public Vector3 TargetPosition { get; set; }
        public Vector3 SourcePosition { get; set; }
        public ResourceType ResourceType { get; set; }
        private GoblinFetchUpdater updater = new GoblinFetchUpdater();
        
    }
}
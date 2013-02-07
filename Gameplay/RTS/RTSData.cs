using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS.Commands;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class RTSData:EngineModelObject
    {
        public RTSData()
        {
            RocketResourceType = new ResourceType();
        }

        public ResourceType RocketResourceType { get; set; }
        
    }
}
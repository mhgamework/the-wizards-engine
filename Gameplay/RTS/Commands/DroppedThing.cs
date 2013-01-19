using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class DroppedThing : EngineModelObject
    {
        public Vector3 Position { get; set; }
        public Thing Thing { get; set; }
    }
}
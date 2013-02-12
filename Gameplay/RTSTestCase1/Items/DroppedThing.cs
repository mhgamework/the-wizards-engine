using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class DroppedThing : EngineModelObject
    {
        /// <summary>
        /// The current position is stored in Entity, due to a design problem.
        /// </summary>
        public Vector3 InitialPosition { get; set; }
        public Thing Thing { get; set; }
    }
}
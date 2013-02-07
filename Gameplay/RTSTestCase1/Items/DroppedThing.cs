using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    [ModelObjectChanged]
    public class DroppedThing : EngineModelObject
    {
        /// <summary>
        /// The current position is stored in Entity, due to a design problem.
        /// </summary>
        public Vector3 InitialPosition { get; set; }
        public Thing Thing { get; set; }
        public Vector3 GetRealTimePosition()
        {
            return get<Entity>().WorldMatrix.xna().Translation.dx();
        }
    }
}
using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public class CommandHolderPart : EngineModelObject
    {
        public CommandHolderPart()
        {
            AssignedCommands = new List<GoblinCommandOrb>();
            HoldingArea = new HoldingAreaDescription { Direction = new Vector3(1, 0, 0) };
        }

        public List<GoblinCommandOrb> AssignedCommands { get; set; }

        public HoldingAreaDescription HoldingArea { get; set; }



       
    }
    public struct HoldingAreaDescription
    {
        public Vector3 RelativeStart;
        public Vector3 Direction;
    }
    public interface ICommandHolder :IModelObject, IPhysical
    {
        CommandHolderPart CommandHolder { get; set; }
    }
}
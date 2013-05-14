using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins.Components
{
    [ModelObjectChanged]
    public class CommandHolderPart : EngineModelObject, IObjectPart
    {
        /// <summary>
        /// 
        /// TODO: autoassign this!
        /// </summary>
        public ICommandHolder Holder { get; set; }

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
}
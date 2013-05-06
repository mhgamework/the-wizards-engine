﻿using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// A cart!!
    /// </summary>
    public class Cart: EngineModelObject, IPhysical, ICommandHolder
    {
        public Cart()
        {
            Physical = new Physical();
            CommandHolder = new CommandHolderPart();

            CommandHolder.HoldingArea = new HoldingAreaDescription()
            {
                RelativeStart =  new Vector3(0,2,0),
                Direction = new Vector3(1,0,0)
            };
        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            Physical.Mesh = TW.Assets.LoadMesh("RTS\\Cannon");
        }

        public CommandHolderPart CommandHolder { get; set; }
    }
}
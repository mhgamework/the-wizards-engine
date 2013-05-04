using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinMovementSimulator : ISimulator
    {
        public void Simulate()
        {
            IModelObject[] goblins = TW.Data.Objects.Where(gob => gob is Goblin).ToArray();

            foreach (Goblin goblin in goblins.Where(gob => gob is Goblin))
            {
                if (((Goblin)goblin).get<GoblinMover>() == null)
                    ((Goblin)goblin).set(new GoblinMover(((Goblin)goblin)));

                ((Goblin)goblin).get<GoblinMover>().Update();
            }

        }
    }
}

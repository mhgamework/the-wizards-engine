using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinMovementSimulatorSimple : ISimulator
    {
        public void Simulate()
        {
            IModelObject[] goblins = TW.Data.Objects.Where(gob => gob is Goblin).ToArray();

            foreach (Goblin goblin in goblins.Where(gob => gob is Goblin))
            {
                Vector3 toGoal = (goblin.Goal - goblin.Position);
                toGoal.Y = 0;
                //goblin.Position = goblin.Goal;
                var distance = TW.Graphics.Elapsed * 3;
                if (distance < toGoal.Length())
                {
                    toGoal.Normalize();
                    goblin.Position += toGoal * distance;

                }
                else
                {
                    goblin.Position = goblin.Goal;
                }
                goblin.Position = new Vector3(goblin.Position.X, 0, goblin.Position.Z);

            }
        }
    }
}

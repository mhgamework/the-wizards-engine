using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTS.Movement
{
    public class GoblinMovementSimulator :ISimulator
    {
        public void Simulate()
        {
            IModelObject[] goblins = TW.Data.Objects.Where(gob => gob is Goblin).ToArray();
                
            foreach (var goblin in goblins.Where(gob => gob is Goblin))
            {
                if (((Goblin) goblin).get<GoblinMover>() == null)
                    ((Goblin)goblin).set(new GoblinMover(((Goblin)goblin)));
            
                ((Goblin)goblin).get<GoblinMover>().Update();
            }

        }
    }
}

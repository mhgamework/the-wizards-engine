using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.RTS.Movement;

namespace MHGameWork.TheWizards.RTS.Fighting
{
    class GoblinFigthingSimulator
    {
        public void Simulate()
        {
            IModelObject[] goblins = TW.Data.Objects.Where(gob => gob is Goblin).ToArray();

            foreach (var goblin in goblins.Where(gob => gob is Goblin))
            {
                if (((Goblin)goblin).get<GoblinFighter>() == null)
                    ((Goblin)goblin).set(new GoblinFighter((Goblin)goblin));
                ((Goblin)goblin).get<GoblinFighter>().Update();
            }
        }

    }
}

using MHGameWork.TheWizards.RTSTestCase1.Goblins.Spawning;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public class GoblinCreator:IGoblinCreator
    {
        public IGoblin CreateGoblin(Vector3 position)
        {
            var gobin = new Goblin();
            gobin.Physical.WorldMatrix = Matrix.Translation(position);
            return gobin;
        }
    }
}
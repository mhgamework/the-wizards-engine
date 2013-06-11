using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins.Spawning
{
    [ModelObjectChanged]
    public class GoblinSpawnPoint:EngineModelObject,IPhysical,IGoblinSpawnPoint
    {
        public Vector3 Position { get; set; }
        public Physical Physical { get; set; }
        public float SpawnTime { get; set; }
        public GoblinSpawnPoint()
        {
            Physical = new Physical();
        }

        public void UpdatePhysical()
        {
            
            Physical.Mesh = UtilityMeshes.CreateMeshWithText(1f, "JASPERS BOX OF GOBLINS", TW.Graphics);
            Physical.WorldMatrix = Matrix.Translation(Position);
            
        }
    }
}
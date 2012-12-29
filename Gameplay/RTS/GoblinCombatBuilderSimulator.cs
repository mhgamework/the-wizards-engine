using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinCombatBuilderSimulator : ISimulator
    {
        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D1))
                placeSpawner(false);
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D2))
                placeSpawner(true);
        }

        private static void placeSpawner(bool isEnemy)
        {
            VoxelBlock last;
            TW.Data.GetSingleton<VoxelTerrain>()
              .Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), out last);

            if (last != null)
            {
                var p = new EvilGoblinSpawner();
                p.Position = last.RelativePosition + last.TerrainChunk.WorldPosition + MathHelper.One*0.5f;
                TW.Data.GetSingleton<Datastore>().Persist(p);

                p.Enemy = isEnemy;
            }
        }
    }
}
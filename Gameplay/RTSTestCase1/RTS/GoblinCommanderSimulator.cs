using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.WorldRendering;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX.DirectInput;
using MHGameWork.TheWizards;
using System.Drawing;

namespace MHGameWork.TheWizards.RTS
{
    /// <summary>
    /// Press T to select goblin, Y to move goblin to a position.
    /// 
    /// </summary>
    public class GoblinCommanderSimulator : ISimulator
    {
        private Goblin currentSelectedGoblin;
        public void Simulate()
        {
            var terrain = TW.Data.GetSingleton<VoxelTerrain>();
            var world = TW.Data.GetSingleton<Engine.WorldRendering.World>();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.T))
            {
                selectGoblin(world);
            }
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.O))
            {
                moveGoblin(terrain);
            }
         if (TW.Graphics.Keyboard.IsKeyPressed(Key.U))
         {
             if (currentSelectedGoblin == null)
                 return;
             VoxelBlock last;
             terrain.Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), out last);
             if (last == null)
                 return;
             var start = terrain.GetPositionOf(last);
             currentSelectedGoblin.Position = start;

         }
        }

        private void moveGoblin(VoxelTerrain terrain)
        {
            VoxelBlock last;
            terrain.Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), out last);
            if (last == null)
                return;
            var start = terrain.GetPositionOf(last);
            if (currentSelectedGoblin == null)
                return;
            if (currentSelectedGoblin.get<GoblinMover>() == null)
                currentSelectedGoblin.set(new GoblinMover(currentSelectedGoblin));
            var goblinMover = currentSelectedGoblin.get<GoblinMover>();
            goblinMover.MoveTo(start);
        }

        private void selectGoblin(Engine.WorldRendering.World world)
        {
            var ray = TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay();
            var result = world.Raycast(ray);
            if (!result.IsHit)
                return;
            if (!(result.Object is Engine.WorldRendering.Entity))
                return;
            var mesh = ((Engine.WorldRendering.Entity)result.Object).Mesh;
              foreach (var goblin in TW.Data.Objects.Where(o=> o is Goblin).Where(goblin => ((Goblin) goblin).get<Engine.WorldRendering.Entity>().Mesh == mesh))
              {
                  currentSelectedGoblin = (Goblin) goblin;
                  return;
              }
        }
        
        
        
        }
    }
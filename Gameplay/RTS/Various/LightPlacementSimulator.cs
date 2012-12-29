using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS.Various
{
    public class LightPlacementSimulator : ISimulator
    {
        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.L))
            {
                VoxelBlock last;
                TW.Data.GetSingleton<VoxelTerrain>()
                  .Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), out last);

                if (last != null)
                {
                    var p = new PointLight();
                    p.Position = last.Position + MathHelper.One * 0.5f;

                    var ent = new Engine.WorldRendering.Entity();
                    ent.WorldMatrix = Matrix.Translation(p.Position);

                    var builder = new MeshBuilder();
                    builder.AddSphere(12,1);
                    ent.Mesh = builder.CreateMesh();
                }
            }
        }
    }
}

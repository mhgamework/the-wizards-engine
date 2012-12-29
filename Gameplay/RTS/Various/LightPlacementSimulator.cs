using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Persistence;
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
                    p.Position = last.RelativePosition + last.TerrainChunk.WorldPosition + MathHelper.One * 0.5f;

                    updateEntity(p);

                    TW.Data.GetSingleton<Datastore>().Persist(p);
                }
            }



            foreach (var light in TW.Data.GetChangedObjects<PointLight>())
                updateEntity(light);
        }

        private static void updateEntity(PointLight light)
        {
            if (light.get<Engine.WorldRendering.Entity>() == null)
                light.set(new Engine.WorldRendering.Entity());

            var ent = light.get<Engine.WorldRendering.Entity>();

            ent.WorldMatrix = Matrix.Scaling(0.05f, 0.05f, 0.05f) * Matrix.Translation(light.Position);
            ent.CastsShadows = false;

            var builder = new MeshBuilder();
            builder.AddSphere(12, 1);
            ent.Mesh = builder.CreateMesh();

        }
    }
}

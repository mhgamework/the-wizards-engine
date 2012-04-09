using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Voxelization;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This class contains functions for debugging graphical stuff.
    /// TODO: this class contains references to all other modules, and is somewhat not correctly placed in this module/ in the NewModules project
    /// TODO: add test?
    /// </summary>
    public class DebugVisualizer
    {

        public static void ShowVoxelGrid(VoxelGrid grid)
        {
            var game = new DX11Game();
            game.InitDirectX();
            var renderer = new DeferredRenderer(game);

            var light = renderer.CreateDirectionalLight();

            var ret = grid;

            var builder = new MeshBuilder();

            for (int x = ret.Min.X; x <= ret.Max.X; x++)
                for (int y = ret.Min.Y; y <= ret.Max.Y; y++)
                    for (int z = ret.Min.Z; z <= ret.Max.Z; z++)
                    {
                        if (!ret[x, y, z]) continue;


                        builder.AddBox(new Vector3(x, y, z), (new Vector3(x, y, z) + MathHelper.One));
                    }


            var voxelMesh = builder.CreateMesh();

            var voxel = renderer.CreateMeshElement(voxelMesh);
            voxel.WorldMatrix = Matrix.Translation(0, 0, 0);

            game.GameLoopEvent += delegate
            {
                renderer.Draw();
            };
            game.Run();
        }
    }
}

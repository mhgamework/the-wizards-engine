using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Core;
using MHGameWork.TheWizards.VoxelTerraining;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class NavigationTest
    {
        private TWEngine engine;

        [SetUp]
        private void setup()
        {
            engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

        }
        [Test]
        public void TestAstarTerrain()
        {
            setup();

            var terrain = TW.Data.GetSingleton<VoxelTerrain>();


            var start = new Vector3(1, 40, 1);
            var end = new Vector3(5, 40, 5);
            List<VoxelBlock> path = null;

            VoxelTerrainTest.generateTerrain(1, 1);
            engine.AddSimulator(new BasicSimulator(delegate
                {
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.T))
                    {
                        VoxelBlock last;
                        terrain.Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), out last);

                        start = terrain.GetPositionOf(last);
                    }
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.Y))
                    {
                        VoxelBlock last;
                        terrain.Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(),out last);
                        end = terrain.GetPositionOf(last);
                        
                    }
                    var star = new TerrainAStar();
                    path = star.findPath(terrain.GetVoxelAt(start), terrain.GetVoxelAt(end));
                    if (path != null)
                        drawPath(path.Select(voxel => terrain.GetPositionOf(voxel) + new Vector3(0.5f)).ToArray());
                }));
            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }

        [Test]
        public void TestDrawPath()
        {
            setup();


            var path = new Vector3[] { new Vector3(), new Vector3(1, 0, 0), new Vector3(3, 0, 1) };

            engine.AddSimulator(new BasicSimulator(() => drawPath(path)));

            engine.Run();
        }

        private void drawPath(Vector3[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                TW.Graphics.LineManager3D.AddCenteredBox(path[i], 0.5f, new Color4(1, 0, 0));
            }
            for (int i = 0; i < path.Length - 1; i++)
            {
                TW.Graphics.LineManager3D.AddLine(path[i], path[i + 1], new Color4(1, 0, 0));
            }
        }
    }
}

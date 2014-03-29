using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Text;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using ProceduralBuilder.Building;
using ProceduralBuilder.Shapes;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class IslandSpaceAllocationTest
    {
        [Test]
        public void TestOneFaceBuildArea()
        {
            var buildSpaceSize = new Vector2(10, 10);
            var mat = Matrix.RotationX(-(float)Math.PI * 0.5f) * Matrix.RotationY((float)Math.PI) *
                      Matrix.Translation(buildSpaceSize.X, 0, 0);
            var buildSpace = new Face("", mat, buildSpaceSize);

            var spaceManager = new IslandSpaceManager { BuildAreaMeshes = new IBuildingElement[] { buildSpace }.ToList() };
            var rnd = new Random(0);
            var placedBoxes = generateBuildPlots(rnd, spaceManager, 10, 1f, 3f);

            DX11Game game = TW.Graphics;
            game.GameLoopEvent += delegate
            {
                TW.Graphics.LineManager3D.AddBox(buildSpace.GetBoundingBox(), new Color4(1, 1, 1));
                foreach (var box in placedBoxes)
                {
                    TW.Graphics.LineManager3D.AddBox(box, new Color4(1, 0, 0));
                }
            };
        }

        [Test]
        public void TestIslandBuildArea()
        {
            const int seed = 1;
            var generator = new IslandGenerator();
            var startShapes = generator.GetIslandBase(seed);
            IMesh islandMesh;
            List<IBuildingElement> navMesh;
            List<IBuildingElement> buildMesh;
            List<IBuildingElement> borderMesh;
            generator.GetIslandParts(startShapes, seed, false, out islandMesh, out navMesh, out buildMesh, out borderMesh);

            //buildMesh = buildMesh.Where(e => ((Face)e).GetBoundingBox().Maximum.Y < 1f).ToList();

            var spaceManager = new IslandSpaceManager { BuildAreaMeshes = buildMesh };

            var nbBoxes = 10;
            var rnd = new Random(0);
            var placedBoxes = generateBuildPlots(rnd, spaceManager, nbBoxes, 1f, 10f);

            DX11Game game = TW.Graphics;
            var infoDisplay = new TextTexture(game, 500, 50);
            game.GameLoopEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Key.F))
                {
                    spaceManager.ClearBuildingSpotReservations();
                    placedBoxes = generateBuildPlots(rnd, spaceManager, nbBoxes, 1f, 10f);
                }

                foreach (var el in buildMesh)
                {
                    TW.Graphics.LineManager3D.AddBox(el.GetBoundingBox(), new Color4(1, 1, 1));
                }
                foreach (var box in placedBoxes)
                {
                    TW.Graphics.LineManager3D.AddBox(box, new Color4(1, 0, 0));
                }

                infoDisplay.Clear();
                infoDisplay.DrawText("Press F to regenerate", new Vector2(0, 0), new Color4(1, 1, 1));
                infoDisplay.UpdateTexture();
                game.Device.ImmediateContext.ClearState();
                game.SetBackbuffer();
                game.Device.ImmediateContext.OutputMerger.BlendState = game.HelperStates.AlphaBlend;
                game.TextureRenderer.Draw(infoDisplay.GPUTexture.View, new Vector2(0, 0), new Vector2(500, 50));
                game.Device.ImmediateContext.ClearState();
                game.SetBackbuffer();
            };
        }

        private List<BoundingBox> generateBuildPlots(Random rnd, IslandSpaceManager spaceManager, int nbBoxes, float minSize, float maxSize)
        {
            var placedBoxes = new List<BoundingBox>();
            for (int i = 0; i < nbBoxes; i++)
            {
                var xSize = minSize + rnd.Next(1, 100) * 0.01f * (maxSize - minSize);
                var zSize = minSize + rnd.Next(1, 100) * 0.01f * (maxSize - minSize);

                var box = new BoundingBox(new Vector3(0, 0, 0), new Vector3(xSize, 1f, zSize));
                var pos = spaceManager.GetBuildPosition(box);

                if (pos != null)
                {
                    spaceManager.TakeBuildingSpot((Vector3)pos, box);
                    placedBoxes.Add(new BoundingBox(box.Minimum + (Vector3)pos, box.Maximum + (Vector3)pos));
                }
            }

            return placedBoxes;
        }
    }
}

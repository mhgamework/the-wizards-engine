using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
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

            var spaceManager = new IslandSpaceAllocator { BuildAreaMeshes = new IBuildingElement[] { buildSpace }.ToList() };
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

            var spaceManager = new IslandSpaceAllocator { BuildAreaMeshes = buildMesh };

            var nbBoxes = 10;
            var rnd = new Random(0);
            var placedBoxes = generateBuildPlots(rnd, spaceManager, nbBoxes, 1f, 10f);

            DX11Game game = TW.Graphics;
            var infoDisplay = new Textarea();
            infoDisplay.Size = new Vector2(500, 50);
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
                infoDisplay.Text = "Press F to regenerate";
            };
        }

        private List<BoundingBox> generateBuildPlots(Random rnd, IslandSpaceAllocator spaceAllocator, int nbBoxes, float minSize, float maxSize)
        {
            var placedBoxes = new List<BoundingBox>();
            for (int i = 0; i < nbBoxes; i++)
            {
                var xSize = minSize + rnd.Next(1, 100) * 0.01f * (maxSize - minSize);
                var zSize = minSize + rnd.Next(1, 100) * 0.01f * (maxSize - minSize);

                var box = new BoundingBox(new Vector3(0, 0, 0), new Vector3(xSize, 1, zSize));
                var pos = spaceAllocator.GetBuildPosition(box);

                if (pos != null)
                {
                    spaceAllocator.TakeBuildingSpot((Vector3)pos, box);
                    placedBoxes.Add(new BoundingBox(box.Minimum + (Vector3)pos, box.Maximum + (Vector3)pos));
                }
            }

            return placedBoxes;
        }
    }
}

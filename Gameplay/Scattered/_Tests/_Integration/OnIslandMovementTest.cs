using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using NSubstitute;
using NUnit.Framework;
using ProceduralBuilder.Building;
using ProceduralBuilder.Shapes;
using SlimDX;
using System.Linq;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class OnIslandMovementTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// Actually tests procutilities :p
        /// </summary>
        [Test]
        public void TestIsAboveFace()
        {
            var face = new Face("trolo", Matrix.RotationX(-MathHelper.PiOver2) * Matrix.Translation(0, 0, 0),
                                  new Vector2(2, 2));

            var eps = 0.0001f;

            Assert.True(isAboveFace(face, new Vector3(0, 2, 0)));
            Assert.True(isAboveFace(face, new Vector3(2-eps, 2, -2+eps)));
            Assert.True(isAboveFace(face, new Vector3(0, 0, 0)));

            Assert.False(isAboveFace(face, new Vector3(0, -1, 0)));

            Assert.False(isAboveFace(face, new Vector3(-0.1f, 2, 0)));
            Assert.False(isAboveFace(face, new Vector3(2.1f, 2, 0)));
            Assert.False(isAboveFace(face, new Vector3(0, 2, 0.1f)));
            Assert.False(isAboveFace(face, new Vector3(0, 2, -2.1f)));

        }

        [Obsolete]
        public static bool isAboveFace(Face f, Vector3 newPos)
        {
            return ProcUtilities.RaycastFace(f, new Ray(newPos, -Vector3.UnitY)).HasValue;
        }

        [Test]
        public void TestWalkAround()
        {
            var pos = new Vector3();
            var island = createIsland();

            var playerOnIslandMover = new PlayerSurfaceMover(ray => island.RaycastDetail(ProcUtilities.RaycastFace, ray).DistanceOrNull);

            // gameloop
            engine.AddSimulator(new BasicSimulator(() =>
                {
                    pos = playerOnIslandMover.ProcessUserMovement(pos);

                    TW.Graphics.SpectaterCamera.CameraPosition = pos;
                    drawFaces(island);
                }));


            engine.Run();

        }



        private static void drawFaces(List<Face> island)
        {
            island
                .ForEach(f => TW.Graphics.LineManager3D.AddBox(f.GetBoundingBox(), new Color4(0, 0, 0)));
        }


        private List<Face> createIsland()
        {
            return new[]
                {
                    new Face("trolo", Matrix.RotationX(-MathHelper.PiOver2)* Matrix.Translation(0, 0, 0), new Vector2(2, 2)),
                    new Face("trolo", Matrix.RotationX(-MathHelper.PiOver2)* Matrix.Translation(2, 0, -1), new Vector2(2, 4))

                }.ToList();
        }
    }
}
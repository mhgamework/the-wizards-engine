using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Visualization;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;
using TreeGenerator.EngineSynchronisation;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class CGCoreTest
    {
        [Test]
        public void TestGenerateRays()
        {
            PerspectiveCamera.Test();
        }

        [Test]
        public void TestSceneRaycaster()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var cam = new PerspectiveCamera();
            var resolution = new Point2(8, 8);
            cam.ProjectionPlaneDistance = 1.3f;

            var raycaster = new SceneRaytracer();

            var visualizer = new CameraVisualizer(TW.Graphics);

            var ent1 = createEntity();
            ent1.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -4));
            var ent2 = createEntity();
            ent2.WorldMatrix = Matrix.Translation(new Vector3(0, 0.5f, -6));
            raycaster.AddEntity(ent1);
            raycaster.AddEntity(ent2);

            engine.AddSimulator(new BasicSimulator(delegate
                                                       {
                                                           visualizer.RenderRays(cam, resolution);
                                                           for (int x = 0; x < resolution.X; x++)
                                                               for (int y = 0; y < resolution.Y; y++)
                                                               {
                                                                   Ray calculateRay = cam.CalculateRay(new Vector2((x + 0.5f) / resolution.X, (y + 0.5f) / resolution.Y));
                                                                   var res = raycaster.Raycast(calculateRay);
                                                                   if (!res.IsHit) continue;
                                                                   var point =
                                                                       calculateRay.Position =
                                                                       calculateRay.Direction * res.Distance;
                                                                   TW.Graphics.LineManager3D.AddCenteredBox(point, 0.1f,
                                                                                                            new Color4(
                                                                                                                0, 1, 0));
                                                               }
                                                       }));

            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();

        }

        [Test]
        public void TestGraphicalRayTracer()
        {
            var gui = new GraphicalRayTracer(new DummyTracer());

            

        }

        

        private WorldRendering.Entity createEntity()
        {
            var ret = new WorldRendering.Entity();

            ret.Mesh =
                OBJParser.OBJParserTest.GetBarrelMesh(
                    new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));


            return ret;

        }

        private class DummyTracer : IRayTracer
        {
            private Seeder seeder = new Seeder(123456789);

            public Color4 GetPixel(Vector2 pos)
            {
                /*for (int i = 0; i < 10000; i++)
                {
                    var a = Math.Pow(456, 456856.695465);
                }*/
                return new Color4(pos.X, pos.Y, 0);
            }
        }
    }
}

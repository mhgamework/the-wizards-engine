using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Texturing;
using MHGameWork.TheWizards.CG.Visualization;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
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
        public void TestBarrelRaycast()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var cam = new PerspectiveCamera();
            var resolution = new Point2(8, 8);
            cam.ProjectionPlaneDistance = 1.3f;

            var raycaster = new MeshFragmentTracer();

            var visualizer = new CameraVisualizer(TW.Graphics);

            var ent1 = createEntity();
            ent1.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -4));
            var ent2 = createEntity();

            engine.AddSimulator(new BasicSimulator(delegate
            {
                visualizer.RenderRays(cam, resolution);

                var lines = new LineManager3DLines();

                for (int x = 0; x < resolution.X; x++)
                    for (int y = 0; y < resolution.Y; y++)
                    {
                        Ray calculateRay = cam.CalculateRay(new Vector2((x + 0.5f) / resolution.X, (y + 0.5f) / resolution.Y));
                        var res = raycaster.TraceFragment(new RayTrace(calculateRay, 0, float.MaxValue));
                        if (res.Clip) continue;
                        var point = res.Position;
                        TW.Graphics.LineManager3D.AddCenteredBox(point, 0.1f,
                                                                 new Color4(
                                                                     0, 1, 0));
                        TW.Graphics.LineManager3D.AddLine(point,
                                                          point + res.Normal,
                                                          new Color4(1, 1, 0));
                    }
            }));

            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }

        [Test]
        public void TestTriangleRaycast()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var cam = new PerspectiveCamera();
            var resolution = new Point2(64, 64);
            cam.ProjectionPlaneDistance = 1.3f;

            var raycaster = new MeshFragmentTracer();

            var visualizer = new CameraVisualizer(TW.Graphics);

            var ent1 = createTriangleEntity();
            ent1.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -4));

            raycaster.AddEntity(ent1);

            engine.AddSimulator(new BasicSimulator(delegate
                                                       {
                                                           //visualizer.RenderRays(cam, resolution);
                                                           for (int x = 0; x < resolution.X; x++)
                                                               for (int y = 0; y < resolution.Y; y++)
                                                               {
                                                                   Ray calculateRay = cam.CalculateRay(new Vector2((x + 0.5f) / resolution.X, (y + 0.5f) / resolution.Y));
                                                                   var res = raycaster.TraceFragment(new RayTrace(calculateRay, 0, float.MaxValue));
                                                                   if (res.Clip) continue;
                                                                   var point = res.Position;
                                                                   TW.Graphics.LineManager3D.AddCenteredBox(point, 0.03f, res.Diffuse);
                                                                   TW.Graphics.LineManager3D.AddLine(point,
                                                                                                     point + res.Normal,
                                                                                                     new Color4(1, 1, 0));
                                                               }
                                                       }));

            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();

        }


        [Test]
        public void TestRaycastTriangle()
        {

            var cam = new PerspectiveCamera();
            cam.ProjectionPlaneDistance = 1.3f;

            var raycaster = new MeshFragmentTracer();

            var ent1 = createTriangleEntity();
            ent1.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -4));

            raycaster.AddEntity(ent1);

            var window = new GraphicalRayTracer(new Tracer(raycaster, cam));


        }

        [Test]
        public void TestRetardShader()
        {

            var cam = new PerspectiveCamera();
            cam.ProjectionPlaneDistance = 1.3f;

            var raycaster = new MeshFragmentTracer();

            var ent1 = createSphereEntity();
            ent1.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -3));

            raycaster.AddEntity(ent1);

            var window = new GraphicalRayTracer(new RetardTracer(raycaster, cam));


        }

        [Test]
        public void TestRetardShaderShadows()
        {

            var cam = new PerspectiveCamera();
            cam.ProjectionPlaneDistance = 1.3f;

            var raycaster = new MeshFragmentTracer();

            var ent1 = createEntity();
            ent1.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -3));

            var ent2 = createSphereEntity();
            ent2.WorldMatrix = Matrix.Translation(new Vector3(0, 1, -2));

            raycaster.AddEntity(ent1);
            //raycaster.AddEntity(ent2);

            var window = new GraphicalRayTracer(new RetardTracer(raycaster, cam));


        }


        [Test]
        public void TestGraphicalRayTracer()
        {
            var gui = new GraphicalRayTracer(new DummyTracer());

        }


        public void TestTextureSampler()
        {
            var cache = new SimpleTexture2DLoader();
            var tex = cache.Load(createEntity().Mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap);
            var sampler = new Texture2DSampler();

            var ui = new GraphicalRayTracer(new SamplerTracer(tex, sampler));
        }

        private class SamplerTracer : IRayTracer
        {
            private Texture2D tex;
            private Texture2DSampler sampler;

            public SamplerTracer(Texture2D tex, Texture2DSampler sampler)
            {
                this.tex = tex;
                this.sampler = sampler;
            }

            public Color4 GetPixel(Vector2 pos)
            {
                return sampler.SampleBilinear(tex, pos);
            }
        }


        public class RetardTracer : IRayTracer
        {
            private ICamera camera;
            private RetardShader shader;

            public RetardTracer(IFragmentTracer fragmentTracer, ICamera camera)
            {
                this.camera = camera;
                shader = new RetardShader(fragmentTracer, camera);
            }

            public Color4 GetPixel(Vector2 pos)
            {
                var rayTrace = new RayTrace(camera.CalculateRay(pos), 0, int.MaxValue);
                return shader.CalculateRayColor(rayTrace);
            }
        }

        public class Tracer : IRayTracer
        {
            private IFragmentTracer fragmentTracer;
            private ICamera camera;

            public Tracer(IFragmentTracer fragmentTracer, ICamera camera)
            {
                this.fragmentTracer = fragmentTracer;
                this.camera = camera;
            }

            public Color4 GetPixel(Vector2 pos)
            {
                //return new Color4(pos.X, pos.Y,0);
                var input = fragmentTracer.TraceFragment(new RayTrace(camera.CalculateRay(pos), 0, int.MaxValue));

                if (input.Position.X > 0 && input.Normal.X != 0)
                {
                    int asfds = 8;

                }

                return new Color4((input.Normal.X + 1) * 0.5f, (input.Normal.Y + 1) * 0.5f, 0);
            }
        }



        private WorldRendering.Entity createSphereEntity()
        {
            var ret = new WorldRendering.Entity();

            var builder = new MeshBuilder();
            builder.AddSphere(12, 1);
            var mesh = builder.CreateMesh();

            ret.Mesh = mesh;

            return ret;

        }

        private WorldRendering.Entity createTriangleEntity()
        {
            var ret = new WorldRendering.Entity();


            TangentVertex[] vertices = new TangentVertex[3];


            vertices[0] = new TangentVertex(new Vector3(-1, 0, 0).xna(), new Vector2(0, 0).xna(), Vector3.Normalize(new Vector3(-1, -1, 1)).xna(), Vector3.Zero.xna());
            vertices[1] = new TangentVertex(new Vector3(1, 0, 0).xna(), new Vector2(1, 0).xna(), Vector3.Normalize(new Vector3(1, -1, 1)).xna(), Vector3.Zero.xna());
            vertices[2] = new TangentVertex(new Vector3(0, 1, 0).xna(), new Vector2(0.5f, 1).xna(), Vector3.Normalize(new Vector3(0, 1, 1)).xna(), Vector3.Zero.xna());



            var part = new RAMMeshPart();
            part.GetGeometryData().SetSourcesFromTangentVertices(new short[] { 0, 1, 2 }, vertices);

            var mesh = new RAMMesh();

            mesh.GetCoreData().Parts.Add(new MeshCoreData.Part { MeshPart = part, ObjectMatrix = Matrix.Translation(0, 0, 0).xna(), MeshMaterial = new MeshCoreData.Material { DiffuseColor = new Color4(0, 1, 0).xna() } });


            ret.Mesh = mesh;

            return ret;

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
                for (int i = 0; i < 1000; i++)
                {
                    var a = Math.Pow(456, 456856.695465);
                }
                return new Color4(pos.X, pos.Y, 0);
            }
        }
    }
}

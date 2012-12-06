using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.OBJParser;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class GenericTraceableSceneTest
    {
        [Test]
        public void TestRenderSphere()
        {
            var scene = new GenericTraceableScene();
            var cam = new PerspectiveCamera();
            cam.ProjectionPlaneDistance = 1.3f;


            var shader = new PhongShader(scene, cam);

            scene.AddGenericSurface(new SphereSurface(shader, new BoundingSphere(new Vector3(0, 0, 0 - 3), 1f)));


            var window = new GraphicalRayTracer(new TracedSceneImage(scene, cam), 1);


        }

        [Test]
        public void TestRaycastGenericSphere()
        {
            var s = new SphereSurface(null, new BoundingSphere(new Vector3(0, 0, 0), 1f));
            Ray ray = new Ray(new Vector3(-0.8f, 0, 0), new Vector3(1, 0, 0));
            float? result;
            s.IntersectsSphere(ref ray, out result);

        }

        [Test]
        public void TestRenderPlane()
        {
            var scene = new GenericTraceableScene();
            var cam = new PerspectiveCamera();
            cam.Position = new Vector3(0, 1, 0);
            cam.ProjectionPlaneDistance = 1.3f;


            var shader = new PhongShader(scene, cam);

            scene.AddGenericSurface(new PlaneSurface(shader, new Plane(Vector3.UnitY, 0)));

            var window = new GraphicalRayTracer(new TracedSceneImage(scene, cam), 1);


        }

        [Test]
        public void TestComplex()
        {
            var scene = new ComplexTestScene();
            var window = new GraphicalRayTracer(new TracedSceneImage(scene.Scene, scene.Camera), 1);
        }


        [Test]
        public void TestTriangleSurface()
        {
            var f = new CGFactory();

            var scene = f.CreateGenericTraceableScene();
            var shader = f.CreatePhong();

            List<TriangleSurface> triangles = getTriangles(shader, MeshLoader.LoadMeshFromObj(new FileInfo(TestFiles.BarrelObj)));

            foreach (var tri in triangles)
            {
                scene.AddGenericSurface(tri);
            }

            f.CreatePerspectiveCamera(new SlimDX.Vector3(0, 5, 5), new Vector3());
            f.Run(1);



        }

        private List<TriangleSurface> getTriangles(PhongShader shader, IMesh mesh)
        {
            var converter = new MeshToTriangleConverter();
            return converter.GetTriangles(mesh, shader);
        }

        [Test]
        public void TestCompactGridSurface()
        {
            var f = new CGFactory();

            var scene = f.CreateGenericTraceableScene();
            var shader = f.CreatePhong();

            List<TriangleSurface> triangles = getTriangles(shader, MeshLoader.LoadMeshFromObj(new FileInfo(TWDir.GameData + "\\Core\\Dragon\\dragon.obj")));

            var grid = new CompactGrid();
            grid.buildGrid(triangles.Select(o => (ISurface)o).ToList());
            scene.AddGenericSurface(new CompactGridSurface(grid));

            f.CreatePerspectiveCamera(new SlimDX.Vector3(-4, 1, 0), new Vector3(0,0,0));
            f.Run(1);




        }

        /// <summary>
        /// Creates an image from a traceablescene and a camera
        /// </summary>
        private class TracedSceneImage : IRenderedImage
        {
            private readonly ITraceableScene scene;
            private readonly ICamera camera;

            public TracedSceneImage(ITraceableScene scene, ICamera camera)
            {
                this.scene = scene;
                this.camera = camera;
            }

            public Color4 GetPixel(Vector2 pos)
            {
                var rayTrace = new RayTrace(camera.CalculateRay(pos), 0, int.MaxValue);
                IShadeCommand cmd;
                scene.Intersect(rayTrace, out cmd, true);
                if (cmd == null) return new Color4();
                return cmd.CalculateColor();

            }
        }
    }
}

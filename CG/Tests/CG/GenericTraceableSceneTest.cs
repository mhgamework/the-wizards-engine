using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.CG.UI;
using NUnit.Framework;

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

            scene.AddGenericSurface(new SphereGeometricSurface(shader, new BoundingSphere(new Vector3(0, 0, 0 - 3), 1f)));


            var window = new GraphicalRayTracer(new TracedSceneImage(scene, cam));


        }

        [Test]
        public void TestRaycastGenericSphere()
        {
            var s = new SphereGeometricSurface(null, new BoundingSphere(new Vector3(0, 0, 0), 1f));
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

            scene.AddGenericSurface(new PlaneGeometricSurface(shader, new Plane(Vector3.UnitY, 0)));

            var window = new GraphicalRayTracer(new TracedSceneImage(scene, cam));


        }

        [Test]
        public void TestComplex()
        {
            var scene = new ComplexTestScene();
            var window = new GraphicalRayTracer(new TracedSceneImage(scene.Scene, scene.Camera));
        }


        [Test]
        public void TestTriangleSurface()
        {
            var f = new CGFactory();

            var scene = f.CreateGenericTraceableScene();
            var shader = f.CreatePhong();

            List<TriangleGeometricSurface> triangles = getTriangles(shader, f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\barrel.obj")));

            foreach (var tri in triangles)
            {
                scene.AddGenericSurface(tri);
            }

            f.CreatePerspectiveCamera(new Vector3(0, 5, 5), new Vector3());
            f.Run();



        }

        private List<TriangleGeometricSurface> getTriangles(PhongShader shader, RAMMesh mesh)
        {
            var converter = new MeshToTriangleConverter();
            return converter.GetTriangles(mesh, shader);
        }

        [Test]
        public void TestCompactGridSceneObject()
        {
            var f = new CGFactory();

            var scene = f.CreateGenericTraceableScene();
            var shader = f.CreatePhong();

            List<TriangleGeometricSurface> triangles = getTriangles(shader, f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\Dragon\\dragon.obj")));

            var grid = new CompactGrid();
            grid.buildGrid(
                triangles.Select(
                    o => (ISceneObject)new GeometrySceneObject { GeometricSurface = o, Shader = shader }).ToList());
            scene.AddSceneObject(new CompactGridGeometricSurface(grid));

            f.CreatePerspectiveCamera(new Vector3(-4, 1, 0), new Vector3(0, 0, 0));
            f.Run();




        }

    }
}

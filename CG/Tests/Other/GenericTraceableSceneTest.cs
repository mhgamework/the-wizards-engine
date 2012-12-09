using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.CG.UI;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests.Other
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

            var surface = new SphereGeometricSurface(1f);

            var obj = new TransformedSceneObject(new GeometrySceneObject (surface,shader)) { Transformation = Matrix.Translation(0, 0, -3) };

            scene.AddSceneObject(obj);


            var window = new GraphicalRayTracer(new TracedSceneImage(scene, cam));


        }

        [Test]
        public void TestRaycastGenericSphere()
        {
            var s = new SphereGeometricSurface(1f);
            Ray ray = new Ray(new Vector3(-0.8f, 0, 0), new Vector3(1, 0, 0));
            float? result;
            s.IntersectsSphere(ref ray, out result);

        }

       

        [Test]
        public void TestComplex()
        {
            var scene = new ComplexTestScene();
            var window = new GraphicalRayTracer(new TracedSceneImage(scene.Scene, scene.Camera));
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
                    o => (ISceneObject)new GeometrySceneObject (o,shader)).ToList());
            scene.AddSceneObject(new CompactGridGeometricSurface(grid));

            f.CreatePerspectiveCamera(new Vector3(-4, 1, 0), new Vector3(0, 0, 0));
            f.Run();




        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Tests.Other;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests
{
    [TestFixture]
    public class GeometryTest
    {
        [Test]
        public void TestSphere()
        {
            var f = new CGFactory();
            var scene = f.GetScene();
            var cam = f.CreatePerspectiveCamera(new Math.Vector3(0, 3, -10), new Math.Vector3());

            var surface = new SphereGeometricSurface(1);

            scene.AddSceneObject(new GeometrySceneObject(surface, f.CreatePhong()));

            f.Run();

        }
        [Test]
        public void TestSphereNormals()
        {
            var f = new CGFactory();
            var scene = f.GetScene();
            var cam = f.CreatePerspectiveCamera(new Math.Vector3(0, 3, -10), new Math.Vector3());

            var surface = new SphereGeometricSurface(1);

            scene.AddSceneObject(new GeometrySceneObject(surface,new NormalsShader()));

            f.Run();

        }
        [Test]
        public void TestPlane()
        {
            var f = new CGFactory();
            var scene = f.GetScene();
            var cam = f.CreatePerspectiveCamera(new Math.Vector3(0, 3, -10), new Math.Vector3());

            var surface = new PlaneGeometricSurface(new Plane(Vector3.Up, 0));

            scene.AddSceneObject(new GeometrySceneObject(surface, f.CreatePhong()));

            f.Run();

        }
        [Test]
        public void TestTriangle()
        {
            var f = new CGFactory();
            var scene = f.GetScene();
            var cam = f.CreatePerspectiveCamera(new Math.Vector3(0, 3, -10), new Math.Vector3());

            TangentVertex[] vertices = new TangentVertex[3];
            vertices[0].pos = new Vector3(-1, 0, -1);
            vertices[1].pos = new Vector3(1, 0, -1);
            vertices[2].pos = new Vector3(0, 0, 1);
            vertices[0].normal = Vector3.Up;
            vertices[1].normal = Vector3.Up;
            vertices[2].normal = Vector3.Up;
            var surface = new TriangleGeometricSurface(vertices,0);

            scene.AddSceneObject(new GeometrySceneObject(surface, f.CreatePhong()));

            f.Run();

        }
    }
}

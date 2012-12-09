using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.CG.Tests.Other;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests
{
    [TestFixture]
    public class SceneObjectsTest
    {
        [Test]
        public void TestTransformedSphereTranslation()
        {
            testTransformedSphere(Matrix.Translation(0, 2, -1));
        }
        [Test]
        public void TestTransformedSphereTranslationScaling()
        {


            var transformation = Matrix.Scaling(2, 1, 1) * Matrix.Translation(0, 2, 0);
            var f = new CGFactory();

            var geom = new SphereGeometry(1);
            var obj = new TransformedSceneObject(new GeometrySceneObject(geom, f.CreatePhong()));
            obj.Transformation = transformation;

            f.AddGroundPlane();
            f.GetScene().AddSceneObject(new GeometrySceneObject((new SphereGeometry(1) { Position = new Vector3(0, 1, 0) }), f.CreatePhong()));

            f.GetScene().AddSceneObject(obj);

            f.Run();
        }
        [Test]
        public void TestTransformedSphereTranslationScalingNormals()
        {


            var transformation= Matrix.Scaling(2, 1, 1)*Matrix.Translation(0, 2, 0);
            var f = new CGFactory();

            var geom = new SphereGeometry(1);
            var obj = new TransformedSceneObject(new GeometrySceneObject(geom, new NormalsShader()));
            obj.Transformation = transformation;

            f.AddGroundPlane();
            f.GetScene().AddSceneObject(new GeometrySceneObject((new SphereGeometry(1) { Position = new Vector3(0, 1, 0) }), new NormalsShader()));

            f.GetScene().AddSceneObject(obj);

            f.Run();
        }

        private void testTransformedSphere(Matrix transformation)
        {
            var f = new CGFactory();

            var geom = new SphereGeometry(1);
            var obj = new TransformedSceneObject(f.CreateSceneObject(geom));
            obj.Transformation = transformation;

            f.AddGroundPlane();
            f.GetScene().AddSceneObject(f.CreateSceneObject(new SphereGeometry(1) {Position = new Vector3(0, 1, 0)}));

            f.GetScene().AddSceneObject(obj);

            f.Run();
        }

        [Test]
        public void TestCompactGrid()
        {
            var transformation = Matrix.Scaling(2, 1, 1) * Matrix.Translation(0, 2, 0);
            var f = new CGFactory();


            f.AddGroundPlane();



            var mesh = f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\Barrel01.obj"));

            var shader = f.CreatePhong();

            var converter = new MeshToTriangleConverter();

            List<TriangleGeometry> triangles = converter.GetTrianglesWithPhong(mesh, f.CreatePhong);

            var grid = new CompactGrid();
            grid.buildGrid(triangles.Select(o => (ISceneObject)new GeometrySceneObject(o, shader)).ToList());
            f.GetScene().AddSceneObject(new CompactGridGeometricSurface(grid));


            f.Run();
        }
    }
}

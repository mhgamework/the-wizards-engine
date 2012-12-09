using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.CG.Tests.Other;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests.Spatial
{
    [TestFixture]
    public class BoundingVolumeHierarchyTest
    {
        [Test]
        public void TestBoundingVolumeHierarchySimple()
        {
            var f = new CGFactory();
                var mesh = f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\Barrel01.obj"));

            var shader = f.CreatePhong();

            var converter = new MeshToTriangleConverter();

            List<TriangleGeometry> triangles = converter.GetTrianglesWithPhong(mesh, f.CreatePhong);


            var builder = new BoundingVolumeHierarchyBuilder();
            var root = builder.CreateNode(triangles.Select(o => (ISceneObject) new GeometrySceneObject(o, shader)), 0);

            f.GetScene().AddSceneObject(root);

            f.Run();


        }
        [Test]
        public void TestBoundingVolumeHierarchyDragon()
        {
            var f = new CGFactory();
            f.CreatePerspectiveCamera(new Vector3(-4, 2, 0), new Vector3(0, 0, 0));
            var mesh = f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\Dragon\\dragon.obj"));

            var shader = f.CreatePhong();

            var converter = new MeshToTriangleConverter();

            List<TriangleGeometry> triangles = converter.GetTrianglesWithPhong(mesh, f.CreatePhong);


            var builder = new BoundingVolumeHierarchyBuilder();
            var root = builder.CreateNode(triangles.Select(o => (ISceneObject)new GeometrySceneObject(o, shader)), 0);

            f.GetScene().AddSceneObject(root);

            f.Run();


        }
    }
}

using System.Collections.Generic;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Shading;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests.Other
{
    [TestFixture]
    public class CGShowCase
    {
        [Test]
        public void TestDragon()
        {
            //var f = new CGFactory();

            //var scene = f.CreateGenericTraceableScene();
            //var shader = f.CreateRefraction();

            //List<TriangleGeometry> triangles = getTriangles(shader, f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\Dragon\\dragon.obj")));

            //var grid = new CompactGrid();
            //grid.buildGrid(triangles.Select(o => (IGeometry)o).ToList());
            //var gridSurface = new CompactGridGeometricSurface(grid);
            //gridSurface.CastsShadows = false;
            //scene.AddGenericSurface(gridSurface);

            //scene.AddGenericSurface(new PlaneGeometry(f.CreatePhong(), new Plane(Vector3.UnitY, 0)));

            //f.CreatePerspectiveCamera(new Vector3(-4, 1, 0), new Vector3(0, 0, 0));
            //f.Run(1);


        }

        private List<TriangleGeometry> getTriangles(IShader shader, RAMMesh mesh)
        {
            var converter = new MeshToTriangleConverter();
            return converter.GetTriangles(mesh, shader);
        }

    }
}

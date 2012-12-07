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
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.OBJParser;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class CGShowCase
    {
        [Test]
        public void TestDragon()
        {
            var f = new CGFactory();

            var scene = f.CreateGenericTraceableScene();
            var shader = f.CreateRefraction();

            List<TriangleSurface> triangles = getTriangles(shader, f.CreateMesh(new FileInfo(TWDir.GameData + "\\Core\\Dragon\\dragon.obj")));

            var grid = new CompactGrid();
            grid.buildGrid(triangles.Select(o => (ISurface)o).ToList());
            var gridSurface = new CompactGridSurface(grid);
            gridSurface.CastsShadows = false;
            scene.AddGenericSurface(gridSurface);

            scene.AddGenericSurface(new PlaneSurface(f.CreatePhong(), new Plane(Vector3.UnitY, 0)));

            f.CreatePerspectiveCamera(new Vector3(-4, 1, 0), new Vector3(0, 0, 0));
            f.Run(1);


        }

        private List<TriangleSurface> getTriangles(IShader shader, RAMMesh mesh)
        {
            var converter = new MeshToTriangleConverter();
            return converter.GetTriangles(mesh, shader);
        }

    }
}

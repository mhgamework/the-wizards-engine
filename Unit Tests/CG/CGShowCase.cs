using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
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
            var ui = new GraphicalRayTracer();

            var mesh =  OBJParserTest.GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));
            var grid = new CompactGrid();

            var converter = new MeshToTriangleConverter();
            var triangles = converter.GetTriangles(mesh);

            grid.buildGrid(triangles.Select(o => (ISurface)o).ToList());


            var scene = new GenericTraceableScene();
            scene.AddGenericSurface(new CompactGridSurface(grid));

            var cam = new PerspectiveCamera();
            cam.Position = new Vector3(0, 5, 30);
            ui.Run(new TracedSceneImage(scene, cam));



        }
    }
}

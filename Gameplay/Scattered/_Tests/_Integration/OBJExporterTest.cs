using System.IO;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered._Engine;
using MHGameWork.TheWizards.WorldRendering;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    public class OBJExporterTest
    {

        [Test]
        public void TestExport()
        {
            var tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = "trolo.png";

            var mesh = UtilityMeshes.CreateBoxWithTexture(tex, new Vector3(10, 10, 10));


            var exporter = new OBJExporter();

            var objMesh = exporter.ConvertFromTWMesh(mesh);

            exporter.SaveToFile(objMesh, TWDir.GameData.CreateSubdirectory("Scattered\\Tests").FullName + "\\OBJExporterTrolo.obj");
        }

        [Test]
        public void TestExportComplex()
        {
            var tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = "trolo.png";


            var meshFact = new EngineMeshFactory(new RAMTextureFactory());
            var mesh = meshFact.loadMeshFromFile(TWDir.GameData.FullName + "\\Core\\MerchantsHouse.obj",
                                                 TWDir.GameData.FullName + "\\Core\\MerchantsHouse.mtl",
                                                 "MerchantsHouse.mtl");
            var exporter = new OBJExporter();

            var objMesh = exporter.ConvertFromTWMesh(mesh);


            exporter.SaveToFile(objMesh, TWDir.GameData.CreateSubdirectory("Scattered\\Tests").FullName + "\\OBJExporterHouse.obj");
        }

     
    }
}
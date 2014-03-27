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
        private Matrix transformation;

        [Test]
        public void TestExport()
        {
            var tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = "trolo.png";

            var mesh = UtilityMeshes.CreateBoxWithTexture(tex, new Vector3(10, 10, 10));


            var objMesh = createObjMeshFromTWMesh(mesh);

            var exporter = new OBJExporter();

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

            var objMesh = createObjMeshFromTWMesh(mesh);

            var exporter = new OBJExporter();

            exporter.SaveToFile(objMesh, TWDir.GameData.CreateSubdirectory("Scattered\\Tests").FullName + "\\OBJExporterHouse.obj");
        }

        private OBJExporter.IMesh createObjMeshFromTWMesh(IMesh mesh)
        {
            var objMesh = Substitute.For<OBJExporter.IMesh>();
            objMesh.Parts.Returns(mesh.GetCoreData().Parts.Select(part =>
                {
                    var ret = Substitute.For<OBJExporter.IMeshPart>();
                    ret.DiffuseTexture.Returns(part.MeshMaterial.DiffuseMap == null ? null : new FileInfo(part.MeshMaterial.DiffuseMap.GetCoreData().DiskFilePath));
                    transformation = part.ObjectMatrix.dx();
                    ret.Positions.Returns(
                        Vector3.TransformCoordinate(
                            part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Select(
                                v => v.dx()).ToArray(),
                            ref transformation));
                    ret.Normals.Returns(
                        Vector3.TransformNormal(
                            part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal).Select(
                                v => v.dx()).ToArray(),
                            ref transformation));
                    ret.Texcoords.Returns(
                        part.MeshPart.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord).Select(
                            v => v.ToSlimDX()).ToArray());
                    return ret;
                }));
            return objMesh;
        }
    }
}
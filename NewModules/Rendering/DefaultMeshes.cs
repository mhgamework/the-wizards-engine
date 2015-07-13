using System.IO;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Tests;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    public static class DefaultMeshes
    {
        public static RAMMesh CreateMerchantsHouseMeshOLD()
        {
            ObjImporter importer = new ObjImporter();


            importer.AddMaterialFileStream("MerchantsHouse.mtl", new FileStream(TestFiles.MerchantsHouseMtl, FileMode.Open));
            importer.ImportObjFile(TestFiles.MerchantsHouseObj);

            var textureFactory = new RAMTextureFactory();
            /*textureFactory.AddAssemblyResolvePath(typeof(ObjImporter).Assembly,
                                                  "MHGameWork.TheWizards.OBJParser.Files.maps");*/
            var conv = new OBJToRAMMeshConverter(textureFactory);
            return conv.CreateMesh(importer);
        }

        public static RAMMesh CreateMerchantsHouseMesh(OBJToRAMMeshConverter c)
        {
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream("MerchantsHouse.mtl", File.OpenRead(TestFiles.MerchantsHouseMtl));
            importer.ImportObjFile(TestFiles.MerchantsHouseObj);

            return c.CreateMesh(importer);
        }

        public static RAMMesh CreateGuildHouseMesh(OBJToRAMMeshConverter c)
        {
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream("GuildHouse01.mtl", File.OpenRead(TestFiles.GuildHouseMtl));
            importer.ImportObjFile(TestFiles.GuildHouseObj);

            return c.CreateMesh(importer);
        }

        public static RAMMesh CreateMeshFromObj(OBJToRAMMeshConverter c, string obj, string mtl)
        {
            var fi = new FileInfo(mtl);
            ObjImporter importer;
            importer = new ObjImporter();
            importer.AddMaterialFileStream(fi.Name, File.OpenRead(mtl));
            importer.ImportObjFile(obj);

            return c.CreateMesh(importer);
        }

        public static RAMTexture GetTestTexture()
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = TestFiles.BrickRoundJPG;
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/
            return tex;
        }

        public static IMesh CreateSimpleTestMesh()
        {
            IMesh mesh;

            mesh = new RAMMesh();

            var part = new MeshCoreData.Part();
            part.ObjectMatrix = Matrix.Identity;
            part.MeshPart = new RAMMeshPart();
            ((RAMMeshPart)part.MeshPart).SetGeometryData(MeshPartGeometryData.CreateTestSquare());

            var mat = new MeshCoreData.Material();

            mat.DiffuseMap = GetTestTexture();

            part.MeshMaterial = mat;
            mesh.GetCoreData().Parts.Add(part);

            return mesh;
        }
    }
}
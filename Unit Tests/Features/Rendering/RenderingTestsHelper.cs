using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    public static class RenderingTestsHelper
    {
        public static RAMMesh CreateMerchantsHouseMesh(OBJToRAMMeshConverter c)
        {
            return RenderingTest.CreateMerchantsHouseMesh(c);
        }

        public static RAMMesh CreateGuildHouseMesh(OBJToRAMMeshConverter c)
        {
            return RenderingTest.CreateGuildHouseMesh(c);
        }

        public static RAMMesh CreateMeshFromObj(OBJToRAMMeshConverter c, string obj, string mtl)
        {
            return RenderingTest.CreateMeshFromObj(c, obj, mtl);
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
    }
}

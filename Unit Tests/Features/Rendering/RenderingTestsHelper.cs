﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
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

        private static RAMTexture loadTexture(string file)
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = file;
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/
            return tex;
        }


        public static RAMTexture GetTestTexture() { return loadTexture(TestFiles.BrickRoundJPG); }
        public static RAMTexture GetDiffuseMap() { return loadTexture(TWDir.GameData + @"\Rendering\BrickOldRounded\BrickOldRounded_COLOR.png"); }
        public static RAMTexture GetNormalMap() { return loadTexture(TWDir.GameData + @"\Rendering\BrickOldRounded\BrickOldRounded_NRM.png"); }
        public static RAMTexture GetSpecularMap() { return loadTexture(TWDir.GameData + @"\Rendering\BrickOldRounded\BrickOldRounded_SPEC.png"); }
        public static RAMTexture GetDiffuseMapAlpha() { return loadTexture(TWDir.GameData + @"\Rendering\Ivy\Ivy_COLOR.png"); }
        public static RAMTexture GetNormalMapAlpha() { return loadTexture(TWDir.GameData + @"\Rendering\Ivy\Ivy_NRM.png"); }
        public static RAMTexture GetSpecularMapAlpha() { return loadTexture(TWDir.GameData + @"\Rendering\Ivy\Ivy_SPEC.png"); }

        public static IMesh CreateSphere(ITexture diffuse, ITexture normal, ITexture specular)
        {
            var builder = new MeshBuilder();
            builder.AddSphere(20, 1);
            var mesh = builder.CreateMesh();

            mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = diffuse;
            //TODO!!!

            return mesh;
        }

        public static IMeshPart CreateSphereMeshPart()
        {
            var mesh = CreateSphere(null, null, null);
            return mesh.GetCoreData().Parts[0].MeshPart;
        }



    }
}

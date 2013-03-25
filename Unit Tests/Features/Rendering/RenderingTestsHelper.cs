using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Tests.Features.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using Microsoft.Xna.Framework;
using SlimDX.Direct3D11;
using MathHelper = DirectX11.MathHelper;
using Vector3 = SlimDX.Vector3;

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
            mesh.GetCoreData().Parts[0].MeshMaterial.NormalMap = normal;
            mesh.GetCoreData().Parts[0].MeshMaterial.SpecularMap = specular;

            return mesh;
        }

        public static IMeshPart CreateSphereMeshPart()
        {
            var mesh = CreateSphere(null, null, null);
            return mesh.GetCoreData().Parts[0].MeshPart;
        }


        public class SimpleLightedScene
        {
            private readonly DX11Game game;
            private readonly GBuffer buffer;
            private float angle;
            private CombineFinalRenderer combineFinal;
            private DeviceContext ctx;
            private PointLightRenderer point;

            public SimpleLightedScene(DX11Game game, GBuffer buffer)
            {
                this.game = game;
                this.buffer = buffer;
                ctx = game.Device.ImmediateContext;
                // Non-related init code

                point = new PointLightRenderer( game,buffer );

                point.LightRadius = 3;
                point.LightIntensity = 1;
                point.ShadowsEnabled = false;

                angle = 0;

                combineFinal = new CombineFinalRenderer(game, buffer);
            }

            public void Render()
            {

                angle += MathHelper.Pi * game.Elapsed;
                point.LightPosition = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), -2);

                ctx.ClearState();
                combineFinal.SetLightAccumulationStates();
                combineFinal.ClearLightAccumulation();
                point.Draw();

                ctx.ClearState();
                game.SetBackbuffer();
                ctx.Rasterizer.SetViewports(new Viewport(400, 300, 400, 300));

                combineFinal.DrawCombined();


                game.SetBackbuffer();
                GBufferTest.DrawGBuffer(game, buffer);

            }
        }
    }
}

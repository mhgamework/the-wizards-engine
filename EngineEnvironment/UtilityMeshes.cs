using System;
using System.IO;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Text;
using SlimDX;
using SlimDX.Direct3D11;
using MathHelper = DirectX11.MathHelper;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector2 = SlimDX.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MHGameWork.TheWizards.RTSTestCase1
{
    /// <summary>
    /// Creates utility meshes usefull for debugging
    /// </summary>
    public class UtilityMeshes
    {
        /// <summary>
        /// Creates a new box mesh with given texture
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static IMesh CreateMeshWithTexture(float radius, ITexture texture)
        {
            return CreateBoxWithTexture(texture, new SlimDX.Vector3(radius));
        }

        public static IMesh CreateBoxColored(Color4 color, SlimDX.Vector3 radius)
        {
            var builder = new MeshBuilder();
            builder.AddBox(-radius, radius);
            var mesh = builder.CreateMesh();

            mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseColor = color.xna();
            mesh.GetCoreData().Parts[0].MeshMaterial.ColoredMaterial = true;

            addBoxCollisionData(radius, mesh);

            return mesh;
        }

        public static IMesh CreateBoxColoredSize(Color4 color, SlimDX.Vector3 size)
        {
            var builder = new MeshBuilder();
            builder.AddBox(new SlimDX.Vector3(), size);
            var mesh = builder.CreateMesh();

            mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseColor = color.xna();
            mesh.GetCoreData().Parts[0].MeshMaterial.ColoredMaterial = true;

            addBoxCollisionDataSize(size, mesh);

            return mesh;
        }

        private static void addBoxCollisionData(SlimDX.Vector3 radius, IMesh mesh)
        {
            var skin = 0.02f;
            mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box()
                {
                    Dimensions = radius.xna() * 2 + Vector3.One * skin,
                    Orientation = Matrix.Identity
                });
        }

        private static void addBoxCollisionDataSize(SlimDX.Vector3 size, IMesh mesh)
        {
            var skin = 0.02f;
            mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box()
            {
                Dimensions = size.xna() + Vector3.One * skin,
                Orientation = Matrix.Identity
            });
        }

        public static IMesh CreateBoxWithTexture(ITexture texture, SlimDX.Vector3 dimensions)
        {
            var builder = new MeshBuilder();
            builder.AddBox(-dimensions, dimensions);
            var mesh = builder.CreateMesh();

            mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = texture;

            addBoxCollisionData(dimensions, mesh);

            return mesh;
        }

        /// <summary>
        /// TODO: this is somewhat haxor
        /// Note: fontSize should do nothing :p remove this
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="text"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static IMesh CreateMeshWithText(float radius, string text, DX11Game game)
        {
            var texture = CreateTextureAssetFromText(text, game);

            return CreateMeshWithTexture(radius, texture);
        }

        public static ITexture CreateTextureAssetFromText(string text, DX11Game game)
        {
            var texSize = 1024;
            TWDir.Cache.CreateSubdirectory("BoxText");

            var fontSize = 100; // not really used??

            var name = "BoxText\\" + GetInt64HashCode(text) + "Auto8.dds";
            var file = TWDir.Cache + "\\" + name;

            if (!File.Exists(file))
            {
                var tex = new TextTexture(game, texSize, texSize);

                tex.SetFont("Arial", fontSize);

                var size = tex.MeasureString(text);
                var scale = Math.Min(texSize / size.X, texSize / size.Y);

                tex.SetFont("Arial", fontSize * scale);
                size = tex.MeasureString(text);

                tex.Clear(new Color4(1, 1, 1, 1));

                tex.DrawText(text, new Vector2(texSize / 2 - size.X / 2, texSize / 2 - size.Y / 2), new Color4(0, 0, 0));
                tex.UpdateTexture();


                Resource.SaveTextureToFile(game.Device.ImmediateContext, tex.GPUTexture.Resource, ImageFileFormat.Dds, file);
            }


            //TODO: remove TW dependency
            var texture = TW.Assets.LoadTextureFromCache(name);
            return texture;
        }

        static Int64 GetInt64HashCode(string strText)
        {
            Int64 hashCode = 0;
            if (!string.IsNullOrEmpty(strText))
            {
                //Unicode Encode Covering all characterset
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.SHA256 hash =
                new System.Security.Cryptography.SHA256CryptoServiceProvider();
                byte[] hashText = hash.ComputeHash(byteContents);
                //32Byte hashText separate
                //hashCodeStart = 0~7  8Byte
                //hashCodeMedium = 8~23  8Byte
                //hashCodeEnd = 24~31  8Byte
                //and Fold
                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 8);
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 24);
                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }
            return (hashCode);
        }
    }
}
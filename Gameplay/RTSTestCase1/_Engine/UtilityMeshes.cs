using System.IO;
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
            var builder = new MeshBuilder();
            builder.AddBox(MathHelper.One * -radius, MathHelper.One * radius);
            var mesh = builder.CreateMesh();

            mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = texture;

            var skin = 0.02f;
            mesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box() { Dimensions = new Vector3(radius * 2 + skin, radius * 2 + skin, radius * 2 + skin), Orientation = Matrix.Identity });

            return mesh;
        }

        /// <summary>
        /// TODO: this is somewhat haxor
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="text"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static IMesh CreateMeshWithText(float radius, string text, DX11Game game, int fontSize = 200)
        {

            var texSize = 1024;
            var file = TWDir.Cache.CreateSubdirectory("BoxText") + "\\" + text + fontSize +  ".dds";

            if (!File.Exists(file))
            {
                var tex = new TextTexture(TW.Graphics, texSize, texSize);

                tex.SetFont("Arial", fontSize);
                tex.Clear();
                tex.DrawText(text, new Vector2(0, texSize / 2 - fontSize), new Color4(1, 1, 1));
                tex.UpdateTexture();


                Resource.SaveTextureToFile(game.Device.ImmediateContext, tex.GPUTexture.Resource, ImageFileFormat.Dds, file);
            }


            var texture = new RAMTexture();
            texture.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            texture.GetCoreData().DiskFilePath = file;


            return CreateMeshWithTexture(radius, texture);
        }
    }
}
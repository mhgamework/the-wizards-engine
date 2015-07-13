using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada.TODO
{
    public class ColladaMaterialOud
    {
        public string Name;
        public MaterialType Type;

        public Color Ambient;
        public Color Diffuse;
        public Color Specular;
        public float Shininess;

        public ColladaTextureOud DiffuseTexture;
        public float DiffuseTextureRepeatU = 1;
        public float DiffuseTextureRepeatV = 1;
        public ColladaTextureOud NormalTexture;
        public float NormalTextureRepeatU = 1;
        public float NormalTextureRepeatV = 1;

        public enum MaterialType
        {
            Phong = 1,
            Blinn
        }
    }
}

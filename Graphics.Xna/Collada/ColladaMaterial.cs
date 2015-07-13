using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada
{
    public class ColladaMaterial
    {
        public string Name;
        public MaterialType Type;

        public Color Ambient;
        public Color Diffuse;
        public Color Specular;
        public float Shininess;

        public ColladaTexture DiffuseTexture;
        public float DiffuseTextureRepeatU = 1;
        public float DiffuseTextureRepeatV = 1;
        /// <summary>
        /// Not yet used
        /// </summary>
        public string DiffuseTexcoordSemantic = "";
        public ColladaTexture NormalTexture;
        public float NormalTextureRepeatU = 1;
        public float NormalTextureRepeatV = 1;
        /// <summary>
        /// Not yet used
        /// </summary>
        public string NormalTexcoordSemantic = "";

        public enum MaterialType
        {
            Phong = 1,
            Blinn
        }

        public override string ToString()
        {
            return "ColladaMaterial: " + Name;
        }
    }
}

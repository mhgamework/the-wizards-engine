using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;

namespace TreeGenerator
{
    public class TreeRenderManager
    {
        public ColladaShader Shader;
        public TWTexture Texture;
        public TWTexture BumpMap;



        public void Intialize(IXNAGame game,string textureName)
        {
            Shader = new ColladaShader(game, new Microsoft.Xna.Framework.Graphics.EffectPool());
            Texture = TWTexture.FromImageFile(game, new GameFile(textureName));

            Shader.Technique = ColladaShader.TechniqueType.Textured;
            
            Shader.ViewInverse = Matrix.Identity;
            Shader.ViewProjection = Matrix.Identity;
            Shader.LightDirection = Vector3.Normalize(new Vector3(0.6f, 1f, 0.6f));
            Shader.LightColor = new Vector3(1, 1, 1);
        }
        public void IntializeBumpMapping(IXNAGame game,string textureName, string textureNameBumpMapName)
        {
            Shader = new ColladaShader(game, new Microsoft.Xna.Framework.Graphics.EffectPool());
            Texture = TWTexture.FromImageFile(game, new GameFile(textureName));
            BumpMap = TWTexture.FromImageFile(game, new GameFile(textureNameBumpMapName));
            //Texture = TWTexture.FromTexture2D(null);
            //BumpMap = TWTexture.FromTexture2D(null);

            Shader.Technique = ColladaShader.TechniqueType.TexturedNormalMapping;
            Shader.DiffuseTexture = Texture.XnaTexture;
            Shader.NormalTexture = BumpMap.XnaTexture;

          
            Shader.ViewInverse = Matrix.Identity;
            Shader.ViewProjection = Matrix.Identity;
            Shader.LightDirection = Vector3.Normalize(new Vector3(0.6f, 1f, 0.6f));
            Shader.LightColor = new Vector3(1, 1, 1);
        }
    }
}

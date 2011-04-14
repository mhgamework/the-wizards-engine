using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Entity.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This class initializes on construction
    /// </summary>
    public class DefaultModelShader
    {
        public delegate void RenderPrimitivesDelegate();
        public void DrawPrimitives(RenderPrimitivesDelegate renderPrimitives)
        {
            Shader.effect.Begin(SaveStateMode.None);

            EffectPass pass = Shader.effect.CurrentTechnique.Passes[0];

            pass.Begin();
            renderPrimitives();
            pass.End();

            Shader.effect.End();
        }

        /// <summary>
        /// This constructor also initializes the DirectX components
        /// </summary>
        /// <param name="_game"></param>
        /// <param name="pool"></param>
        public DefaultModelShader(IXNAGame _game, EffectPool pool)
            : this(_game)
        {
            LoadShader(pool);


        }

        private DefaultModelShader(IXNAGame _game)
        {
            game = _game;
        }

        public DefaultModelShader Clone()
        {
            DefaultModelShader clone = new DefaultModelShader(game);
            clone.shader = shader.Clone();

            clone.LoadTechniques();
            clone.LoadParameters();
            return clone;
        }



        public enum TechniqueType
        {
            Colored = 1,
            Textured,
            TexturedNormalMapping,
            ColoredShadows,
            TexturedShadows,
            TexturedNormalMappingShadows,
        }


        private BasicShader shader;
        public BasicShader Shader
        {
            get { return shader; }
            set { shader = value; }
        }
        private IXNAGame game;

        private EffectParameter
            paramViewProjection,
            paramWorld,
            paramViewInverse,
            paramLightDir,
            paramLightColor,
            paramAmbientColor,
            paramDiffuseColor,
            paramSpecularColor,
            paramShininess,
            paramDiffuseTexture,
            paramDiffuseTextureRepeat,
            paramNormalTexture,
            paramNormalTextureRepeat,
            paramShadowOcclusionTexture,
            paramBackbufferSize;

        private EffectTechnique
            techniqueColored,
            techniqueTextured,
            techniqueTexturedNormalMapping,
            techniqueColoredShadows,
            techniqueTexturedShadows,
            techniqueTexturedNormalMappingShadows;



        private TechniqueType technique;

        public TechniqueType Technique
        {
            get { return technique; }
            set { SetTechniqueType(value); }
        }


        // Shared

        private Matrix viewProjection;
        /// <summary>
        /// Shared
        /// </summary>
        public Matrix ViewProjection
        {
            get { return viewProjection; }
            set { viewProjection = value; paramViewProjection.SetValue(value); }
        }
        private Matrix viewInverse;
        /// <summary>
        /// Shared
        /// </summary>
        public Matrix ViewInverse
        {
            get { return viewInverse; }
            set { viewInverse = value; paramViewInverse.SetValue(value); }
        }
        private Vector3 lightDirection;
        /// <summary>
        /// Shared
        /// </summary>
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; paramLightDir.SetValue(value); }
        }
        private Vector3 lightColor;
        /// <summary>
        /// Shared
        /// </summary>
        public Vector3 LightColor
        {
            get { return lightColor; }
            set { lightColor = value; paramLightColor.SetValue(value); }
        }
        private Texture2D shadowOcclusionTexture;
        /// <summary>
        /// Shared
        /// </summary>
        public Texture2D ShadowOcclusionTexture
        {
            get { return shadowOcclusionTexture; }
            set { shadowOcclusionTexture = value; paramShadowOcclusionTexture.SetValue(value); }
        }
        private Vector2 backbufferSize;
        /// <summary>
        /// Shared
        /// </summary>
        public Vector2 BackbufferSize
        {
            get { return backbufferSize; }
            set { backbufferSize = value; paramBackbufferSize.SetValue(value); }
        }

        // Non-shared

        private Matrix world;
        public Matrix World
        {
            get { return world; }
            set { world = value; paramWorld.SetValue(value); }
        }
        private Vector4 ambientColor;
        public Vector4 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; paramAmbientColor.SetValue(value); }
        }
        private Vector4 diffuseColor;
        public Vector4 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; paramDiffuseColor.SetValue(value); }
        }
        private Vector4 specularColor;
        public Vector4 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; paramSpecularColor.SetValue(value); }
        }
        private float shininess;
        public float Shininess
        {
            get { return shininess; }
            set { shininess = value; paramShininess.SetValue(value); }
        }
        private Texture2D diffuseTexture;
        public Texture2D DiffuseTexture
        {
            get { return diffuseTexture; }
            set { diffuseTexture = value; paramDiffuseTexture.SetValue(value); }
        }
        private Texture2D normalTexture;
        public Texture2D NormalTexture
        {
            get { return normalTexture; }
            set { normalTexture = value; paramNormalTexture.SetValue(value); }
        }
        private Vector2 diffuseTextureRepeat;
        public Vector2 DiffuseTextureRepeat
        {
            get { return diffuseTextureRepeat; }
            set { diffuseTextureRepeat = value; paramDiffuseTextureRepeat.SetValue(value); }
        }
        private Vector2 normalTextureRepeat;
        public Vector2 NormalTextureRepeat
        {
            get { return normalTextureRepeat; }
            set { normalTextureRepeat = value; paramNormalTextureRepeat.SetValue(value); }
        }


        private void SetTechniqueType(TechniqueType value)
        {
            technique = value;
            switch (value)
            {
                case TechniqueType.Colored:
                    shader.Effect.CurrentTechnique = techniqueColored;
                    break;
                case TechniqueType.Textured:
                    shader.Effect.CurrentTechnique = techniqueTextured;
                    break;
                case TechniqueType.TexturedNormalMapping:
                    shader.Effect.CurrentTechnique = techniqueTexturedNormalMapping;
                    break;
                case TechniqueType.ColoredShadows:
                    shader.effect.CurrentTechnique = techniqueColoredShadows;
                    break;
                case TechniqueType.TexturedShadows:
                    shader.Effect.CurrentTechnique = techniqueTexturedShadows;
                    break;
                case TechniqueType.TexturedNormalMappingShadows:
                    shader.Effect.CurrentTechnique = techniqueTexturedNormalMappingShadows;
                    break;
            }
        }


        private void LoadShader(EffectPool pool)
        {
            //
            // Load the shader and set the material properties
            //

            if (shader != null) shader.Dispose();

            System.IO.Stream strm = EmbeddedFile.GetStreamFullPath(
                "MHGameWork.TheWizards.Rendering.Files.DefaultModelShader.fx",
                game.EngineFiles.DebugFilesDirectory + "/DefaultModelShader.fx");

            shader = BasicShader.LoadFromFXFile(game, strm, pool);
            //Shader = BasicShader.LoadFromFXFile( game, new GameFile( shaderFilename ), pool );

            LoadParameters();
            LoadTechniques();

            SetTechniqueType(TechniqueType.Colored);

        }

        private void LoadParameters()
        {

            paramViewProjection = shader.effect.Parameters["viewProjection"];
            paramWorld = shader.effect.Parameters["world"];
            paramViewInverse = shader.effect.Parameters["viewInverse"];
            paramLightDir = shader.effect.Parameters["lightDir"];
            paramLightColor = shader.effect.Parameters["lightColor"];
            paramShadowOcclusionTexture = shader.effect.Parameters["ShadowOcclusionTexture"];
            paramAmbientColor = shader.effect.Parameters["ambientColor"];
            paramDiffuseColor = shader.effect.Parameters["diffuseColor"];
            paramSpecularColor = shader.effect.Parameters["specularColor"];
            paramShininess = shader.effect.Parameters["shininess"];
            paramDiffuseTexture = shader.effect.Parameters["diffuseTexture"];
            paramDiffuseTextureRepeat = shader.effect.Parameters["diffuseTextureRepeat"];
            paramNormalTexture = shader.effect.Parameters["normalTexture"];
            paramNormalTextureRepeat = shader.effect.Parameters["normalTextureRepeat"];
            paramBackbufferSize = shader.effect.Parameters["BackbufferSize"];



            paramWorld.SetValue(Matrix.Identity);



        }

        private void LoadTechniques()
        {
            techniqueColored = shader.effect.Techniques["SpecularPerPixelColored"];
            techniqueTextured = shader.effect.Techniques["SpecularPerPixelTextured"];
            techniqueTexturedNormalMapping = shader.effect.Techniques["SpecularPerPixelNormalMapping"];
            techniqueTexturedShadows = shader.effect.Techniques["TexturedShadowed"];
            //techniqueTexturedNormalMappingShadows= shader.effect.Techniques[""];
        }

        /// <summary>
        /// This method is meant for usage with one shader per meshpart, all parameters preset and using shared parameters.
        /// Technique should have just one pass.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="state"></param>
        public void RenderPrimitiveSinglePass(IRenderPrimitives primitives, SaveStateMode state)
        {
            Shader.effect.Begin(state);


            // Render all passes (usually just one)
            //for ( int num = 0; num < Shader.effect.CurrentTechnique.Passes.Count; num++ )
            //{
            //EffectPass pass = Shader.effect.CurrentTechnique.Passes[ num ];
            EffectPass pass = Shader.effect.CurrentTechnique.Passes[0];

            pass.Begin();
            primitives.RenderPrimitives();
            pass.End();
            //}


            // End shader
            Shader.effect.End();
        }




        public void Dispose()
        {
            Shader.Dispose();

            Shader = null;

        }


        public void CommitChanges()
        {
            shader.Effect.CommitChanges();
        }
    }
}

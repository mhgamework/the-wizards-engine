using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.ServerClient.Graphics;
using MHGameWork.TheWizards.Common.Core.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient.Entity.Rendering;

namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// This should be called something else, since this is a general Colored/Textured/Normal renderer. (could add parallax)
    /// </summary>
    public class ColladaShader
    {
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
            set { SetTechniqueType( value ); }
        }


        // Shared

        private Matrix viewProjection;
        /// <summary>
        /// Shared
        /// </summary>
        public Matrix ViewProjection
        {
            get { return viewProjection; }
            set { viewProjection = value; paramViewProjection.SetValue( value ); }
        }
        private Matrix viewInverse;
        /// <summary>
        /// Shared
        /// </summary>
        public Matrix ViewInverse
        {
            get { return viewInverse; }
            set { viewInverse = value; paramViewInverse.SetValue( value ); }
        }
        private Vector3 lightDirection;
        /// <summary>
        /// Shared
        /// </summary>
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; paramLightDir.SetValue( value ); }
        }
        private Vector3 lightColor;
        /// <summary>
        /// Shared
        /// </summary>
        public Vector3 LightColor
        {
            get { return lightColor; }
            set { lightColor = value; paramLightColor.SetValue( value ); }
        }
        private Texture2D shadowOcclusionTexture;
        /// <summary>
        /// Shared
        /// </summary>
        public Texture2D ShadowOcclusionTexture
        {
            get { return shadowOcclusionTexture; }
            set { shadowOcclusionTexture = value; paramShadowOcclusionTexture.SetValue( value ); }
        }
        private Vector2 backbufferSize;
        /// <summary>
        /// Shared
        /// </summary>
        public Vector2 BackbufferSize
        {
            get { return backbufferSize; }
            set { backbufferSize = value; paramBackbufferSize.SetValue( value ); }
        }

        // Non-shared

        private Matrix world;
        public Matrix World
        {
            get { return world; }
            set { world = value; paramWorld.SetValue( value ); }
        }
        private Vector4 ambientColor;
        public Vector4 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; paramAmbientColor.SetValue( value ); }
        }
        private Vector4 diffuseColor;
        public Vector4 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; paramDiffuseColor.SetValue( value ); }
        }
        private Vector4 specularColor;
        public Vector4 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; paramSpecularColor.SetValue( value ); }
        }
        private float shininess;
        public float Shininess
        {
            get { return shininess; }
            set { shininess = value; paramShininess.SetValue( value ); }
        }
        private Texture2D diffuseTexture;
        public Texture2D DiffuseTexture
        {
            get { return diffuseTexture; }
            set { diffuseTexture = value; paramDiffuseTexture.SetValue( value ); }
        }
        private Texture2D normalTexture;
        public Texture2D NormalTexture
        {
            get { return normalTexture; }
            set { normalTexture = value; paramNormalTexture.SetValue( value ); }
        }
        private Vector2 diffuseTextureRepeat;
        public Vector2 DiffuseTextureRepeat
        {
            get { return diffuseTextureRepeat; }
            set { diffuseTextureRepeat = value; paramDiffuseTextureRepeat.SetValue( value ); }
        }
        private Vector2 normalTextureRepeat;
        public Vector2 NormalTextureRepeat
        {
            get { return normalTextureRepeat; }
            set { normalTextureRepeat = value; paramNormalTextureRepeat.SetValue( value ); }
        }


        private void SetTechniqueType( TechniqueType value )
        {
            technique = value;
            switch ( value )
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

        public ColladaShader( IXNAGame _game, EffectPool pool )
            : this( _game )
        {
            LoadShader( pool );


        }

        private ColladaShader( IXNAGame _game )
        {
            game = _game;
        }

        public ColladaShader Clone()
        {
            ColladaShader clone = new ColladaShader( game );
            clone.shader = shader.Clone();

            clone.LoadTechniques();
            clone.LoadParameters();
            return clone;
        }



        /*public static MaterialCollada FromColladaMaterial( ColladaMaterial mat )
        {
            MaterialCollada ret = new MaterialCollada();

            ret.Ambient = mat.Ambient;
            ret.Diffuse = mat.Diffuse;
            ret.Specular = mat.Specular;
            ret.Shininess = mat.Shininess;

            if ( mat.DiffuseTexture != null )
                ret.DiffuseTexture = mat.DiffuseTexture.GetFullFilename();
            ret.DiffuseTextureRepeatU = mat.DiffuseTextureRepeatU;
            ret.DiffuseTextureRepeatV = mat.DiffuseTextureRepeatV;

            if ( mat.NormalTexture != null )
                ret.NormalTexture = mat.NormalTexture.GetFullFilename();
            ret.NormalTextureRepeatU = mat.NormalTextureRepeatU;
            ret.NormalTextureRepeatV = mat.NormalTextureRepeatV;

            return ret;

        }*/


        private void LoadShader( EffectPool pool )
        {
            //
            // Load the shader and set the material properties
            //

            if ( shader != null ) shader.Dispose();
            System.IO.Stream strm =
                File.OpenRead(
                    @"C:\_MHData\1 - Projecten\The Wizards\_Source\Common.Core\Core\Graphics\Files\ColladaModelNew.fx");
            //System.IO.Stream strm = EmbeddedFile.GetStreamFullPath(
            //    "MHGameWork.TheWizards.Graphics.Files.ColladaModelNew.fx",
            //    game.EngineFiles.DebugFilesDirectory + "/ColladaModelNew.fx" );

            shader = BasicShader.LoadFromFXFile( game, strm, pool );
            //Shader = BasicShader.LoadFromFXFile( game, new GameFile( shaderFilename ), pool );

            LoadParameters();
            LoadTechniques();

            SetTechniqueType( TechniqueType.Colored );

        }

        private void LoadParameters()
        {

            paramViewProjection = shader.effect.Parameters[ "viewProjection" ];
            paramWorld = shader.effect.Parameters[ "world" ];
            paramViewInverse = shader.effect.Parameters[ "viewInverse" ];
            paramLightDir = shader.effect.Parameters[ "lightDir" ];
            paramLightColor = shader.effect.Parameters[ "lightColor" ];
            paramShadowOcclusionTexture = shader.effect.Parameters[ "ShadowOcclusionTexture" ];
            paramAmbientColor = shader.effect.Parameters[ "ambientColor" ];
            paramDiffuseColor = shader.effect.Parameters[ "diffuseColor" ];
            paramSpecularColor = shader.effect.Parameters[ "specularColor" ];
            paramShininess = shader.effect.Parameters[ "shininess" ];
            paramDiffuseTexture = shader.effect.Parameters[ "diffuseTexture" ];
            paramDiffuseTextureRepeat = shader.effect.Parameters[ "diffuseTextureRepeat" ];
            paramNormalTexture = shader.effect.Parameters[ "normalTexture" ];
            paramNormalTextureRepeat = shader.effect.Parameters[ "normalTextureRepeat" ];
            paramBackbufferSize = shader.effect.Parameters[ "BackbufferSize" ];




        }

        private void LoadTechniques()
        {
            techniqueColored = shader.effect.Techniques[ "SpecularPerPixelColored" ];
            techniqueTextured = shader.effect.Techniques[ "SpecularPerPixelTextured" ];
            techniqueTexturedNormalMapping = shader.effect.Techniques[ "SpecularPerPixelNormalMapping" ];
            techniqueTexturedShadows= shader.effect.Techniques["TexturedShadowed"];
            //techniqueTexturedNormalMappingShadows= shader.effect.Techniques[""];
        }

        ///// <summary>
        ///// This method is meant for usage with one shader per meshpart, all paramaters preset and using shared parameters.
        ///// Technique should have just one pass.
        ///// </summary>
        ///// <param name="primitive"></param>
        ///// <param name="state"></param>
        //public void RenderPrimitiveSinglePass( IRenderPrimitives primitive, SaveStateMode state )
        //{
        //    Shader.effect.Begin( state );


        //    // Render all passes (usually just one)
        //    //for ( int num = 0; num < Shader.effect.CurrentTechnique.Passes.Count; num++ )
        //    //{
        //    //EffectPass pass = Shader.effect.CurrentTechnique.Passes[ num ];
        //    EffectPass pass = Shader.effect.CurrentTechnique.Passes[ 0 ];

        //    pass.Begin();
        //    primitive.RenderPrimitives();
        //    pass.End();
        //    //}


        //    // End shader
        //    Shader.effect.End();
        //}

        /// <summary>
        /// This method is meant for usage with one shader per meshpart, all parameters preset and using shared parameters.
        /// Technique should have just one pass.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="state"></param>
        public void RenderPrimitiveSinglePass( IRenderPrimitives primitives, SaveStateMode state )
        {
            Shader.effect.Begin( state );


            // Render all passes (usually just one)
            //for ( int num = 0; num < Shader.effect.CurrentTechnique.Passes.Count; num++ )
            //{
            //EffectPass pass = Shader.effect.CurrentTechnique.Passes[ num ];
            EffectPass pass = Shader.effect.CurrentTechnique.Passes[ 0 ];

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



        public static void TestLoadShader()
        {
            TestXNAGame.Start( "ColladaShader.TestLoadShader",
                               delegate
                                   {
                                       ColladaShader shader = new ColladaShader( TestXNAGame.Instance, null );
                                   },
                               delegate
                                   {
                                       Console.WriteLine( "Shader loaded succesfully!" );
                                       TestXNAGame.Instance.Exit();
                                   } );
        }
    }
}
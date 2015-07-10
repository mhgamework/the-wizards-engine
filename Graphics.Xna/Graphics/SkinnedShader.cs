using MHGameWork.TheWizards.Common.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    /// <summary>
    /// This should be called something else, since this is a general Colored/Textured/Normal renderer. (could add parallax)
    /// </summary>
    public class SkinnedShader
    {
        public enum TechniqueType
        {
            Colored = 1,
            //Textured,
            TexturedNormalMapping,
            DisplayWeights,
            //ColoredShadows,
            //TexturedShadows,
            //TexturedNormalMappingShadows,
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
            //paramViewInverse,
            paramCameraPosition,
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
            paramBackbufferSize,
            paramSkinnedMatricesVS20,
            paramSelectedBoneIndex;

        private EffectTechnique
            techniqueColored,
            //techniqueTextured;
            techniqueTexturedNormalMapping,
            techniqueDisplayWeights;
        //techniqueColoredShadows,
        //techniqueTexturedShadows,
        //techniqueTexturedNormalMappingShadows;



        private TechniqueType technique;

        public TechniqueType Technique
        {
            get { return technique; }
            set { SetTechniqueType( value ); }
        }


        // Shared


        public void SetBoneMatrices( Matrix[] matrices )
        {

            Vector4[] values = new Vector4[ matrices.Length * 3 ];
            for ( int i = 0; i < matrices.Length; i++ )
            {
                //if ( !( matrices[ i ].M14 == 0 && matrices[ i ].M24 == 0 && matrices[ i ].M34 == 0 && matrices[ i ].M44 == 1 ) ) throw new Exception();



                // Note: We use the transpose matrix here.
                // This has to be reconstructed in the shader, but this is not
                // slower than directly using matrices and this is the only way
                // we can store 80 matrices with ps2.0.
                values[ i * 3 + 0 ] = new Vector4(
                    matrices[ i ].M11, matrices[ i ].M21, matrices[ i ].M31, matrices[ i ].M41 );
                values[ i * 3 + 1 ] = new Vector4(
                    matrices[ i ].M12, matrices[ i ].M22, matrices[ i ].M32, matrices[ i ].M42 );
                values[ i * 3 + 2 ] = new Vector4(
                    matrices[ i ].M13, matrices[ i ].M23, matrices[ i ].M33, matrices[ i ].M43 );
            } // for
            paramSkinnedMatricesVS20.SetValue( values );
        } // SetBoneMatrices(matrices)

        private Matrix viewProjection;
        public Matrix ViewProjection
        {
            get { return viewProjection; }
            set { viewProjection = value; paramViewProjection.SetValue( value ); }
        }
        //private Matrix viewInverse;
        //public Matrix ViewInverse
        //{
        //    get { return viewInverse; }
        //    set { viewInverse = value; paramViewInverse.SetValue( value ); }
        //}
        private Vector3 cameraPosition;

        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; paramCameraPosition.SetValue( value ); }
        }

        private Vector3 lightDirection;
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; paramLightDir.SetValue( value ); }
        }
        private Vector3 lightColor;
        public Vector3 LightColor
        {
            get { return lightColor; }
            set { lightColor = value; paramLightColor.SetValue( value ); }
        }
        private Texture2D shadowOcclusionTexture;
        public Texture2D ShadowOcclusionTexture
        {
            get { return shadowOcclusionTexture; }
            set { shadowOcclusionTexture = value; paramShadowOcclusionTexture.SetValue( value ); }
        }
        private Vector2 backbufferSize;
        public Vector2 BackbufferSize
        {
            get { return backbufferSize; }
            set { backbufferSize = value; paramBackbufferSize.SetValue( value ); }
        }
        /*private Vector4[] skinnedMatricesVS20;
        public Vector4[] SkinnedMatricesVS20
        {
            get { return skinnedMatricesVS20; }
            set { skinnedMatricesVS20 = value; paramSkinnedMatricesVS20.SetValue( value ); }
        }*/



        // Non-shared

        private Matrix world;
        public Matrix World
        {
            get { return world; }
            set { world = value; paramWorld.SetValue( value ); }
        }

        private float selectedBoneIndex;
        public float SelectedBoneIndex
        {
            get { return selectedBoneIndex; }
            set { selectedBoneIndex = value; paramSelectedBoneIndex.SetValue( value ); }
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
                //case TechniqueType.Textured:
                //    shader.Effect.CurrentTechnique = techniqueTextured;
                //    break;
                case TechniqueType.TexturedNormalMapping:
                    shader.Effect.CurrentTechnique = techniqueTexturedNormalMapping;
                    break;
                case TechniqueType.DisplayWeights:
                    shader.Effect.CurrentTechnique = techniqueDisplayWeights;
                    break;
                //case TechniqueType.ColoredShadows:
                //    shader.effect.CurrentTechnique = techniqueColoredShadows;
                //    break;
                //case TechniqueType.TexturedShadows:
                //    shader.Effect.CurrentTechnique = techniqueTexturedShadows;
                //    break;
                //case TechniqueType.TexturedNormalMappingShadows:
                //    shader.Effect.CurrentTechnique = techniqueTexturedNormalMappingShadows;
                //    break;
            }
        }

        public SkinnedShader( IXNAGame _game, EffectPool pool )
            : this( _game )
        {
            LoadShader( pool );


        }

        private SkinnedShader( IXNAGame _game )
        {
            game = _game;
        }

        public SkinnedShader Clone()
        {
            SkinnedShader clone = new SkinnedShader( game );
            clone.shader = shader.Clone();

            clone.LoadTechniques();
            clone.LoadParameters();
            return clone;
        }

        private void LoadShader( EffectPool pool )
        {
            //
            // Load the shader and set the material properties
            //

            if ( shader != null ) shader.Dispose();

            System.IO.Stream strm = EmbeddedFile.GetStreamFullPath(
                "MHGameWork.TheWizards.Graphics.Files.SkinnedNormalMapping.fx",
                game.EngineFiles.DebugFilesDirectory + "/SkinnedNormalMapping.fx" );

            shader = BasicShader.LoadFromFXFile( game, strm, pool );
            //Shader = BasicShader.LoadFromFXFile( game, new GameFile( shaderFilename ), pool );

            LoadParameters();
            LoadTechniques();

            SetTechniqueType( TechniqueType.Colored );

        }

        private void LoadParameters()
        {

            paramViewProjection = shader.effect.Parameters[ "viewProj" ];
            paramWorld = shader.effect.Parameters[ "world" ];
            //paramViewInverse = shader.effect.Parameters[ "viewInverse" ];
            paramCameraPosition = shader.effect.Parameters[ "cameraPos" ];
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
            paramSkinnedMatricesVS20 = shader.effect.Parameters[ "skinnedMatricesVS20" ];
            paramSelectedBoneIndex = shader.effect.Parameters[ "SelectedBoneIndex" ];



        }

        private void LoadTechniques()
        {
            techniqueColored = shader.effect.Techniques[ "DiffuseSpecularColored20" ];
            //techniqueTextured = shader.effect.Techniques[ "DiffuseSpecular" ];
            techniqueTexturedNormalMapping = shader.effect.Techniques[ "DiffuseSpecular20" ];
            techniqueDisplayWeights = shader.effect.Techniques[ "DisplayWeights" ];
            //techniqueTexturedShadows= shader.effect.Techniques[""];
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
        public void RenderPrimitiveSinglePass( Primitives primitives, SaveStateMode state )
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



        //public static void TestLoadShader()
        //{
        //    TestXNAGame.Start( "ColladaShader.TestLoadShader",
        //    delegate
        //    {
        //        ColladaShader shader = new ColladaShader( TestXNAGame.Instance, null );
        //    },
        //    delegate
        //    {
        //        Console.WriteLine( "Shader loaded succesfully!" );
        //        TestXNAGame.Instance.Exit();
        //    } );
        //}
    }
}

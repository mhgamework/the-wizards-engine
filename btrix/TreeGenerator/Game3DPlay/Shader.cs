using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    public class BasicShader
    {
        private IXNAGame game;

        public IXNAGame Game
        {
            get { return game; }
        }

        public Effect effect;

        private Matrix world;
        private Matrix viewProjection;
        private Matrix viewInverse;
        private Matrix worldViewProjection;

        private EffectParameter worldParam;
        private EffectParameter viewProjectionParam;
        private EffectParameter viewInverseParam;
        private EffectParameter worldViewProjectionParam;

        private string currentTechnique;


        private BasicShader( IXNAGame nGame )
        {
            game = nGame;
        }

        /// <summary>
        /// Load an effect from an hlsl .fx file
        /// </summary>
        public static BasicShader LoadFromFXFile( IXNAGame game, IGameFile file )
        {
            BasicShader shader = new BasicShader( game );

            //obs?
            // Dispose old shader
            if ( shader.effect != null )
            {
                shader.effect.Dispose();
                shader.effect = null;
            }
            CompiledEffect compiledEffect;
            try
            {
                compiledEffect = Effect.CompileEffectFromFile( file.GetFullFilename(), null, null, CompilerOptions.None, TargetPlatform.Windows );

                shader.effect = new Effect( game.GraphicsDevice, compiledEffect.GetEffectCode(), CompilerOptions.None, null );
            } // try
            catch ( Exception ex )
            {
                /*Log.Write( "Failed to load shader " + shaderContentName + ". " +
                    "Error: " + ex.ToString() );*/
                // Rethrow error, app can't continue!
                throw ex;
            }
            



            shader.LoadParameters();

            return shader;
        } // Load()

        public delegate void RenderDelegate();

        /// <summary>
        /// Render using this shader.
        /// </summary>
        /// <param name="setMat">Set matrix</param>
        /// <param name="techniqueName">Technique name</param>
        /// <param name="renderDelegate">Render delegate</param>
        public void RenderMultipass( RenderDelegate renderDelegate )
        {
            //SetCameraParameters( game.Camera );
            // Start shader
            //effect.CurrentTechnique = effect.Techniques[ techniqueName ];
            effect.Begin( SaveStateMode.None );

            // Render all passes (usually just one)
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            for ( int num = 0; num < effect.CurrentTechnique.Passes.Count; num++ )
            {
                EffectPass pass = effect.CurrentTechnique.Passes[ num ];

                pass.Begin();
                renderDelegate();
                pass.End();
            } // foreach (pass)

            // End shader
            effect.End();
        } // Render(passName, renderDelegate)

        public void SetTechnique( string techniqueName )
        {
            if ( currentTechnique == techniqueName ) return;
            EffectTechnique technique = GetTechnique( techniqueName );
            if ( technique == null ) throw new InvalidOperationException( "Given technique does not exists!" );
            effect.CurrentTechnique = technique;
            currentTechnique = techniqueName;
        }


        /// <summary>
        /// Returns null when technique doesn't exist.
        /// </summary>
        /// <param name="techniqueName"></param>
        private EffectTechnique GetTechnique( string techniqueName )
        {
            EffectTechnique technique = effect.Techniques[ techniqueName ];
            return technique;
        }

        public void SetParameter( string parameterName, Matrix value )
        {
            EffectParameter parameter = GetParameter( parameterName );
            if ( parameter == null ) throw new InvalidOperationException( "Given parameter does not exist!" );
            parameter.SetValue( value );
        }
        public void SetParameter( string parameterName, Vector3 value )
        {
            EffectParameter parameter = GetParameter( parameterName );
            if ( parameter == null ) throw new InvalidOperationException( "Given parameter does not exist!" );
            parameter.SetValue( value );
        }
        public void SetParameter( string parameterName, TWTexture value )
        {
            EffectParameter parameter = GetParameter( parameterName );
            if ( parameter == null ) throw new InvalidOperationException( "Given parameter does not exist!" );
            parameter.SetValue( value.XnaTexture );
        }
        public void SetParameter( string parameterName, float value )
        {
            EffectParameter parameter = GetParameter( parameterName );
            if ( parameter == null ) throw new InvalidOperationException( "Given parameter does not exist!" );
            parameter.SetValue( value );
        }
        public void SetParameter( string parameterName, Color value )
        {
            EffectParameter parameter = GetParameter( parameterName );
            if ( parameter == null ) throw new InvalidOperationException( "Given parameter does not exist!" );
            parameter.SetValue( value.ToVector4() );
        }
        public void SetParameter( string parameterName, Vector4 value )
        {
            EffectParameter parameter = GetParameter( parameterName );
            if ( parameter == null ) throw new InvalidOperationException( "Given parameter does not exist!" );
            parameter.SetValue( value );
        }




        public void SaveToXML( TWXmlNode node )
        {

            
        }

        public void LoadFromXML( TWXmlNode node )
        {
        }



















        /// <summary>
        /// Returns null when technique doesn't exist.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private EffectParameter GetParameter( string parameterName )
        {
            EffectParameter parameter = effect.Parameters[ parameterName ];

            return parameter;
        }


        //private void SetCameraParameters( ICamera cam )
        //{
        //    ViewProjection = cam.ViewProjection;
        //    ViewInverse = cam.ViewInverse;


        //}

        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="setMatrix">Set matrix</param>
        private void SetValue( EffectParameter param,
            ref Matrix lastUsedMatrix, Matrix newMatrix )
        {
            if ( param == null ) return;
            /*obs, always update, matrices change every frame anyway!
             * matrix compare takes too long, it eats up almost 50% of this method.
            if (param != null &&
                lastUsedMatrix != newMatrix)
             */
            {
                lastUsedMatrix = newMatrix;
                param.SetValue( newMatrix );
            } // if (param)
        } // SetValue(param, setMatrix)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedVector">Last used vector</param>
        /// <param name="newVector">New vector</param>
        private void SetValue( EffectParameter param,
            ref Vector3 lastUsedVector, Vector3 newVector )
        {
            if ( param != null &&
                lastUsedVector != newVector )
            {
                lastUsedVector = newVector;
                param.SetValue( newVector );
            } // if (param)
        } // SetValue(param, lastUsedVector, newVector)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedColor">Last used color</param>
        /// <param name="newColor">New color</param>
        private void SetValue( EffectParameter param,
            ref Color lastUsedColor, Color newColor )
        {
            // Note: This check eats few % of the performance, but the color
            // often stays the change (around 50%).
            if ( param != null &&
                //slower: lastUsedColor != newColor)
                lastUsedColor.PackedValue != newColor.PackedValue )
            {
                lastUsedColor = newColor;
                //obs: param.SetValue(ColorHelper.ConvertColorToVector4(newColor));
                param.SetValue( newColor.ToVector4() );
            } // if (param)
        } // SetValue(param, lastUsedColor, newColor)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedValue">Last used value</param>
        /// <param name="newValue">New value</param>
        private void SetValue( EffectParameter param,
            ref float lastUsedValue, float newValue )
        {
            if ( param != null &&
                lastUsedValue != newValue )
            {
                lastUsedValue = newValue;
                param.SetValue( newValue );
            } // if (param)
        } // SetValue(param, lastUsedValue, newValue)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedValue">Last used value</param>
        /// <param name="newValue">New value</param>
        private void SetValue( EffectParameter param,
            ref TWTexture lastUsedValue, TWTexture newValue )
        {
            if ( param != null &&
                lastUsedValue != newValue )
            {
                lastUsedValue = newValue;
                param.SetValue( newValue.XnaTexture );
            } // if (param)
        } // SetValue(param, lastUsedValue, newValue)


        protected virtual void LoadParameters()
        {
            if ( effect == null ) throw new InvalidOperationException( "Impossible! No effect has been loaded." );

            worldParam = effect.Parameters[ "world" ];
            viewProjectionParam = effect.Parameters[ "viewProjection" ];
            viewInverseParam = effect.Parameters[ "viewInverse" ];
            worldViewProjectionParam = effect.Parameters[ "worldViewProjection" ];


        }




        public Matrix World
        {
            get
            { return world; }
            set
            { SetValue( worldParam, ref world, value ); }
        }

        public Matrix ViewProjection
        {
            get
            { return viewProjection; }
            set
            { SetValue( viewProjectionParam, ref viewProjection, value ); }
        }

        public Matrix ViewInverse
        {
            get
            { return viewInverse; }
            set
            { SetValue( viewInverseParam, ref viewInverse, value ); }
        }

        public Matrix WorldViewProjection
        {
            get
            { return worldViewProjection; }
            set
            { SetValue( worldViewProjectionParam, ref worldViewProjection, value ); }
        }

    }
}


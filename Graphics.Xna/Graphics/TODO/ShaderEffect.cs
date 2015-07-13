using System;
using MHGameWork.TheWizards.Graphics.Xna.Collada.TODO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO
{
    public class ShaderEffect
    {

        private IXNAGame game;


        /// <summary>
        /// Content name for this shader
        /// </summary>
        private string shaderContentName = "";

        /// <summary>
        /// Effect
        /// </summary>
        protected Effect effect = null;
        /// <summary>
        /// Effect handles for shaders.
        /// </summary>
        protected EffectParameter worldViewProj,
            viewProj,
            world,
            viewInverse,
            lightDir,
            ambientColor,
            diffuseColor,
            specularColor,
            specularPower,
            diffuseTexture,
            normalTexture,
            diffuseTextureRepeatU,
            diffuseTextureRepeatV,
            normalTextureRepeatU,
            normalTextureRepeatV;

        /// <summary>
        /// Is this shader valid to render? If not we can't perform any rendering.
        /// </summary>
        /// <returns>Bool</returns>
        public bool Valid
        {
            get
            {
                return effect != null;
            } // get
        } // Valid

        /// <summary>
        /// Effect
        /// </summary>
        /// <returns>Effect</returns>
        public Effect Effect
        {
            get
            {
                return effect;
            } // get
        } // Effect

        /// <summary>
        /// Number of techniques
        /// </summary>
        /// <returns>Int</returns>
        public int NumberOfTechniques
        {
            get
            {
                return effect.Techniques.Count;
            } // get
        } // NumberOfTechniques

        /// <summary>
        /// Get technique
        /// </summary>
        /// <param name="techniqueName">Technique name</param>
        /// <returns>Effect technique</returns>
        public EffectTechnique GetTechnique( string techniqueName )
        {
            return effect.Techniques[ techniqueName ];
        } // GetTechnique(techniqueName)

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
            ref Texture2D lastUsedValue, Texture2D newValue )
        {
            if ( param != null &&
                lastUsedValue != newValue )
            {
                lastUsedValue = newValue;
                param.SetValue( newValue );
            } // if (param)
        } // SetValue(param, lastUsedValue, newValue)

        protected Matrix lastUsedWorldViewProjMatrix = Matrix.Identity;
        /// <summary>
        /// Set world view proj matrix
        /// </summary>
        public Matrix WorldViewProjMatrix
        {
            set
            {
                SetValue( worldViewProj, ref lastUsedWorldViewProjMatrix, value );
            } // set
        } // WorldViewProjMatrix


        protected Matrix lastUsedViewProjMatrix = Matrix.Identity;
        /// <summary>
        /// Set view proj matrix
        /// </summary>
        protected Matrix ViewProjMatrix
        {
            set
            {
                SetValue( viewProj, ref lastUsedViewProjMatrix, value );
            } // set
        } // ViewProjMatrix

        //obs: protected Matrix lastUsedWorldMatrix = Matrix.Identity;
        /// <summary>
        /// Set world matrix
        /// </summary>
        public Matrix WorldMatrix
        {
            set
            {
                // This is the most used property here.
                //obs: SetValue(world, ref lastUsedWorldMatrix, value);
                /*obs, world matrix ALWAYS changes! and it is always used!
                if (world != null &&
                    lastUsedWorldMatrix != value)
                {
                    lastUsedWorldMatrix = value;
                    world.SetValue(lastUsedWorldMatrix);
                } // if (world)
                 */

                // Faster, we checked world matrix in constructor.
                world.SetValue( value );
            } // set
        } // WorldMatrix

        protected Matrix lastUsedInverseViewMatrix = Matrix.Identity;
        /// <summary>
        /// Set view inverse matrix
        /// </summary>
        protected Matrix InverseViewMatrix
        {
            set
            {
                SetValue( viewInverse, ref lastUsedInverseViewMatrix, value );
            } // set
        } // InverseViewMatrix

        protected Vector3 lastUsedLightDir = Vector3.Zero;
        /// <summary>
        /// Set light direction
        /// </summary>
        protected Vector3 LightDir
        {
            set
            {
                // Make sure lightDir is normalized (fx files are optimized
                // to work with a normalized lightDir vector)
                value.Normalize();
                // Set negative value, shader is optimized not to negate dir!
                SetValue( lightDir, ref lastUsedLightDir, -value );
            } // set
        } // LightDir

        protected Color lastUsedAmbientColor = ColorHelper.Empty;
        /// <summary>
        /// Ambient color
        /// </summary>
        public Color AmbientColor
        {
            set
            {
                SetValue( ambientColor, ref lastUsedAmbientColor, value );
            } // set
        } // AmbientColor

        protected Color lastUsedDiffuseColor = ColorHelper.Empty;
        /// <summary>
        /// Diffuse color
        /// </summary>
        public Color DiffuseColor
        {
            set
            {
                SetValue( diffuseColor, ref lastUsedDiffuseColor, value );
            } // set
        } // DiffuseColor

        protected Color lastUsedSpecularColor = ColorHelper.Empty;
        /// <summary>
        /// Specular color
        /// </summary>
        public Color SpecularColor
        {
            set
            {
                SetValue( specularColor, ref lastUsedSpecularColor, value );
            } // set
        } // SpecularColor

        private float lastUsedSpecularPower = 0;
        /// <summary>
        /// SpecularPower for specular color
        /// </summary>
        public float SpecularPower
        {
            set
            {
                SetValue( specularPower, ref lastUsedSpecularPower, value );
            } // set
        } // SpecularPower

        protected Texture2D lastUsedDiffuseTexture = null;
        /// <summary>
        /// Set diffuse texture
        /// </summary>
        public TextureBookengine DiffuseTexture
        {
            set
            {
                SetValue( diffuseTexture, ref lastUsedDiffuseTexture,
                    value != null ? value.XnaTexture : null );
            } // set
        } // DiffuseTexture

        public Texture2D DiffuseTextureMHGW
        {
            set
            {
                SetValue( diffuseTexture, ref lastUsedDiffuseTexture,
                    value != null ? value : null );
            } // set
        } // DiffuseTexture

        protected Texture2D lastUsedNormalTexture = null;
        /// <summary>
        /// Set normal texture for normal mapping
        /// </summary>
        public TextureBookengine NormalTexture
        {
            set
            {
                SetValue( normalTexture, ref lastUsedNormalTexture,
                    value != null ? value.XnaTexture : null );
            } // set
        } // NormalTexture


        public Texture2D NormalTextureMHGW
        {
            set
            {
                SetValue( normalTexture, ref lastUsedNormalTexture,
                    value != null ? value : null );
            } // set
        } // DiffuseTexture

        public ShaderEffect( IXNAGame _game, string shaderName )
        {
            game = _game;
            //if ( BaseGame.Device == null )
            //    throw new NullReferenceException(
            //        "XNA device is not initialized, can't create ShaderEffect." );

            //shaderContentName = StringHelper.ExtractFilename( shaderName, true );
            shaderContentName = shaderName;

            //TODO: engine.ShaderManager.AddShader( this );

            //Load();

            //BaseGame.RegisterGraphicContentObject( this );
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            //TODO: engine.ShaderManager.RemoveShader( this );
            // Dispose shader effect
            if ( effect != null )
                effect.Dispose();
            effect = null;
        } // Dispose()


        /// <summary>
        /// Reload effect (can be useful if we change the fx file dynamically).
        /// </summary>
        public virtual void Load( Microsoft.Xna.Framework.Content.ContentManager content )
        {
            //obs?
            // Dispose old shader
            if ( effect != null )
            {
                effect.Dispose();
                effect = null;
            }


            // Load shader
            try
            {
                // We have to try, there is no "Exists" method.
                // We could try to check the xnb filename, but why bother? ^^
                effect = content.Load<Effect>( shaderContentName );
                //Path.Combine( Directories.ContentDirectory, shaderContentName ) );
            } // try

            catch
            {
                if ( System.IO.File.Exists( shaderContentName ) == false ) throw new System.IO.FileNotFoundException( "Shader file '" + shaderContentName + "' not found!" );
                // Try again by loading by filename (only allowed for windows!)
                // Content file was most likely removed for easier testing :)
                try
                {
                    CompiledEffect compiledEffect = Effect.CompileEffectFromFile(
                        shaderContentName,
                        //Path.Combine( "Shaders", shaderContentName + ".fx" ),
                        null, null, CompilerOptions.None,
                        TargetPlatform.Windows );

                    effect = new Effect( game.GraphicsDevice,
                        compiledEffect.GetEffectCode(), CompilerOptions.None, null );
                } // try
                catch ( Exception ex )
                {
                    /*Log.Write( "Failed to load shader " + shaderContentName + ". " +
                        "Error: " + ex.ToString() );*/
                    // Rethrow error, app can't continue!
                    throw ex;
                } // catch
            } // catch


            GetParameters();
        } // Load()


        /// <summary>
        /// Get parameters, override to support more
        /// </summary>
        protected virtual void GetParameters()
        {
            worldViewProj = effect.Parameters[ "worldViewProj" ];
            viewProj = effect.Parameters[ "viewProj" ];
            world = effect.Parameters[ "world" ];
            viewInverse = effect.Parameters[ "viewInverse" ];
            lightDir = effect.Parameters[ "lightDir" ];
            ambientColor = effect.Parameters[ "ambientColor" ];
            diffuseColor = effect.Parameters[ "diffuseColor" ];
            specularColor = effect.Parameters[ "specularColor" ];
            specularPower = effect.Parameters[ "shininess" ];
            diffuseTexture = effect.Parameters[ "diffuseTexture" ];
            normalTexture = effect.Parameters[ "normalTexture" ];

            diffuseTextureRepeatU = effect.Parameters[ "diffuseTextureRepeatU" ];
            diffuseTextureRepeatV = effect.Parameters[ "diffuseTextureRepeatV" ];

            normalTextureRepeatU = effect.Parameters[ "normalTextureRepeatU" ];
            normalTextureRepeatV = effect.Parameters[ "normalTextureRepeatV" ];

        } // GetParameters()

        /// <summary>
        /// Set parameters, this overload sets all material parameters too.
        /// </summary>
        public virtual void SetParametersCollada( ColladaMaterialOud setMat )
        {
            //TODO: world matrix not correctly implemented
            //TODO: lightdir


            if ( worldViewProj != null )
                //TODO: worldViewProj.SetValue( game.Camera.ViewProjection * game.Camera.WorldMatrix );
                worldViewProj.SetValue( game.Camera.ViewProjection );
            if ( viewProj != null )
                viewProj.SetValue( game.Camera.ViewProjection );
            if ( world != null )
                //TODO: world.SetValue( game.Camera.WorldMatrix );
                world.SetValue( Matrix.Identity );
            if ( viewInverse != null )
                viewInverse.SetValue( game.Camera.ViewInverse );
            if ( lightDir != null )
                lightDir.SetValue( Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) ) );
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BaseGame.LightDirection );

            // Set all material properties
            if ( setMat != null )
            {
                AmbientColor = new Color( new Vector4( 0.25f, 0.25f, 0.25f, 1f ) );
                //AmbientColor = setMat.Ambient;
                DiffuseColor = setMat.Diffuse;
                SpecularColor = setMat.Specular;
                //SpecularColor = new Color( new Vector4( 0.5f, 0.5f, 0.70f, 1f ) );

                SpecularPower = 80;
                //SpecularPower = setMat.Shininess;
                if ( setMat.DiffuseTexture != null )
                {
                    float temp;

                    DiffuseTextureMHGW = setMat.DiffuseTexture.Texture;
                    temp = 0;
                    SetValue( diffuseTextureRepeatU, ref temp, setMat.DiffuseTextureRepeatU );
                    temp = 0;
                    SetValue( diffuseTextureRepeatV, ref temp, setMat.DiffuseTextureRepeatV );
                }
                if ( setMat.NormalTexture != null )
                {
                    float temp;

                    NormalTextureMHGW = setMat.NormalTexture.Texture;
                    temp = 0;
                    SetValue( normalTextureRepeatU, ref temp, setMat.NormalTextureRepeatU );
                    temp = 0;
                    SetValue( normalTextureRepeatV, ref temp, setMat.NormalTextureRepeatV );
                }
                //NormalTexture = setMat.normalTexture;
            } // if (setMat)
        } // SetParameters()

        /// <summary>
        /// Set parameters, this overload sets all material parameters too.
        /// </summary>
        public virtual void SetParameters( Material setMat )
        {
            //TODO: world matrix not correctly implemented
            //TODO: lightdir


            if ( worldViewProj != null )
                //TODO: worldViewProj.SetValue( game.Camera.ViewProjection * game.Camera.WorldMatrix );
                worldViewProj.SetValue( game.Camera.ViewProjection );
            if ( viewProj != null )
                viewProj.SetValue( game.Camera.ViewProjection );
            if ( world != null )
                //TODO: world.SetValue( game.Camera.WorldMatrix );
                world.SetValue( Matrix.Identity );
            if ( viewInverse != null )
                viewInverse.SetValue( game.Camera.ViewInverse );
            if ( lightDir != null )
                lightDir.SetValue( Vector3.Normalize( new Vector3( -1, -1, -1 ) ) );
            //lightDir.SetValue( BaseGame.LightDirection );

            // Set all material properties
            if ( setMat != null )
            {
                AmbientColor = setMat.ambientColor;
                DiffuseColor = setMat.diffuseColor;
                SpecularColor = setMat.specularColor;
                SpecularPower = setMat.specularPower;
                DiffuseTexture = setMat.diffuseTexture;
                NormalTexture = setMat.normalTexture;
            } // if (setMat)
        } // SetParameters()

        /// <summary>
        /// Set parameters, override to set more
        /// </summary>
        public virtual void SetParameters()
        {
            SetParameters( null );
        } // SetParameters()



        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            effect.CommitChanges();
        } // Update()


        #region Render

        public delegate void RenderDelegate();

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="setMat">Set matrix</param>
        /// <param name="techniqueName">Technique name</param>
        /// <param name="renderDelegate">Render delegate</param>
        public void RenderCollada( string techniqueName,
            RenderDelegate renderDelegate )
        {
            /*will become important later in the book.
        // Can we do the requested technique?
        // For graphic cards not supporting ps2.0, fall back to ps1.1
        if (BaseGame.CanUsePS20 == false &&
            techniqueName.EndsWith("20"))
            // Use same technique without the 20 ending!
            techniqueName = techniqueName.Substring(0, techniqueName.Length - 2);
         */

            // Start shader
            effect.CurrentTechnique = effect.Techniques[ techniqueName ];
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

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="setMat">Set matrix</param>
        /// <param name="techniqueName">Technique name</param>
        /// <param name="renderDelegate">Render delegate</param>
        public void Render( Material setMat,
            string techniqueName,
            RenderDelegate renderDelegate )
        {
            SetParameters( setMat );

            /*will become important later in the book.
            // Can we do the requested technique?
            // For graphic cards not supporting ps2.0, fall back to ps1.1
            if (BaseGame.CanUsePS20 == false &&
                techniqueName.EndsWith("20"))
                // Use same technique without the 20 ending!
                techniqueName = techniqueName.Substring(0, techniqueName.Length - 2);
             */

            // Start shader
            effect.CurrentTechnique = effect.Techniques[ techniqueName ];
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

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="techniqueName">Technique name</param>
        /// <param name="renderDelegate">Render delegate</param>
        public void Render( string techniqueName,
            RenderDelegate renderDelegate )
        {
            Render( null, techniqueName, renderDelegate );
        } // Render(techniqueName, renderDelegate)
        #endregion

        #region Render single pass shader
        /// <summary>
        /// Render single pass shader, little faster and simpler than
        /// Render and it just uses the current technique and renderes only
        /// the first pass (most shaders have only 1 pass anyway).
        /// Used for MeshRenderManager!
        /// </summary>
        /// <param name="renderDelegate">Render delegate</param>
        public void RenderSinglePassShader(
            RenderDelegate renderDelegate )
        {
            // Start effect (current technique should be set)
            effect.Begin( SaveStateMode.None );
            // Start first pass
            effect.CurrentTechnique.Passes[ 0 ].Begin();

            // Render
            renderDelegate();

            // End pass and shader
            effect.CurrentTechnique.Passes[ 0 ].End();
            effect.End();
        } // RenderSinglePassShader(renderDelegate)
        #endregion
    }

}

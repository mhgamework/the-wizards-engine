#region Using directives
#if DEBUG
//using NUnit.Framework;
#endif
using System;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO
{
	/// <summary>
	/// Material class for DirectX materials used for Models. Consists of
	/// normal DirectX material settings (ambient, diffuse, specular),
	/// the diffuse texture and optionally of normal map, height map and shader
	/// parameters.
	/// </summary>
	public class Material : IDisposable
	{
		#region Constants
		/// <summary>
		/// Default color values are:
		/// 0.15f for ambient and 1.0f for diffuse and 1.0f specular.
		/// </summary>
		public static readonly Color
			DefaultAmbientColor = new Color( 40, 40, 40 ),
			DefaultDiffuseColor = new Color( 210, 210, 210 ),
			DefaultSpecularColor = new Color( 255, 255, 255 );

		/// <summary>
		/// Default specular power (24)
		/// </summary>
		const float DefaultSpecularPower = 24.0f;

		/// <summary>
		/// Parallax amount for parallax and offset shaders.
		/// </summary>
		public const float DefaultParallaxAmount = 0.04f;//0.07f;
		#endregion

		#region Variables
		/// <summary>
		/// Colors
		/// </summary>
		public Color diffuseColor = DefaultDiffuseColor,
			ambientColor = DefaultAmbientColor,
			specularColor = DefaultSpecularColor;

		/// <summary>
		/// Specular power
		/// </summary>
		public float specularPower = DefaultSpecularPower;

		/// <summary>
		/// Diffuse texture for the material. Can be null for unused.
		/// </summary>
		public TextureBookengine diffuseTexture = null;
		/// <summary>
		/// Normal texture in case we use normal mapping. Can be null for unused.
		/// </summary>
		public TextureBookengine normalTexture = null;
		/// <summary>
		/// Height texture in case we use parallax mapping. Can be null for unused.
		/// </summary>
		public TextureBookengine heightTexture = null;
		/// <summary>
		/// Detail texture, used for landscape rendering. Can be null for unused.
		/// </summary>
		public TextureBookengine detailTexture = null;
		/// <summary>
		/// Parallax amount for parallax and offset shaders.
		/// </summary>
		public float parallaxAmount = DefaultParallaxAmount;
		#endregion

		#region Properties
		/// <summary>
		/// Checks if the diffuse texture has alpha
		/// </summary>
		public bool HasAlpha
		{
			get
			{
				if ( diffuseTexture != null )
					return diffuseTexture.HasAlphaPixels;
				else
					return false;
			} // get
		} // HasAlpha
		#endregion

		#region Constructors
		#region Default Constructors
		/// <summary>
		/// Create material, just using default values.
		/// </summary>
		public Material()
		{
		} // Material()

		/// <summary>
		/// Create material, just using default color values.
		/// </summary>
		public Material(IXNAGame _game, string setDiffuseTexture)
		{
			diffuseTexture = new TextureBookengine(_game, setDiffuseTexture );
		} // Material(setDiffuseTexture)

		/// <summary>
		/// Create material
		/// </summary>
		public Material(IXNAGame _game, Color setAmbientColor, Color setDiffuseColor,
			string setDiffuseTexture)
		{
			ambientColor = setAmbientColor;
			diffuseColor = setDiffuseColor;
			diffuseTexture = new TextureBookengine( _game, setDiffuseTexture );
			// Leave rest to default
		} // Material(ambientColor, diffuseColor, setDiffuseTexture)

		/// <summary>
		/// Create material
		/// </summary>
		public Material(Color setAmbientColor, Color setDiffuseColor,
			TextureBookengine setDiffuseTexture)
		{
			ambientColor = setAmbientColor;
			diffuseColor = setDiffuseColor;
			diffuseTexture = setDiffuseTexture;
			// Leave rest to default
		} // Material(ambientColor, diffuseColor, setDiffuseTexture)

		/// <summary>
		/// Create material
		/// </summary>
		public Material(IXNAGame _game, string setDiffuseTexture, string setNormalTexture)
		{
			diffuseTexture = new TextureBookengine( _game, setDiffuseTexture );
			normalTexture = new TextureBookengine( _game, setNormalTexture );
			// Leave rest to default
		} // Material(ambientColor, diffuseColor, setDiffuseTexture)

		/// <summary>
		/// Create material
		/// </summary>
		public Material(IXNAGame _game, string setDiffuseTexture, string setNormalTexture,
			string setHeightTexture)
		{
			diffuseTexture = new TextureBookengine( _game, setDiffuseTexture );
			normalTexture = new TextureBookengine( _game, setNormalTexture );
			heightTexture = new TextureBookengine( _game, setHeightTexture );
			// Leave rest to default
		} // Material(ambientColor, diffuseColor, setDiffuseTexture)

		/// <summary>
		/// Create material
		/// </summary>
		public Material(IXNAGame _game, Color setAmbientColor, Color setDiffuseColor,
			Color setSpecularColor, string setDiffuseTexture,
			string setNormalTexture, string setHeightTexture,
			string setDetailTexture)
		{
			ambientColor = setAmbientColor;
			diffuseColor = setDiffuseColor;
			specularColor = setSpecularColor;
			diffuseTexture = new TextureBookengine( _game, setDiffuseTexture );
			if ( String.IsNullOrEmpty( setNormalTexture ) == false )
				normalTexture = new TextureBookengine( _game, setNormalTexture );
			if ( String.IsNullOrEmpty( setHeightTexture ) == false )
				heightTexture = new TextureBookengine( _game, setHeightTexture );
			if ( String.IsNullOrEmpty( setDetailTexture ) == false )
				detailTexture = new TextureBookengine( _game, setDetailTexture );
			// Leave rest to default
		} // Material(ambientColor, diffuseColor, setDiffuseTexture)
		#endregion

		#region Helpers for creating material from shader parameters
		/*TODO
		/// <summary>
		/// Search effect parameter
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <param name="paramName">Param name</param>
		/// <returns>Object</returns>
		private static object SearchEffectParameter(
			EffectDefault[] parameters, string paramName)
		{
			foreach (EffectDefault param in parameters)
			{
				if (StringHelper.Compare(param.ParameterName, paramName))
				{
					return param.Data;
				} // if (StringHelper.Compare)
			} // foreach (param in parameters)
			// Not found
			return null;
		} // SearchEffectParameter(parameters, paramName)

		/// <summary>
		/// Search effect float parameter
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <param name="paramName">Param name</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns>Float</returns>
		private static float SearchEffectFloatParameter(
			EffectDefault[] parameters, string paramName, float defaultValue)
		{
			object ret = SearchEffectParameter(parameters, paramName);
			if (ret != null &&
				ret.GetType() == typeof(float))
				return (float)ret;
			// Not found? Then just return default value.
			return defaultValue;
		} // SearchEffectFloatParameter(parameters, paramName, defaultValue)

		/// <summary>
		/// Search effect color parameter
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <param name="paramName">Param name</param>
		/// <param name="defaultColor">Default color</param>
		/// <returns>Color</returns>
		private static Color SearchEffectColorParameter(
			EffectDefault[] parameters, string paramName, Color defaultColor)
		{
			object ret = SearchEffectParameter(parameters, paramName);
			if (ret != null &&
				ret.GetType() == typeof(float[]))
			{
				float[] data = (float[])ret;
				if (data.Length >= 4)
				{
					byte red = (byte)(data[0] * 255.0f);
					byte green = (byte)(data[1] * 255.0f);
					byte blue = (byte)(data[2] * 255.0f);
					byte alpha = (byte)(data[3] * 255.0f);
					return Color.FromArgb(alpha, red, green, blue);
				} // if (data.Length)
			} // if (ret)
			// Not found? Then just return default value.
			return defaultColor;
		} // SearchEffectColorParameter(parameters, paramName, defaultColor)

		/// <summary>
		/// Search effect texture parameter
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <param name="paramName">Param name</param>
		/// <param name="defaultTexture">Default texture</param>
		/// <returns>Texture</returns>
		private static Texture SearchEffectTextureParameter(
			EffectDefault[] parameters, string paramName, Texture defaultTexture)
		{
			object ret = SearchEffectParameter(parameters, paramName);
			if (ret != null &&
				ret.GetType() == typeof(string))
			{
				// Use the models directory
				return new Texture(
					Directories.TextureModelsSubDirectory + "\\" +
					StringHelper.ExtractFilename((string)ret, true));
			} // if (ret)
			// Not found? Then just return default value.
			return defaultTexture;
		} // SearchEffectTextureParameter(parameters, paramName, defaultTexture)
		 */
		#endregion

		#region Constructor for creating material from EffectInstance from x file
		/*TODO
		/// <summary>
		/// Material
		/// </summary>
		public Material(EffectInstance modelEffectInstance,
			ExtendedMaterial dxMaterial)
		{
			EffectDefault[] parameters = modelEffectInstance.GetDefaults();

			// If shader could not be loaded or is missing, we can't set
			// any shader parameters, load material normally without shaders.
			if (GraphicForm.ParallaxShader.Valid == false)
			{
				// Load material like a normal extended material.
				LoadExtendedMaterial(dxMaterial);

				// Leave rest to default, only load diffuseTexture from shader
				// if none is set in the extended material.
				if (diffuseTexture == null)
					diffuseTexture = SearchEffectTextureParameter(
						parameters, "diffuseTexture", null);

				// Get outta here, all the advanced shader stuff is not required.
				return;
			} // if (GraphicForm.ParallaxShader.Valid)

			d3dMaterial.Ambient = SearchEffectColorParameter(
				parameters, "ambientColor", DefaultAmbientColor);
			d3dMaterial.Diffuse = SearchEffectColorParameter(
				parameters, "diffuseColor", DefaultDiffuseColor);
			d3dMaterial.Specular = SearchEffectColorParameter(
				parameters, "specularColor", DefaultSpecularColor);
			d3dMaterial.SpecularSharpness = SearchEffectFloatParameter(
				parameters, "shininess", DefaultShininess);

			// If diffuse is white, reduce it to nearly white!
			if (d3dMaterial.Diffuse == Color.White)
				d3dMaterial.Diffuse = Color.FromArgb(255, 230, 230, 230);
			// Same for specular color
			if (d3dMaterial.Specular == Color.White)
				d3dMaterial.Specular = Color.FromArgb(255, 230, 230, 230);

			diffuseTexture = SearchEffectTextureParameter(
				parameters, "diffuseTexture", null);
			normalTexture = SearchEffectTextureParameter(
				parameters, "normalTexture", null);
			heightTexture = SearchEffectTextureParameter(
				parameters, "heightTexture", null);

			parallaxAmount = SearchEffectFloatParameter(
				parameters, "parallaxAmount", DefaultParallaxAmount);
		} // Material(modelEffectInstance, dxMaterial)
		 */
		#endregion

		#region Create material from effect settings
		/// <summary>
		/// Create material
		/// </summary>
		/// <param name="effect">Effect</param>
		public Material(IXNAGame _game, Effect effect)
		{
			EffectParameter diffuseTextureParameter =
				effect.Parameters[ "diffuseTexture" ];
			if ( diffuseTextureParameter != null )
				diffuseTexture = new TextureBookengine( _game,
					diffuseTextureParameter.GetValueTexture2D() );

			EffectParameter normalTextureParameter =
				effect.Parameters[ "normalTexture" ];
			if ( normalTextureParameter != null )
				normalTexture = new TextureBookengine( _game,
					normalTextureParameter.GetValueTexture2D() );

			EffectParameter diffuseColorParameter =
				effect.Parameters[ "diffuseColor" ];
			if ( diffuseColorParameter != null )
				diffuseColor = new Color( diffuseColorParameter.GetValueVector4() );

			EffectParameter ambientColorParameter =
				effect.Parameters[ "ambientColor" ];
			if ( ambientColorParameter != null )
				ambientColor = new Color( ambientColorParameter.GetValueVector4() );

			EffectParameter specularColorParameter =
				effect.Parameters[ "specularColor" ];
			if ( specularColorParameter != null )
				specularColor = new Color( specularColorParameter.GetValueVector4() );

			EffectParameter specularPowerParameter =
				effect.Parameters[ "specularPower" ];
			if ( specularPowerParameter != null )
				specularPower = specularPowerParameter.GetValueSingle();
		} // Material(effect)
		#endregion
		#endregion

		#region Dispose
		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			if ( diffuseTexture != null )
				diffuseTexture.Dispose();
			if ( normalTexture != null )
				normalTexture.Dispose();
			if ( heightTexture != null )
				heightTexture.Dispose();
			if ( detailTexture != null )
				detailTexture.Dispose();
		} // Dispose()
		#endregion
	} // class Material
} // namespace XnaGraphicEngine.Graphics

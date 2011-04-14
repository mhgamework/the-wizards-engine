using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorMaterialCollada : EditorMaterial
    {
      
        private BasicShader shader;


        public Color Ambient;
        public Color Diffuse;
        public Color Specular;
        public float Shininess;

        public string DiffuseTexture;
        public float DiffuseTextureRepeatU = 1;
        public float DiffuseTextureRepeatV = 1;
        public string NormalTexture;
        public float NormalTextureRepeatU = 1;
        public float NormalTextureRepeatV = 1;




        public EditorMaterialCollada()
        {


        }

        public static EditorMaterialCollada FromColladaMaterial( ColladaMaterial mat )
        {
            EditorMaterialCollada ret = new EditorMaterialCollada( );

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

        }

        public override void Initialize( IXNAGame game )
        {
            base.Initialize( game );
            //TODO: We could ask the shader parameters as arguments to the intialize function like in EditorModelRenderData.Initialize
            LoadShader( game );
        }

        private void LoadShader( IXNAGame game )
        {


            //
            // Load the shader and set the material properties
            //

            shader = BasicShader.LoadFromFXFile( game, game.EngineFiles.GetColladaModelShaderStream(),null );

            //ret.shader.SetTechnique( "SpecularPerPixelNormalMapping" );
            //ret.shader.SetTechnique( "SpecularPerPixel" );
            shader.SetTechnique( "SpecularPerPixelColored" );


            //TODO: world matrix not correctly implemented
            //TODO: lightdir

            shader.SetParameter( "lightDir", Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) ) );
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BasenGame.LightDirection );
            //ColladaMaterial mat = meshPart.Material;

            // Set all material properties

            shader.SetParameter( "ambientColor", new Vector4( 0.25f, 0.25f, 0.25f, 1f ) );
            //AmbientColor = setMat.Ambient;
            shader.SetParameter( "diffuseColor", Diffuse );
            shader.SetParameter( "specularColor", Specular );

            //ret.shader.SetParameter( "shininess", 80f );
            //ret.shader.SetParameter( "shininess", mat.Shininess );


            TWTexture texture;
            if ( DiffuseTexture != null )
            {
                shader.SetTechnique( "SpecularPerPixel" );
                texture = TWTexture.FromImageFile( game, new GameFile( DiffuseTexture ) );

                shader.SetParameter( "diffuseTexture", texture );
                shader.SetParameter( "diffuseTextureRepeatU", DiffuseTextureRepeatU );
                shader.SetParameter( "diffuseTextureRepeatV", DiffuseTextureRepeatV );
            }
            if ( NormalTexture != null )
            {
                shader.SetTechnique( "SpecularPerPixelNormalMapping" );
                texture = TWTexture.FromImageFile( game, new GameFile( NormalTexture ) );

                shader.SetParameter( "normalTexture", texture );
                shader.SetParameter( "normalTextureRepeatU", NormalTextureRepeatU );
                shader.SetParameter( "normalTextureRepeatV", NormalTextureRepeatV );

            }
            //NormalTexture = setMat.normalTexture;

        }

        protected override void RenderInternal( IXNAGame game )
        {
            base.RenderInternal( game );
            shader.ViewProjection = game.Camera.ViewProjection;
            shader.ViewInverse = game.Camera.ViewInverse;

            shader.effect.Begin( SaveStateMode.None );

            for ( int i = 0; i < batchedCount; i++ )
            {
                shader.World = batchedPrimitives[ i ].WorldMatrix;


                // Render all passes (usually just one)
                //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                for ( int num = 0; num < shader.effect.CurrentTechnique.Passes.Count; num++ )
                {
                    EffectPass pass = shader.effect.CurrentTechnique.Passes[ num ];

                    pass.Begin();
                    batchedPrimitives[ i ].RenderPrimitives();
                    pass.End();
                } // foreach (pass)



            }


            // End shader
            shader.effect.End();
        }

        /*public void SaveToXML( TWXmlNode node )
        {
            XMLSerializer.WriteColor( node.CreateChildNode( "Ambient" ), this.Ambient );
            XMLSerializer.WriteColor( node.CreateChildNode( "Diffuse" ), this.Diffuse );
            XMLSerializer.WriteColor( node.CreateChildNode( "Specular" ), this.Specular );
            node.AddChildNode( "Shininess", this.Shininess.ToString() );

            node.AddChildNode( "DiffuseTexture", this.DiffuseTexture );
            node.AddChildNode( "DiffuseTextureRepeatU", this.DiffuseTextureRepeatU.ToString() );
            node.AddChildNode( "DiffuseTextureRepeatV", this.DiffuseTextureRepeatV.ToString() );

            node.AddChildNode( "NormalTexture", this.NormalTexture );
            node.AddChildNode( "NormalTextureRepeatU", this.NormalTextureRepeatU.ToString() );
            node.AddChildNode( "NormalTextureRepeatV", this.NormalTextureRepeatV.ToString() );


        }
        public static EditorMaterialCollada LoadFromXML( EditorObject obj, TWXmlNode node )
        {
            EditorMaterialCollada ret = new EditorMaterialCollada( obj );

            ret.Ambient = XMLSerializer.ReadColor( node.FindChildNode( "Ambient" ) );
            ret.Diffuse = XMLSerializer.ReadColor( node.FindChildNode( "Diffuse" ) );
            ret.Specular = XMLSerializer.ReadColor( node.FindChildNode( "Specular" ) );
            ret.Shininess = float.Parse( node.ReadChildNodeValue( "Shininess" ) );

            ret.DiffuseTexture = node.ReadChildNodeValue( "DiffuseTexture" );
            ret.DiffuseTextureRepeatU = float.Parse( node.ReadChildNodeValue( "DiffuseTextureRepeatU" ) );
            ret.DiffuseTextureRepeatV = float.Parse( node.ReadChildNodeValue( "DiffuseTextureRepeatV" ) );


            ret.NormalTexture = node.ReadChildNodeValue( "NormalTexture" );
            ret.NormalTextureRepeatU = float.Parse( node.ReadChildNodeValue( "NormalTextureRepeatU" ) );
            ret.NormalTextureRepeatV = float.Parse( node.ReadChildNodeValue( "NormalTextureRepeatV" ) );

            return ret;
        }*/

        //private void RenderPrimitives()
        //{
        //    for ( int i = 0; i < batchedCount; i++ )
        //    {
        //        batchedPrimitives[ i ].RenderPrimitives();
        //    }
        //}


        public override void Dispose()
        {
            //TODO: create shader.dispose
            shader.effect.Dispose();

            shader = null;
            
        }
    }
}

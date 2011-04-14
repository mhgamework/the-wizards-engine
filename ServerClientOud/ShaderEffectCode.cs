using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ShaderEffectCode : IUnique, IXmlSerializable
    {
        private int id = -1;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private byte[] effectCode;

        public byte[] EffectCode
        {
            get { return effectCode; }
            set { effectCode = value; }
        }

        public void SaveToXml( TWXmlNode node )
        {
            node.AddChildNode( "ID", id.ToString() );
            node.CreateChildNode( "EffectCode" ).AddCData( Convert.ToBase64String( effectCode ) );

        }

        public void LoadFromXml( TWXmlNode node )
        {
            id = int.Parse( node.ReadChildNodeValue( "ID" ) );
            effectCode = Convert.FromBase64String( node.FindChildNode( "EffectCode" ).ReadCData() );
        }

        public static void TestSerialize()
        {
            TestXNAGame game = null;
            TestXNAGame.Start( "ShaderEffectCode.TestSerialize",
                delegate
                {
                    game = TestXNAGame.Instance;

                    Microsoft.Xna.Framework.Graphics.CompiledEffect compiledEffect;

                    //string filename = game.EngineFiles.ColladaModelShader.GetFullFilename();
                    Stream strm = game.EngineFiles.GetColladaModelShaderStream();
                    System.IO.StreamReader reader = null;
                    reader = new System.IO.StreamReader( strm );
                    string source = reader.ReadToEnd();

                    compiledEffect = Microsoft.Xna.Framework.Graphics.Effect.CompileEffectFromSource(
                        source, null, null,
                        Microsoft.Xna.Framework.Graphics.CompilerOptions.None,
                        Microsoft.Xna.Framework.TargetPlatform.Windows );

                    ShaderEffectCode shaderEffectCode = new ShaderEffectCode();

                    shaderEffectCode.EffectCode = compiledEffect.GetEffectCode();

                    System.Xml.XmlDocument doc = TWXmlNode.CreateXmlDocument();
                    TWXmlNode shaderEffectCodeNode = new TWXmlNode( doc, "ShaderEffectCode" );
                    shaderEffectCode.SaveToXml( shaderEffectCodeNode );
                    doc.Save( game.EngineFiles.DirUnitTests + "ShaderEffectCode_TestSerialize.xml" );

                    shaderEffectCodeNode = TWXmlNode.GetRootNodeFromFile( game.EngineFiles.DirUnitTests + "ShaderEffectCode_TestSerialize.xml" );

                    shaderEffectCode = new ShaderEffectCode();
                    shaderEffectCode.LoadFromXml( shaderEffectCodeNode );

                    doc = TWXmlNode.CreateXmlDocument();
                    shaderEffectCodeNode = new TWXmlNode( doc, "ShaderEffectCode" );
                    shaderEffectCode.SaveToXml( shaderEffectCodeNode );
                    doc.Save( game.EngineFiles.DirUnitTests + "ShaderEffectCode_TestSerialize2.xml" );


                }, delegate
                {
                } );
        }

    }
}

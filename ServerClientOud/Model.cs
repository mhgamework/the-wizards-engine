using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Collada;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// DEPRECATED!!! To be renamed to Model
    /// 
    /// </summary>
    public class TWModel
    {
        IXNAGame game;
        Matrix worldMatrix = Matrix.Identity;
        private List<IModelPart> parts = new List<IModelPart>();

        public List<IModelPart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }


        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                for (int i = 0; i < parts.Count; i++)
                {
                    parts[i].UpdateWorldMatrix(worldMatrix);
                }
            }
        }


        private TWModel(IXNAGame nGame)
        {
            game = nGame;
        }

        public void Render()
        {
            game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            //game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            //game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            //game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].Render();
            }
        }

        public void RenderPrimitives()
        {
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].RenderPrimitives();
            }
        }


        public void SaveToXML(TWXmlNode node)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                TWXmlNode partNode = node.CreateChildNode("ModelPart");
                parts[i].SaveToXML(partNode);
            }
        }

        public void LoadFromXML(TWXmlNode node)
        {
        }


        public static TWModel FromColladaFile(IXNAGame game, GameFile colladaFile)
        {
            ColladaModel colladaModel = ColladaModel.FromFile(colladaFile);

            return FromColladaModel(game, colladaModel);
        }

        public static TWModel FromColladaModel(IXNAGame game, ColladaModel colladaModel)
        {
            TWModel model = new TWModel(game);
            ColladaScene colladaScene = colladaModel.Scene;

            //Add Parts
            for (int iNode = 0; iNode < colladaScene.Nodes.Count; iNode++)
            {
                LoadFromColladaSceneNode(model, colladaScene.Nodes[iNode]);

            }


            return model;
        }

        private static void LoadFromColladaSceneNode(TWModel model, ColladaSceneNodeBase node)
        {
            if (node.Type != ColladaSceneNodeBase.NodeType.Node) return;
            if (node.Instance_Geometry != null)
            {
                ColladaMesh mesh = node.Instance_Geometry;

                for (int iPart = 0; iPart < mesh.Parts.Count; iPart++)
                {
                    ColladaMesh.PrimitiveList meshPart = mesh.Parts[iPart];
                    ModelPartStatic modelPart = ModelPartStatic.FromColladaMeshPart(model, meshPart);
                    model.parts.Add(modelPart);

                    modelPart.localMatrix = node.GetFullMatrix();
                }
            }

            for (int iNode = 0; iNode < node.Nodes.Count; iNode++)
            {
                LoadFromColladaSceneNode(model, node.Nodes[iNode]);



            }

        }




        public IXNAGame Game
        {
            get { return game; }
        }




        public static void TestRenderModelFromCollada()
        {
            TestXNAGame game = null;
            TWModel model = null;

            TestXNAGame.Start("TestLoadColladaModel",
                delegate
                {
                    game = TestXNAGame.Instance;

                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Wall001.dae" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\NWNTree001.DAE" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Terras026.DAE" );
                    GameFile file = new GameFile(System.Windows.Forms.Application.StartupPath + @"\Content\PakhuisMarktplaats044.DAE");
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Docks.DAE" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Martkplein.DAE" );

                    model = TWModel.FromColladaFile(game, file);

                    model.WorldMatrix = Matrix.CreateRotationX(-MathHelper.PiOver2);
                },
                delegate
                {
                    model.Render();
                });






        }

        public static void TestSaveLoadXML()
        {
            TWModel model = null;

            TestXNAGame.Start("TestSaveLoadXML",
                delegate
                {
                    GameFile file = new GameFile(System.Windows.Forms.Application.StartupPath + @"\Content\Wall001.dae");
                    model = TWModel.FromColladaFile(TestXNAGame.Instance, file);

                    System.Xml.XmlDocument doc = TWXmlNode.CreateXmlDocument();
                    TWXmlNode modelNode = new TWXmlNode(doc, "Model");

                    model.SaveToXML(modelNode);

                    System.IO.FileStream fs = null;
                    try
                    {
                        fs = new System.IO.FileStream(System.Windows.Forms.Application.StartupPath + @"\Content\Wall001_TWModel.xml"
                           , System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Delete);

                        doc.Save(fs);
                    }
                    finally
                    {
                        fs.Close();
                    }

                    model = new TWModel(TestXNAGame.Instance);

                    model.LoadFromXML(modelNode);


                },
                delegate
                {
                });




        }
    }
}

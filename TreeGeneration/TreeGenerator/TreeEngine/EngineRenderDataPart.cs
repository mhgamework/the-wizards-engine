using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Graphics.Xna.Graphics;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Common.Core.Collada;
using MHGameWork.TheWizards.Common.Core.Graphics;
using TreeGenerator.Clouds;
using Seeder = TreeGenerator.help.Seeder;


namespace TreeGenerator.TreeEngine
{
    public class EngineTreeRenderDataPart
    {
        public List<TangentVertex> Vertices1 = new List<TangentVertex>();
        public bool RenderAsBillBoards = false;


        public int NumVertices;
        public Vector3 Position;

        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader shader1;
        public BasicShader depthShader;

        public BasicShader BillBoardShader;
        public BasicShader depthBillBoardShader;
        public BasicShader shaderOrthographic;

        private TreeRenderManager renderManager;
        IXNAGame game;
        GraphicsDevice device;
        public float UFactor = 1;
        public float Vfactor = 1;

        public string Texture, BumpTexture;
        public Vector3[] BoundingBox = new Vector3[8];

        public EngineTreeRenderDataPart(Vector3 pos)
        {
            Position = pos;
        }

        public void Initialize(IXNAGame _game)
        {
            game = _game;
            device = game.GraphicsDevice;
            if (Texture != null)
            {
                if (RenderAsBillBoards)
                {
                    InitializeAsBillBoardedLeafs(_game, Texture);
                }
                else
                {
                    if (BumpTexture == null)
                    {
                        Initialize(game, Texture);
                    }
                    else
                    {
                        InitializeBump(game, Texture, BumpTexture);
                    }
                }
            }
            else
            {
                initializeWithoutTexture(game);
            }

            SetWorldMatrix(Matrix.Identity);
        }
        private void initializeWithoutTexture(IXNAGame _game)
        {
            game = _game;
            decl = TangentVertexExtensions.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = Vertices1.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(Vertices1.ToArray());


            shader1 = new ColladaShader(_game, new Microsoft.Xna.Framework.Graphics.EffectPool());
            shader1.Technique = ColladaShader.TechniqueType.Textured;


            shader1.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            shader1.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            shader1.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);



            shader1.Technique = ColladaShader.TechniqueType.Textured;


            CalculateBoundingBox();

            Vertices1 = null; //for testing only
        }
        private void Initialize(IXNAGame _game, string texture)
        {

            game = _game;
            decl = TangentVertexExtensions.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = Vertices1.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(Vertices1.ToArray());
            renderManager = new TreeRenderManager();
            renderManager.Intialize(game, texture);

            shader1 = renderManager.Shader.Clone();

            shader1.AmbientColor = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
            shader1.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            shader1.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);


            shader1.DiffuseTexture = renderManager.Texture.XnaTexture;
            shader1.Technique = ColladaShader.TechniqueType.Textured;


            CalculateBoundingBox();

            //depthShader = BasicShader.LoadFromFXFile(game, new GameFile(System.Windows.Forms.Application.StartupPath + @"\Engine\Depth.fx"));
            //depthShader.SetParameter("g_matView", game.Camera.View);
            //depthShader.SetParameter("g_matProj", game.Camera.Projection);
            //depthShader.SetParameter("g_fFarClip", game.Camera.FarClip);
            //depthShader.SetParameter("g_matWorld", Matrix.Identity);

            //Vertices1 = null; //for testing only
        }

        private void InitializeBump(IXNAGame _game, string texture, string bumpTexture)
        {

            game = _game;


            decl = TangentVertexExtensions.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = Vertices1.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(Vertices1.ToArray());

            renderManager.IntializeBumpMapping(game, texture, bumpTexture);
            shader1 = renderManager.Shader.Clone();

            shader1.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            shader1.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            shader1.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);


            shader1.DiffuseTexture = renderManager.Texture.XnaTexture;
            shader1.NormalTexture = renderManager.Texture.XnaTexture;//not sure
            shader1.Technique = ColladaShader.TechniqueType.TexturedNormalMapping;

            CalculateBoundingBox();

            Vertices1 = null; //for testing only

        }
        private void InitializeAsBillBoardedLeafs(IXNAGame _game, string texture)
        {
            game = _game;
            TWTexture TextureImage = TWTexture.FromImageFile(game,new MHGameWork.TheWizards.ServerClient.GameFile(texture));

            BillBoardShader = BasicShader.LoadFromEmbeddedFile(game, Assembly.GetExecutingAssembly(), "TreeGenerator.TreeEngine.BillBoardShader.fx", "..\\..\\..\\TreeEngine\\BillBoardShader.fx", new EffectPool());
            BillBoardShader.SetTechnique("Billboard");
            BillBoardShader.SetParameter("world", Matrix.Identity);
            BillBoardShader.SetParameter("viewProjection", Matrix.Identity);
            BillBoardShader.SetParameter("viewInverse", Matrix.Identity);
            BillBoardShader.SetParameter("diffuseTexture", TextureImage);

            //depthBillBoardShader = shaderOrthographic = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"TreeEngine\PointSpritePerspectiveDepthShader.fx"));
            //depthBillBoardShader.SetTechnique("PointSprites");
            //depthBillBoardShader.SetParameter("world", Matrix.Identity);
            //depthBillBoardShader.SetParameter("viewProjection", Matrix.Identity);
            //depthBillBoardShader.SetParameter("viewInverse", Matrix.Identity);
            //depthBillBoardShader.SetParameter("g_matView", Matrix.Identity);


            vertexStride = TangentVertex.SizeInBytes;
            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), Vertices1.Count, BufferUsage.None);
            vertexBuffer.SetData(Vertices1.ToArray());
            decl = new VertexDeclaration(device, TangentVertex.VertexElements);
            vertexCount = Vertices1.Count;

            CalculateBoundingBox();
        }

        // float time = 0;
        public void RenderTree(Matrix viewProjection, Matrix viewInverse)
        {


            if (RenderAsBillBoards)
            {
                RenderState renderState = device.RenderState;

                //device.RenderState.CullMode = CullMode.None;

                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 80;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;



                //shader.SetParameter("time", time);
                BillBoardShader.SetParameter("viewProjection", viewProjection);
                BillBoardShader.SetParameter("viewInverse", viewInverse);
                BillBoardShader.RenderMultipass(renderPrimitivesAsBillBoards);


            }
            else
            {
                //device.RenderState.CullMode = CullMode.None;
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 80;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
                shader1.ViewProjection = viewProjection;
                shader1.ViewInverse = viewInverse;
                shader1.Shader.RenderMultipass(RenderPrimitives);
                //for (int i = 0; i < vertexCount; i++)
                //{
                //    game.LineManager3D.AddLine(Vertices1[i].pos,Vertices1[i].pos+ Vertices1[i].normal*0.2f, Color.Red);
                //}
            }
        }
        public void RenderTree()
        {



            if (RenderAsBillBoards)
            {
                RenderState renderState = device.RenderState;
                //device.RenderState.CullMode = CullMode.None;

                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 80;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;


                BillBoardShader.SetParameter("viewProjection", game.Camera.ViewProjection);
                BillBoardShader.SetParameter("viewInverse", game.Camera.ViewInverse);
                BillBoardShader.RenderMultipass(renderPrimitivesAsBillBoards);

            }
            else
            {
                // device.RenderState.CullMode = CullMode.None;
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 80;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
                shader1.ViewProjection = game.Camera.ViewProjection;
                shader1.ViewInverse = game.Camera.ViewInverse;
                shader1.Shader.RenderMultipass(RenderPrimitives);
            }

        }
        public void RenderPrimitives()
        {

            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.VertexDeclaration = decl;
            //device.RenderState.CullMode = CullMode.None;

            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }
        private void renderPrimitivesAsBillBoards()
        {
            device.VertexDeclaration = decl;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexCount);
        }
        public void RenderLinearDepth(ICamera cam)
        {
            Matrix viewProjection = cam.ViewProjection;
            Matrix view = cam.View;
            float farclipPlane = cam.FarClip;
            Matrix projection = cam.Projection;
            if (RenderAsBillBoards)
            {
                RenderState renderState = device.RenderState;
                device.RenderState.PointSpriteEnable = true;
                device.RenderState.CullMode = CullMode.None;

                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 80;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;

                //shader.SetParameter("time", time);
                depthBillBoardShader.SetParameter("viewProjection", viewProjection);
                depthBillBoardShader.SetParameter("g_matProj", projection);
                depthBillBoardShader.SetParameter("g_matView", view);
                depthBillBoardShader.SetParameter("g_fFarClip", farclipPlane);
                depthBillBoardShader.RenderMultipass(renderPrimitivesAsBillBoards);
                device.RenderState.PointSpriteEnable = false;

            }
            else
            {
                device.RenderState.CullMode = CullMode.None;
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 80;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
                depthShader.SetParameter("g_matView", view);
                depthShader.SetParameter("g_matProj", projection);
                depthShader.SetParameter("g_fFarClip", farclipPlane);

                depthShader.RenderMultipass(RenderPrimitives);
            }
        }

        public void SetWorldMatrix(Matrix mat)
        {
            if (RenderAsBillBoards)
            {
                BillBoardShader.SetParameter("world", mat);
                //depthBillBoardShader.SetParameter("world", mat);

            }
            else
            {
                shader1.World = mat;
                //depthShader.SetParameter("g_matWorld", mat);
            }


        }

        public void TransFormBoundingBox(Matrix mat)
        {

            Vector3.Transform(BoundingBox, ref mat, BoundingBox);
        }

        private void CalculateBoundingBox()
        {
            Vector3 min = Vertices1[0].pos;
            Vector3 max = Vertices1[0].pos;
            for (int i = 0; i < Vertices1.Count; i++)
            {
                Vector3 pos = Vertices1[i].pos;
                Vector3.Min(ref min, ref pos, out min);
                Vector3.Max(ref max, ref pos, out max);
            }
            BoundingBox[0] = new Vector3(min.X, min.Y, min.Z);
            BoundingBox[1] = new Vector3(max.X, min.Y, min.Z);
            BoundingBox[2] = new Vector3(max.X, min.Y, max.Z);
            BoundingBox[3] = new Vector3(min.X, min.Y, max.Z);

            BoundingBox[4] = new Vector3(min.X, max.Y, max.Z);
            BoundingBox[5] = new Vector3(max.X, max.Y, max.Z);
            BoundingBox[6] = new Vector3(max.X, max.Y, min.Z);
            BoundingBox[7] = new Vector3(min.X, max.Y, min.Z);

        }





        public static void TestPointSpriteLeaves()
        {
            XNAGame game;
            game = new XNAGame();

            EngineTreeRenderDataPart part = new EngineTreeRenderDataPart(Vector3.Zero);
            Seeder seeder = new Seeder(46);
            game.InitializeEvent +=
                delegate
                {
                    for (int i = 0; i < 50; i++)
                    {
                        part.Vertices1.Add(
                            new TangentVertex(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(20, 20, 20)),
                                              new Vector2(1, 1), Vector3.Zero, Vector3.Zero));
                    }
                    part.RenderAsBillBoards = true;
                    part.Texture = "speedtree/DefaultLeaves.tga";
                    part.Initialize(game);

                };
            game.DrawEvent +=
                delegate
                {

                    part.RenderTree();
                };
            game.Run();
        }

    }
}

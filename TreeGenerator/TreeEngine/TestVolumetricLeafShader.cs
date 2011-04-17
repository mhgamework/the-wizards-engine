using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace TreeGenerator.TreeEngine
{
    
    public class TestVolumetricLeafShader
    {
        private TreeStructureGenerater genStruct = new TreeStructureGenerater();
        private EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);

        private TreeLeafType leafType;
        private TreeStructureLeaf leaf = new TreeStructureLeaf();

        private List<TangentVertex> tangentVertices = new List<TangentVertex>();
        private List<VolumetricLeafVertex> leafVertices = new List<VolumetricLeafVertex>();

        private XNAGame game;
        private BasicShader shader;
        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;

        private Vector3 windDirection;
        private float windStrength;

        public void initialize(XNAGame _game)
        {
            game = _game;
            leafType = TreeTypeData.GetTestTreeType().Levels[0].LeafType[0];
            leaf=genStruct.CreateLeave(leafType, new help.Directions(Vector3.UnitZ, Vector3.UnitX), 0, 0, 0, 0, 0, 0);
            tangentVertices= gen.CreateVerticesForVolumetricLeaf(leaf, Vector3.Zero);
            convertVertices();
           
            
            decl = new VertexDeclaration(game.GraphicsDevice, VolumetricLeafVertex.VertexElements);
            vertexStride = VolumetricLeafVertex.SizeInBytes;
            vertexCount = leafVertices.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VolumetricLeafVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(leafVertices.ToArray());

            TWTexture TextureImage = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + (@"Textures\speedtree\CoconutPalmFrond.tga")));
            shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"TreeEngine\LeafMotionShader.fx"));
            shader.SetParameter("diffuseTexture", TextureImage);

            shader.SetParameter("frequency", 15f);
            shader.SetParameter("waveSpeed", 3f);
            shader.SetParameter("windDirection", Vector3.Right);
            shader.SetParameter("windStrength", 1f);
            shader.SetParameter("fBendScale", 1f);
            shader.SetParameter("centre", new Vector3(0,-1,1));

            shader.SetParameter("leafNormal", Vector3.Up);
            shader.SetParameter("leafTangent", Vector3.Right);

        }

        public void convertVertices()
        {
            for (int i = 0; i < tangentVertices.Count; i++)
            {
                leafVertices.Add(new VolumetricLeafVertex(tangentVertices[i].pos, tangentVertices[i].uv, tangentVertices[i].normal, tangentVertices[i].tangent, 0, 1.0f / (tangentVertices.Count - i)));
            }
        }

        private float time;
        public void render()
        {
            if (time < ((MathHelper.TwoPi / (float)shader.GetParameter("frequency").GetValueSingle())))
            {
                time += game.Elapsed * 0.2f;
            }
            else { time = 0; }
            shader.SetParameter("time", time);
            shader.SetParameter("world", Matrix.Identity);
            shader.SetParameter("viewProjection", game.Camera.ViewProjection);

            shader.RenderMultipass(RenderPrimitives);
        }
        public void RenderPrimitives()
        {

            game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            game.GraphicsDevice.VertexDeclaration = decl;
            game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }
        public struct VolumetricLeafVertex
        {
            public Vector3 position;
            public Vector2 texCoord;
            public Vector3 Normal;
            public Vector3 Tangent;
            public float TreeHeight;
            public float BendingCooficient;


            public VolumetricLeafVertex(Vector3 position, Vector2 texCoord,Vector3 normal,Vector3 tangent,float treeHeight,float bendingCooficient)
            {
                this.position = position;
                this.texCoord = texCoord;
                Normal = normal;
                Tangent = tangent;
                TreeHeight = treeHeight;
                BendingCooficient = bendingCooficient;
            }

            public static readonly VertexElement[] VertexElements =
     {
         new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
         new VertexElement(0, sizeof(float)*3, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
         new VertexElement(0, sizeof(float)*5, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
         new VertexElement(0, sizeof(float)*8, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0),
         new VertexElement(0, sizeof(float)*11, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 0),
         new VertexElement(0, sizeof(float)*12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 1),



     };
            public static int SizeInBytes = sizeof(float) * (3+2+3+3+1+1);
        }

        
        public static void TestLeaf()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.SpectaterCamera.FarClip = 100000;

            TestVolumetricLeafShader test = new TestVolumetricLeafShader();
            game.InitializeEvent +=
                delegate
                    {
                        test.initialize(game);
                    };
            game.DrawEvent +=
                delegate
                    {
                        test.render();

                        for (int i = 0; i < test.vertexCount/3; i++)
                        {
                            game.LineManager3D.AddTriangle(test.leafVertices[i * 3].position, test.leafVertices[i * 3 + 1].position, test.leafVertices[i * 3 + 2].position, Color.Black);
                        }

                        for (int i = 0; i < test.vertexCount / 3; i++)
                        {
                            game.LineManager3D.AddTriangle(test.tangentVertices[i * 3].pos, test.tangentVertices[i * 3 + 1].pos, test.tangentVertices[i * 3 + 2].pos, Color.Red);
                        }
                    };
            game.Run();
        }
    }
}

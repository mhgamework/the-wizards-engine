using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Terrain;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Common.Core.Collada;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Database;
using MHGameWork.TheWizards.ServerClient.Database;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.Terrain.Editor;
using TreeGenerator.NoiseGenerater;
using System.Windows.Forms;

namespace TreeGenerator.Grass
{
    public class GrassMesh
    {
        public List<TangentVertex> Verts1 = new List<TangentVertex>();
        public List<TangentVertex> Verts2 = new List<TangentVertex>();
        public List<TangentVertex> Verts3 = new List<TangentVertex>();


        //public TangentVertex[] Verts;
        public Vector3 Position;

        public VertexBuffer vertexBuffer1, vertexBuffer2, vertexBuffer3;
        public VertexDeclaration decl1, decl2, decl3;
        public int vertexCount1, vertexCount2, vertexCount3;
        public int triangleCount1, triangleCount2, triangleCount3;
        public int vertexStride;
        public BasicShader shader1, shader2, shader3;

        private GraphicsDevice device;
        private IXNAGame game;
        private Seeder seeder;
        private float time;

        Vector3 TerrainPosition = Vector3.Zero;
        TerrainFullData data = null;
        TWTexture GrassMap;
        float densityDivider = 0.20f;
        Vector3[,] DensityData;
        Color[] GrassColors;
        public string DensityTexture = "DifferentGrasstypesTest2.jpg";
        private bool grass2, grass3;


        public void Initialize(IXNAGame _game, bool _grass2, bool _grass3)
        {
            game = _game;
            seeder = new Seeder(123);
            vertexStride = TangentVertex.SizeInBytes;
            grass2 = _grass2;
            grass3 = _grass3;


            shader1 = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"\Engine\GrassShader.fx"));
            shader1.SetTechnique("AnimatedGrass");
            TWTexture texture1 = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory +@"Grass\Textures\GrassTextures\grass.tga"));
            shader1.SetParameter("world", Matrix.Identity);
            shader1.SetParameter("viewProjection", Matrix.Identity);
            shader1.SetParameter("diffuseTexture", texture1);
            shader1.SetParameter("frequency", 10f);
            shader1.SetParameter("amplitude", 0.5f);
            shader1.SetParameter("waveSpeed", 5f);
            shader1.SetParameter("time", 0);
            shader1.SetParameter("startPointWave", Vector3.Zero);
            Vector3 windDirection = new Vector3(0, 0, 1);
            windDirection.Normalize();
            shader1.SetParameter("WindDirection", windDirection);
            if (grass2)
            {
                shader2 = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"\Engine\GrassShader.fx"));
                shader2.SetTechnique("AnimatedGrass");
                TWTexture texture2 = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Grass\Textures\GrassTextures\flower001(purple).dds"));
                shader2.SetParameter("world", Matrix.Identity);
                shader2.SetParameter("viewProjection", Matrix.Identity);
                shader2.SetParameter("diffuseTexture", texture2);
                shader2.SetParameter("frequency", 10f);
                shader2.SetParameter("amplitude", 0.5f);
                shader2.SetParameter("waveSpeed", 5f);
                shader2.SetParameter("time", 0);
                shader2.SetParameter("startPointWave", Vector3.Zero);
                shader2.SetParameter("WindDirection", windDirection);
            }
            if (grass3)
            {
                shader3 = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"\Engine\GrassShader.fx"));
                shader3.SetTechnique("AnimatedGrass");
                TWTexture texture3 = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Grass\Textures\GrassTextures\flower002(orange).dds"));
                shader3.SetParameter("world", Matrix.Identity);
                shader3.SetParameter("viewProjection", Matrix.Identity);
                shader3.SetParameter("diffuseTexture", texture3);
                shader3.SetParameter("frequency", 10f);
                shader3.SetParameter("amplitude", 0.5f);
                shader3.SetParameter("waveSpeed", 5f);
                shader3.SetParameter("time", 0);
                shader3.SetParameter("startPointWave", Vector3.Zero);
                shader3.SetParameter("WindDirection", windDirection);

            }
            //shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"\Engine\BasicTestShader.fx"));
            //shader.SetTechnique("PointSprites");






            //TWTexture texture2 = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Grass\Textures\DesityTextures\GrassMap3.jpg"));
            GrassMap = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Grass\Textures\DesityTextures\" + DensityTexture));
            //GrassMap = texture2;

        }

        public void CreateGrass(Vector3 corner1, Vector3 corner2, Vector2 sizeOfFace1, Vector2 sizeOfFace2, Vector2 sizeOfFace3)
        {
            //terrain
            SetUpTerrain(corner1, corner2);
            //densityMap
            int texWidth = GrassMap.XnaTexture.Width;
            int texHeight = GrassMap.XnaTexture.Height;

            float TileSizeX = (corner2.X - corner1.X) / texWidth;
            float TileSizeY = (corner2.Z - corner1.Z) / texHeight;
            float tileSurface = TileSizeX * TileSizeY;

            LoadHeightData();
            //Vector3 meanColor = getMean(texWidth,texHeight);

            //float fieldSize = tileSurface * DensityData.Length;//Math.Abs(corner1.X-corner2.X)*Math.Abs(corner1.Z-corner2.Z);
            int VertPerGrassPlunce = (4 + 3) * 6;
            //vertexCount=(int)((fieldSize * meanColor.X) * VertPerGrassPlunce);
            //Verts = new TangentVertex[vertexCount];


            int index = 0;
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    Vector3 tempCorner1 = new Vector3(corner1.X + TileSizeX * x, 0, corner1.Y + TileSizeY * y);
                    Vector3 tempCorner2 = new Vector3(corner1.X + TileSizeX * (1 + x), 0, corner1.Y + TileSizeY * (1 + y));

                    for (int i = 0; i < ((tileSurface * DensityData[x, y].X)); i++)
                    {
                        Vector3 pos = seeder.NextVector3(tempCorner1, tempCorner2);

                        CreateStarOneNew(pos, sizeOfFace1, 4, 1);// numGrassBandsInTexture);
                        index += 6;
                        CreateStarTwoNew(pos, sizeOfFace1, 1);//, numGrassBandsInTexture);
                        index += 3;
                        //if (index * VertPerGrassPlunce > vertexCount)
                        //    throw new InvalidOperationException();
                        //}
                    }
                }
            }

            if (grass2)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    for (int y = 0; y < texHeight; y++)
                    {
                        Vector3 tempCorner1 = new Vector3(corner1.X + TileSizeX * x, 0, corner1.Y + TileSizeY * y);
                        Vector3 tempCorner2 = new Vector3(corner1.X + TileSizeX * (1 + x), 0, corner1.Y + TileSizeY * (1 + y));

                        for (int i = 0; i < (tileSurface * DensityData[x, y].Y); i++)
                        {
                            Vector3 pos = seeder.NextVector3(tempCorner1, tempCorner2);

                            CreateStarOneNew(pos, sizeOfFace2, 6, 2);
                            index += 6;
                            CreateStarTwoNew(pos, sizeOfFace2, 2);
                            index += 3;

                        }
                    }
                }
            }
            if (grass3)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    for (int y = 0; y < texHeight; y++)
                    {
                        Vector3 tempCorner1 = new Vector3(corner1.X + TileSizeX * x, 0, corner1.Y + TileSizeY * y);
                        Vector3 tempCorner2 = new Vector3(corner1.X + TileSizeX * (1 + x), 0, corner1.Y + TileSizeY * (1 + y));

                        for (int i = 0; i < (tileSurface * DensityData[x, y].Z); i++)
                        {
                            Vector3 pos = seeder.NextVector3(tempCorner1, tempCorner2);

                            CreateStarOneNew(pos, sizeOfFace3, 6, 3);
                            index += 6;
                            CreateStarTwoNew(pos, sizeOfFace3, 3);
                            index += 3;

                        }
                    }
                }
            }
        }
        public void CreateStarOneNew(Vector3 position, Vector2 sizeOfFace, int numPlains, int index)
        {
            sizeOfFace += new Vector2(seeder.NextFloat(-sizeOfFace.X * 0.5f, sizeOfFace.Y * 0.5f), seeder.NextFloat(-sizeOfFace.X * 0.5f, sizeOfFace.Y * 0.5f));


            float anglePerPlain = MathHelper.Pi / numPlains;


            for (int i = 0; i < numPlains; i++)
            {

                Vector3 dir = new Vector3((float)Math.Sin(anglePerPlain * (i + 1)), 0, (float)Math.Cos(anglePerPlain * (i + 1)));

                CreatePlainNew(position, dir, sizeOfFace, index);


            }
        }
        public void CreateStarTwoNew(Vector3 position, Vector2 sizeOfFace, int index)
        {
            sizeOfFace += new Vector2(seeder.NextFloat(-sizeOfFace.X * 0.5f, sizeOfFace.Y * 0.5f), seeder.NextFloat(-sizeOfFace.X * 0.5f, sizeOfFace.Y * 0.5f));


            List<TangentVertex> verts = new List<TangentVertex>();
            List<TangentVertex> tempVerts = new List<TangentVertex>();
            //float VPerLevel = (1 / numGrassBandsInTexture);
            //int texPos = seeder.NextInt(1, numGrassBandsInTexture - 1);
            CreatePlainNew(position - Vector3.Left * sizeOfFace.Y * 0.35f, Vector3.Forward, sizeOfFace, index);//, new Vector2(VPerLevel * (texPos - 1), VPerLevel * texPos));

            CreatePlainNew(position + new Vector3(1, 0, 1) * sizeOfFace.Y * 0.3f, new Vector3(1, 0, 1), sizeOfFace, index);//, new Vector2(VPerLevel * (texPos - 1), VPerLevel * texPos));

            CreatePlainNew(position + new Vector3(1, 0, -1) * sizeOfFace.Y * 0.3f, new Vector3(1, 0, -1), sizeOfFace, index);//, new Vector2(VPerLevel * (texPos - 1), VPerLevel * texPos));


        }
        public void CreatePlainNew(Vector3 position, Vector3 direction, Vector2 sizeOfFace, int index)
        {
            direction.Normalize();
            Vector3 normal = Vector3.Up;

            if (index == 1)
            {


                sizeOfFace.Y = seeder.NextFloat(sizeOfFace.Y * 0.9f, sizeOfFace.Y * 1.1f);

                Verts1.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 1, 1, normal, direction));
                Verts1.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 0, 0, normal, direction));//putting random height size in it
                Verts1.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 1, 0, normal, direction));
                //triangle 2
                Verts1.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 1, 1, normal, direction));
                Verts1.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 0, 0, normal, direction));
                Verts1.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f)), 0, 1, normal, direction));
            }
            if (index == 2)
            {
                sizeOfFace.Y = seeder.NextFloat(sizeOfFace.Y * 0.9f, sizeOfFace.Y * 1.1f);

                Verts2.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 1, 1, normal, direction));
                Verts2.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 0, 0, normal, direction));//putting random height size in it
                Verts2.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 1, 0, normal, direction));
                //triangle 2
                Verts2.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 1, 1, normal, direction));
                Verts2.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 0, 0, normal, direction));
                Verts2.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f)), 0, 1, normal, direction));
            }
            if (index == 3)
            {
                sizeOfFace.Y = seeder.NextFloat(sizeOfFace.Y * 0.9f, sizeOfFace.Y * 1.1f);

                Verts3.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 1, 1, normal, direction));
                Verts3.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 0, 0, normal, direction));//putting random height size in it
                Verts3.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 1, 0, normal, direction));
                //triangle 2
                Verts3.Add(new TangentVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 1, 1, normal, direction));
                Verts3.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 0, 0, normal, direction));
                Verts3.Add(new TangentVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f)), 0, 1, normal, direction));
            }


        }
        public void SetRenderData()
        {
            //clean up of data
            GrassMap = null;
            data = null;
            DensityData = null;
            GrassColors = null;

            vertexCount1 = Verts1.Count; //Verts.Length;
            triangleCount1 = vertexCount1 / 3;

            vertexBuffer1 = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount1, BufferUsage.None);
            vertexBuffer1.SetData(Verts1.ToArray());
            Verts1 = null;
            device = vertexBuffer1.GraphicsDevice;
            decl1 = TangentVertex.CreateVertexDeclaration(game);
            if (grass2)
            {
                vertexCount2 = Verts2.Count; //Verts.Length;
                triangleCount2 = vertexCount2 / 3;

                vertexBuffer2 = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount2, BufferUsage.None);
                vertexBuffer2.SetData(Verts2.ToArray());
                Verts2 = null;
                device = vertexBuffer2.GraphicsDevice;
                decl2 = TangentVertex.CreateVertexDeclaration(game);
            }
            if (grass3)
            {

                vertexCount3 = Verts3.Count; //Verts.Length;
                triangleCount3 = vertexCount3 / 3;

                vertexBuffer3 = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount3, BufferUsage.None);
                vertexBuffer3.SetData(Verts3.ToArray());
                Verts3 = null;
                device = vertexBuffer1.GraphicsDevice;
                decl3 = TangentVertex.CreateVertexDeclaration(game);
            }

        }

        public void Render()
        {
            if (time < ((MathHelper.TwoPi / (float)shader1.GetParameter("frequency").GetValueSingle())))
            {
                time += game.Elapsed * 0.2f;
            }
            else { time = 0; }

            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 170;
            device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            RenderPrimitives();


            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 170;
            device.RenderState.AlphaFunction = CompareFunction.Less;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.DepthBufferWriteEnable = false;
            RenderPrimitives();




        }
        private void RenderPrimitives()
        {

            shader1.SetParameter("viewProjection", game.Camera.ViewProjection);
            shader1.SetParameter("time", time);
            shader1.RenderMultipass(renderPrimitive);

            if (grass2)
            {
                shader2.SetParameter("viewProjection", game.Camera.ViewProjection);
                shader2.SetParameter("time", time);
                shader2.RenderMultipass(renderPrimitive2);
            }
            if (grass3)
            {
                shader3.SetParameter("viewProjection", game.Camera.ViewProjection);
                shader3.SetParameter("time", time);
                shader3.RenderMultipass(renderPrimitive3);
            }
        }

        private void renderPrimitive()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;

            device.Vertices[0].SetSource(vertexBuffer1, 0, vertexStride);
            device.VertexDeclaration = decl1;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount1);
        }
        private void renderPrimitive2()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;

            device.Vertices[0].SetSource(vertexBuffer2, 0, vertexStride);
            device.VertexDeclaration = decl2;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount2);
        }
        private void renderPrimitive3()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;

            device.Vertices[0].SetSource(vertexBuffer3, 0, vertexStride);
            device.VertexDeclaration = decl3;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount3);
        }


        //new improved design
        public struct GrassVertex
        {
            public Vector3 Position;
            public float UpOrDown;
            public Vector2 TexUV;
            public Vector3 Normal;
            public Color ColorInf;


            public GrassVertex(Vector3 position, float upOrDown, Vector2 texUV, Vector3 normal,Color color)
            {
                Position = position;
                UpOrDown=upOrDown;
                TexUV = texUV;
                Normal = normal;
                ColorInf=color;
            }

            public static readonly VertexElement[] VertexElements =
             {
                 new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                 new VertexElement(0, 4*(3), VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 0),
                 new VertexElement(0, 4*(3+1), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(0, 4*(3+1+2), VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                 new VertexElement(0,4*(3+1+2+3),VertexElementFormat.Color,VertexElementMethod.Default,VertexElementUsage.Color,0),
                 
             };
            public static int SizeInBytes = sizeof(float) * (3 + 1 + 2  + 3+1);

            public override string ToString()
            {
                string text ="upordown: "+ UpOrDown.ToString()+" position: "+Position.ToString()+" UV: "+TexUV.ToString();
                 return text;
            }
        }
        public List<GrassVertex> grassVertices = new List<GrassVertex>();
        public int NumTexInMap;
        public BasicShader shaderNew;
        int vertexCountNew, triangleCountNew;
        VertexBuffer vertexBufferNew;
        VertexDeclaration declNew;
        List<Vector4> UVCoords=new List<Vector4>();
        public void InitializeNewDesign(IXNAGame _game)
        {
            game = _game;
            seeder = new Seeder(12123);
            vertexStride = GrassVertex.SizeInBytes;



            shaderNew = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"\Grass\GrassShaderNewDesign.fx"));
            shaderNew.SetTechnique("AnimatedGrass");
            TWTexture texture1 = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Grass\Textures\GrassTextures\textureAtlas01.dds"));
            shaderNew.SetParameter("world", Matrix.Identity);
            shaderNew.SetParameter("viewProjection", Matrix.Identity);
            shaderNew.SetParameter("diffuseTexture", texture1);
            shaderNew.SetParameter("frequency", 15f);
            shaderNew.SetParameter("amplitude", 1f);
            shaderNew.SetParameter("waveSpeed", 5f);
            shaderNew.SetParameter("time", 0);
            shaderNew.SetParameter("startPointWave", Vector3.Zero);
            Vector3 windDirection = new Vector3(0, 0, 1);
            windDirection.Normalize();
            shaderNew.SetParameter("WindDirection", windDirection);
            shaderNew.SetParameter("world", Matrix.Identity);

            GrassMap = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Grass\Textures\DesityTextures\" + DensityTexture));
            RetrieveUVFromXML(game.EngineFiles.RootDirectory + @"Grass\Textures\GrassTextures\textureAtlas01");
            
            
            NumTexInMap=UVCoords.Count;
        }
        public void RetrieveUVFromXML(string path)
        {
            TWXmlNode node = TWXmlNode.GetRootNodeFromFile(path + ".XML");

            if (node.Name != "AtlasTextureUV") throw new Exception("Rootnode is not a atlasTexture node! Invalid XML format!");


            foreach (TWXmlNode childNode in node.GetChildNodes())
            {
                //TWXmlNode node = node.GetChildNodes[i];
                if (childNode.Name != "Texture") continue;
               UVCoords.Add(new Vector4(float.Parse(childNode.ReadChildNodeValue("UVStartX")),float.Parse(childNode.ReadChildNodeValue("UVStartY")),float.Parse(childNode.ReadChildNodeValue("UVEndX")),float.Parse(childNode.ReadChildNodeValue("UVEndY"))));
                
            }
        }
        public void CreateGrassNewDesign(Vector3 corner1, Vector3 corner2, Vector2 sizeOfFace1)
        {
            //terrain
            SetUpTerrain(corner1, corner2);
            //densityMap
            int texWidth = GrassMap.XnaTexture.Width;
            int texHeight = GrassMap.XnaTexture.Height;

            float TileSizeX = (corner2.X - corner1.X) / texWidth;
            float TileSizeY = (corner2.Z - corner1.Z) / texHeight;
            float tileSurface = TileSizeX * TileSizeY;

            LoadHeightData();
            //Vector3 meanColor = getMean(texWidth,texHeight);

            //float fieldSize = tileSurface * DensityData.Length;//Math.Abs(corner1.X-corner2.X)*Math.Abs(corner1.Z-corner2.Z);
            int VertPerGrassPlunce = (4 + 3) * 6;
            //vertexCount=(int)((fieldSize * meanColor.X) * VertPerGrassPlunce);
            //Verts = new TangentVertex[vertexCount];


            int index = 0;
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    Vector3 tempCorner1 = new Vector3(corner1.X + TileSizeX * x, 0, corner1.Y + TileSizeY * y);
                    Vector3 tempCorner2 = new Vector3(corner1.X + TileSizeX * (1 + x), 0, corner1.Y + TileSizeY * (1 + y));

                    for (int i = 0; i < ((tileSurface * DensityData[x, y].X)); i++)
                    {
                        Vector3 pos = seeder.NextVector3(tempCorner1, tempCorner2);
                        Color c = Color.Brown;
                        Color c2 = Color.DarkGreen;
                        Color color = seeder.NextColor(c.R, c2.R, c.G, c2.G, c.B, c2.B);
                        
                        index = seeder.NextInt(0, NumTexInMap);
                       
                        CreateStarOneNewDesign(pos, sizeOfFace1, 4,color, index);// numGrassBandsInTexture);
                        //index += 6;
                        CreateStarTwoNewDesign(pos, sizeOfFace1,color, index);//, numGrassBandsInTexture);
                        //index += 3;
                        //if (index * VertPerGrassPlunce > vertexCount)
                        //    throw new InvalidOperationException();
                        //}
                    }
                }
            }


            SetRenderDataNew();
        }
        public void CreateStarOneNewDesign(Vector3 position, Vector2 sizeOfFace, int numPlains,Color color, int index)
        {
            sizeOfFace += new Vector2(seeder.NextFloat(sizeOfFace.X * 0.8f, sizeOfFace.X * 1.2f), seeder.NextFloat(-sizeOfFace.Y * 0.8f, sizeOfFace.Y * 1.2f));


            float anglePerPlain = MathHelper.Pi / numPlains;


            for (int i = 0; i < numPlains; i++)
            {

                Vector3 dir = new Vector3((float)Math.Sin(anglePerPlain * (i + 1)), 0, (float)Math.Cos(anglePerPlain * (i + 1)));

                CreatePlainNewDesign(position, dir, sizeOfFace,color, index);


            }
        }
        public void CreateStarTwoNewDesign(Vector3 position, Vector2 sizeOfFace,Color color, int index)
        {
            sizeOfFace += new Vector2(seeder.NextFloat(sizeOfFace.X * 0.8f, sizeOfFace.X * 1.2f), seeder.NextFloat(-sizeOfFace.Y * 0.8f, sizeOfFace.Y * 1.2f));
            
            CreatePlainNewDesign(position - Vector3.Left * sizeOfFace.Y * 0.35f, Vector3.Forward, sizeOfFace,color, index);//, new Vector2(VPerLevel * (texPos - 1), VPerLevel * texPos));

            CreatePlainNewDesign(position + new Vector3(1, 0, 1) * sizeOfFace.Y * 0.3f, new Vector3(1, 0, 1), sizeOfFace,color, index);//, new Vector2(VPerLevel * (texPos - 1), VPerLevel * texPos));

            CreatePlainNewDesign(position + new Vector3(1, 0, -1) * sizeOfFace.Y * 0.3f, new Vector3(1, 0, -1), sizeOfFace,color, index);//, new Vector2(VPerLevel * (texPos - 1), VPerLevel * texPos));


        }
        public void CreatePlainNewDesign(Vector3 position, Vector3 direction, Vector2 sizeOfFace,Color color, int index)
        {
            direction.Normalize();
            Vector3 normal = Vector3.Up;
            //Vector2 uvX=new Vector2(0,1);// I have to make an automised function for this
            //float unit=1f/NumTexInMap;
            ////Vector2 uvY =new Vector2(unit*index+BetweenTex,unit*(index+1));
            //Vector2 uvY = new Vector2(1, 1);






            grassVertices.Add(new GrassVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 0, new Vector2(UVCoords[index].X, UVCoords[index].W), normal, getNoiseColor(position, Color.Green, Color.Brown)));
            grassVertices.Add(new GrassVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 1, new Vector2(UVCoords[index].Z, UVCoords[index].Y), normal, getNoiseColor(position, Color.Green, Color.Brown)));//putting random height size in it
            grassVertices.Add(new GrassVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 1, new Vector2(UVCoords[index].X, UVCoords[index].Y), normal, getNoiseColor(position, Color.Green, Color.Brown)));
            //triangle 2
            grassVertices.Add(new GrassVertex(RetrieveHeight(position - direction * (sizeOfFace.X * 0.5f)), 0, new Vector2(UVCoords[index].X, UVCoords[index].W), normal, getNoiseColor(position, Color.Green, Color.Brown)));
            grassVertices.Add(new GrassVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f) + Vector3.Up * sizeOfFace.Y), 1, new Vector2(UVCoords[index].Z, UVCoords[index].Y), normal, getNoiseColor(position, Color.Green, Color.Brown)));
            grassVertices.Add(new GrassVertex(RetrieveHeight(position + direction * (sizeOfFace.X * 0.5f)), 0, new Vector2(UVCoords[index].Z, UVCoords[index].W), normal, getNoiseColor(position, Color.Green, Color.Brown)));
        }
        public void SetRenderDataNew()
        {
            //clean up of data
            GrassMap = null;
            data = null;
            DensityData = null;
            GrassColors = null;

            vertexCountNew = grassVertices.Count;
            triangleCountNew = vertexCountNew / 3;

            vertexBufferNew = new VertexBuffer(game.GraphicsDevice, typeof(GrassVertex), vertexCountNew, BufferUsage.None);
            vertexBufferNew.SetData(grassVertices.ToArray());
            //grassVertices = null;
            device = vertexBufferNew.GraphicsDevice;
            declNew = new VertexDeclaration(device, GrassVertex.VertexElements);
          

        }

        public void RenderNewDesign()
        {
            if (time < ((MathHelper.TwoPi / (float)shaderNew.GetParameter("frequency").GetValueSingle())))
            {
                time += game.Elapsed * 0.2f;
            }
            else { time = 0; }
            shaderNew.SetParameter("viewProjection", game.Camera.ViewProjection);
            shaderNew.SetParameter("time", time);

            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 170;
            device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            shaderNew.RenderMultipass(renderPrimitiveNewDesign);


            //device.RenderState.CullMode = CullMode.None;
            //device.RenderState.AlphaTestEnable = true;
            //device.RenderState.ReferenceAlpha = 180;
            //device.RenderState.AlphaFunction = CompareFunction.LessEqual;
            //device.RenderState.AlphaBlendEnable = true;
            //device.RenderState.DepthBufferWriteEnable = true;
            //shaderNew.RenderMultipass(renderPrimitiveNewDesign);
            



        }
        private void renderPrimitiveNewDesign()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;

            device.Vertices[0].SetSource(vertexBufferNew, 0, vertexStride);
            device.VertexDeclaration = declNew;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCountNew);
        }

        private PerlinNoiseGenerater noiseGen = new PerlinNoiseGenerater();
        private Color getNoiseColor(Vector3 pos, Color c1, Color c2)
        {

            Vector3 diff = new Vector3(Math.Abs(c1.ToVector3().X - c2.ToVector3().X), Math.Abs(c1.ToVector3().Y - c2.ToVector3().Y), Math.Abs(c1.ToVector3().Z - c2.ToVector3().Z));
            Color color = new Color(c1.ToVector3() + noiseGen.interpolatedNoise(pos.X*0.3f, pos.Z*0.3f)*diff);
            return color;
        }


        //to use the terraindata
        public Vector3 RetrieveHeight(Vector3 pos)
        {

            //pos.Y += data.HeightMap.CalculateHeight(pos.X - TerrainPosition.X, pos.Z - TerrainPosition.Z);
            return pos;
        }
        public void SetUpTerrain(Vector3 corner1, Vector3 corner2)
        {
            throw new NotImplementedException();

           // TerrainPosition = corner1;
           // float lenght = corner2.Z - corner1.Z;
           // float with = corner2.X - corner1.X;
           // int blocksize = 20;
           // Database database = loadDatabaseServices();
           // TerrainManagerService tms = new TerrainManagerService(database);
           // TaggedTerrain taggedTerrain = tms.CreateTerrain();


           // data = null;


           // data = taggedTerrain.GetFullData();
           // data.NumBlocksX = (int)(with / blocksize);
           // data.NumBlocksZ = (int)(lenght / blocksize);
           // data.BlockSize = blocksize;
           // data.SizeX = data.NumBlocksX * data.BlockSize;
           // data.SizeZ = data.NumBlocksZ * data.BlockSize;
           // data.Position = TerrainPosition;//new Vector3(-data.BlockSize * (data.NumBlocksX / 2), 4, -data.BlockSize * (data.NumBlocksZ / 2));
           //// data.HeightMap = new TerrainHeightMap(data.NumBlocksX * data.BlockSize, data.NumBlocksZ * data.BlockSize);

           // TerrainRaiseTool.RaiseTerrain(data, 300, 300, 60, 100);
           // TerrainRaiseTool.RaiseTerrain(data, 0, 600, 120, 30);
           // TerrainRaiseTool.RaiseTerrain(data, 600, 0, 60, 400);
           // TerrainRaiseTool.RaiseTerrain(data, 78, 112, 18, -5);
           // TerrainRaiseTool.RaiseTerrain(data, -7, 50, 50, 30);
           // TerrainRaiseTool.RaiseTerrain(data, 0, 0, 300, 10);
           // TerrainRaiseTool.RaiseTerrain(data, 0, 0, 100, 40);
           // TerrainRaiseTool.RaiseTerrain(data, 500, 500, 100, 60);



        }
        private Database loadDatabaseServices()
        {
            throw new NotImplementedException();
            //Database database = new Database();
            //database.AddService(new DiskSerializerService(database, Application.StartupPath + "\\WizardsEditorSave"));
            //database.AddService(new DiskLoaderService(database));
            //database.AddService(new SettingsService(database, Application.StartupPath + "\\Settings.xml"));
            //database.AddService(new UniqueIDService(database));

            //return database;
        }
        private void LoadHeightData()
        {
            int texWidth = GrassMap.XnaTexture.Width;
            int texHeight = GrassMap.XnaTexture.Height;

            GrassColors = new Color[texWidth * texHeight];
            GrassMap.XnaTexture.GetData(GrassColors);

            DensityData = new Vector3[texWidth, texHeight];
            for (int x = 0; x < texWidth; x++)
                for (int y = 0; y < texHeight; y++)
                    DensityData[x, y] = new Vector3(GrassColors[x + y * texWidth].R * densityDivider, GrassColors[x + y * texWidth].G * densityDivider, GrassColors[x + y * texWidth].B) * densityDivider;
        }
        private Vector3 getMean(int texWidth, int texLength)
        {
            Vector3 meanColor = Vector3.Zero;
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texLength; y++)
                {
                    meanColor.X += DensityData[x, y].X;
                    meanColor.Y += DensityData[x, y].Y;
                    meanColor.Z += DensityData[x, y].Z;
                }


            }
            return (meanColor / DensityData.Length);
        }


        public static void TestGrass()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.SpectaterCamera.FarClip = 100000;

            GrassMesh mesh = new GrassMesh();
            //mesh.vertices = mesh.CreateGrassPlain(Vector3.Zero, new Vector2(80, 80), 20, new Vector2(1,0.8f));



            game.InitializeEvent +=
                delegate
                {
                    mesh.Initialize(game, true, true);
                    mesh.CreateGrass(Vector3.Zero, new Vector3(50, 0, 50), new Vector2(1, 0.8f), new Vector2(1.2f, 1f), new Vector2(1.2f, 1f));
                    mesh.SetRenderData();
                };
            game.DrawEvent +=
                delegate
                {

                    mesh.Render();

                };
            game.Run();
        }

        public static void TestGrassNewDesign()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.SpectaterCamera.FarClip = 100000;

            GrassMesh mesh = new GrassMesh();
            



            game.InitializeEvent +=
                delegate
                {
                    mesh.InitializeNewDesign(game);
                    mesh.CreateGrassNewDesign(Vector3.Zero, new Vector3(50, 0, 50), new Vector2(1, 1));
                    //mesh.CreateStarOneNewDesign(Vector3.Zero, new Vector2(1, 1), 4, Color.White, 0);
                    //mesh.CreateStarOneNewDesign(Vector3.UnitX, new Vector2(1, 1), 6,0);
                    //mesh.CreateStarTwoNewDesign(new Vector3(3, 0, 3), new Vector2(1, 1), 0);
                    mesh.SetRenderDataNew();
                };
            game.DrawEvent +=
                delegate
                {

                    mesh.RenderNewDesign();
                    //for (int i = 0; i < mesh.triangleCountNew; i++)
                    //{
                    //    game.LineManager3D.AddTriangle(mesh.grassVertices[i * 3].Position, mesh.grassVertices[i * 3 + 1].Position, mesh.grassVertices[i * 3 + 2].Position, Color.Red);
                    //}
                };
            game.Run();
        }

        public void TestGrassMesh()
        {
            Grass.GrassMesh grass = new TreeGenerator.Grass.GrassMesh();
            //grass.vertices = grass.CreateStarOne(MeshPosition, new Vector2(2, 2), 5);
            //grass.Initialize(game);
            grass.shader1.SetParameter("viewProjection", game.Camera.ViewProjection);
            grass.shader1.SetParameter("time", 0.0f);
            grass.Render();

        }
        public void TestGrassMesh(Matrix viewProjection)
        {
            Grass.GrassMesh grass = new TreeGenerator.Grass.GrassMesh();
            //grass.vertices = grass.CreateStarOne(MeshPosition, new Vector2(2, 2), 5);
            //grass.Initialize(game);
            grass.shader1.SetParameter("viewProjection", viewProjection);
            grass.shader1.SetParameter("time", 0.0f);
            grass.Render();

        }

    }
}

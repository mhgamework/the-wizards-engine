using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;
using MHGameWork.TheWizards.Terrain;
using MHGameWork.TheWizards.Terrain.Geomipmap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.NoiseGenerater;

namespace MHGameWork.TheWizards.Tests.Terrain
{
    public class SimpleTerrain
    {
        private readonly HeightMap map;
        private readonly int blockSize;
        private readonly int numBlocks;

        private SimpleTerrainBlock[] blocks;
        private TerrainShaderNew shader;
        private Texture2D heightmapTexture;
        private IXNAGame game;
        private IndexBufferBuilder builder;
        private VertexBuffer vb;
        private VertexDeclaration decl;
        private Texture2D normalTexture;

        public SimpleTerrain(HeightMap map, int blockSize, int numBlocks)
        {
            this.map = map;
            this.blockSize = blockSize;
            this.numBlocks = numBlocks;



            blocks = new SimpleTerrainBlock[numBlocks * numBlocks];
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = new SimpleTerrainBlock();

            }

        }

        public void Draw()
        {

            shader.Shader.SetParameter("viewProjection", game.Camera.ViewProjection);
            shader.Shader.SetParameter("maxHeight", 200);
            shader.Shader.SetParameter("displacementMap", heightmapTexture);
            shader.Shader.SetParameter("normalMap", normalTexture);
            shader.Shader.SetParameter("heightMapSize", heightmapTexture.Width);
            shader.Shader.SetParameter("lightDir", Vector3.Normalize(new Vector3(1, -1, 1)));

            game.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
            game.GraphicsDevice.VertexDeclaration = decl;

            shader.Shader.SetTechnique("DrawHeightColoredLit");
            for (int x = 0; x < numBlocks; x++)
                for (int z = 0; z < numBlocks; z++)
                {



                    var block = GetBlock(x, z);

                    shader.Shader.SetParameter("world", Matrix.CreateTranslation(x * blockSize, 0, z * blockSize));
                    shader.Shader.SetParameter("heightMapOffset", new Vector2(x * blockSize, z * blockSize));

                    game.GraphicsDevice.Indices = block.IndexBuffer;
                    renderBlock(block);
                }




            game.GraphicsDevice.Indices = null;
        }

        private void renderBlock(SimpleTerrainBlock block)
        {
            shader.Shader.RenderMultipass(delegate
                                          {
                                              if (block.TriangleCount == 0) return;
                                              game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                                        (blockSize + 1) * (blockSize + 1), 0,
                                                                                        block.TriangleCount);
                                          });
        }

        public void Update()
        
        {
            for (int x = 0; x < numBlocks; x++)
                for (int z = 0; z < numBlocks; z++)
                {
                    var block = GetBlock(x, z);

                    var toBlock = game.Camera.ViewInverse.Translation - (new Vector3(x, 0, z) * blockSize +
                                  Vector3.One * blockSize / 2);
                    toBlock.Y = 0;
                    var distSq = (toBlock).LengthSquared();
                    var newLevel = MinDistanceCalculator.DetermineLowestAllowedDetailLevel(block.MinDistancesSquared, distSq, block.DetailLevel, builder.MaxDetailLevel);
                    newLevel = 0;
                    if (newLevel != block.DetailLevel) builder.ChangeDetailLevel(block, newLevel);

                }

        }

        public void Initialize(IXNAGame game)
        {
            this.game = game;
            builder = new IndexBufferBuilder(game);
            builder.BlockSize = blockSize;


            for (int x = 0; x < numBlocks; x++)
                for (int z = 0; z < numBlocks; z++)
                {
                    var block = GetBlock(x, z);
                    block.MinDistancesSquared = MinDistanceCalculator.CalculateMinDistancesSquared(game.Camera.Projection, map,
                                                                      builder.BlockSize, 0, 0);

                    builder.ChangeDetailLevel(block, builder.MaxDetailLevel);

                    block.SetNeightbour(Common.GeoMipMap.TerrainBlockEdge.West, GetBlock(x-1, z ));
                    block.SetNeightbour(Common.GeoMipMap.TerrainBlockEdge.East, GetBlock(x+1, z ));
                    block.SetNeightbour(Common.GeoMipMap.TerrainBlockEdge.North, GetBlock(x , z-1));
                    block.SetNeightbour(Common.GeoMipMap.TerrainBlockEdge.South, GetBlock(x , z+1));

                }

            shader = new TerrainShaderNew(game, new EffectPool());

            var vertices = new VertexMultitextured[(builder.BlockSize + 1) * (builder.BlockSize + 1)];
            var texels = new float[map.Width * map.Length];
            var normals = new Color[map.Width * map.Length];




            for (int i = 0; i < builder.BlockSize + 1; i++)
                for (int j = 0; j < builder.BlockSize + 1; j++)
                {
                    var vIndex = builder.IndexFromCoords(i, j);
                    vertices[vIndex].Position = new Vector3(i, 0, j);
                    vertices[vIndex].TextureCoordinate = new Vector2(i, j);

                }

            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Length; j++)
                {
                    var height = map.GetHeight(i, j);

                    texels[i * (map.Width) + j] = height;
                }


            vb = new VertexBuffer(game.GraphicsDevice, typeof(VertexMultitextured),
                                  vertices.Length, BufferUsage.None);
            vb.SetData(vertices);

            decl = new VertexDeclaration(game.GraphicsDevice,
                                         VertexMultitextured.VertexElements);

            heightmapTexture = new Texture2D(game.GraphicsDevice, map.Width, map.Length, 1,
                                             TextureUsage.None, SurfaceFormat.Single);
            heightmapTexture.SetData(texels);





            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Length; j++)
                {
                    normals[i * (map.Width) + j] = new Color(new Vector4(
                                  HeightmapNormalCalculator.CalculateAveragedNormal(map, i, j), 1));
                }
            normalTexture = new Texture2D(game.GraphicsDevice, map.Width, map.Length, 1,
                                          TextureUsage.None, SurfaceFormat.Color);
            normalTexture.SetData(normals);



        }

        public SimpleTerrainBlock GetBlock(int x, int z)
        {
            if (x < 0) return null;
            if (z < 0) return null;
            if (x > numBlocks - 1) return null;
            if (z > numBlocks - 1) return null;
            return blocks[getBlockIndex(x, z)];
        }

        private int getBlockIndex(int x, int z)
        {
            return x * numBlocks + z;
        }
    }
}

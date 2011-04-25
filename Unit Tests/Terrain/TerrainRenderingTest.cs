using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;
using MHGameWork.TheWizards.Terrain.Geomipmap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using TreeGenerator.NoiseGenerater;

namespace MHGameWork.TheWizards.Tests.Terrain
{
    [TestFixture]
    public class TerrainRenderingTest
    {
        [Test]
        public void TestCreateIndices()
        {
            var game = new XNAGame();
            var block = new SimpleTerrainBlock();
            var builder = new IndexBufferBuilder(game);
            builder.BlockSize = 16;
            VertexBuffer vb = null;
            VertexDeclaration decl = null;
            var vertices = new VertexMultitextured[(builder.BlockSize + 1) * (builder.BlockSize + 1)];
            for (int i = 0; i < builder.BlockSize + 1; i++)
            {
                for (int j = 0; j < builder.BlockSize + 1; j++)
                {
                    vertices[builder.IndexFromCoords(i, j)].Position = new Vector3(
                        i, 0, j);
                }
            }


            TerrainShaderNew shader = null;

            game.InitializeEvent += delegate
                                        {
                                            shader = new TerrainShaderNew(game, new EffectPool());
                                            builder.ChangeDetailLevel(block, 0);

                                            vb = new VertexBuffer(game.GraphicsDevice, typeof(VertexMultitextured),
                                                                      vertices.Length, BufferUsage.None);
                                            vb.SetData(vertices);

                                            decl = new VertexDeclaration(game.GraphicsDevice,
                                                                             VertexMultitextured.VertexElements);



                                        };

            game.UpdateEvent += delegate
                                    {
                                        if (game.Keyboard.IsKeyPressed(Keys.Add))
                                        {
                                            if (block.DetailLevel < builder.MaxDetailLevel)
                                                builder.ChangeDetailLevel(block, block.DetailLevel + 1);
                                        }
                                        else if (game.Keyboard.IsKeyPressed(Keys.Subtract))
                                        {
                                            if (block.DetailLevel > 0)
                                                builder.ChangeDetailLevel(block, block.DetailLevel - 1);

                                        }
                                    };
            game.DrawEvent += delegate
                                  {
                                      game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                                      shader.Shader.SetTechnique("DrawTexturedPreprocessed");
                                      shader.Shader.SetParameter("world", Matrix.Identity);
                                      shader.Shader.SetParameter("viewProjection", game.Camera.ViewProjection);



                                      shader.Shader.RenderMultipass(delegate
                                      {
                                          game.GraphicsDevice.Indices = block.IndexBuffer;
                                          game.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
                                          game.GraphicsDevice.VertexDeclaration = decl;
                                          game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                                    vertices.Length, 0,
                                                                                    block.TriangleCount);
                                      });


                                      game.GraphicsDevice.Indices = null;
                                  };


            game.Run();
        }

        [Test]
        public void TestRenderFromHeightmap()
        {
            var game = new XNAGame();
            var block = new SimpleTerrainBlock();
            var builder = new IndexBufferBuilder(game);
            builder.BlockSize = 16;
            VertexBuffer vb = null;
            VertexDeclaration decl = null;

            var noise = new PerlinNoiseGenerater();



            var vertices = new VertexMultitextured[(builder.BlockSize + 1) * (builder.BlockSize + 1)];
            var texels = new float[vertices.Length];
            for (int i = 0; i < builder.BlockSize + 1; i++)
            {
                for (int j = 0; j < builder.BlockSize + 1; j++)
                {
                    var vIndex = builder.IndexFromCoords(i, j);
                    vertices[vIndex].Position = new Vector3(
                        i, noise.interpolatedNoise(i, j) * 2, j);
                    vertices[vIndex].TextureCoordinate = new Vector2(i, j);

                    if (j == 3 && i == 3) vertices[vIndex].Position.Y = 5;
                    if (j == 0 || i == 0) vertices[vIndex].Position.Y = -5;

                    texels[i * (builder.BlockSize + 1) + j] = vertices[vIndex].Position.Y;
                }
            }





            TerrainShaderNew shader = null;
            Texture2D heightmap = null;

            game.InitializeEvent += delegate
            {
                shader = new TerrainShaderNew(game, new EffectPool());
                builder.ChangeDetailLevel(block, 0);

                vb = new VertexBuffer(game.GraphicsDevice, typeof(VertexMultitextured),
                                          vertices.Length, BufferUsage.None);
                vb.SetData(vertices);

                decl = new VertexDeclaration(game.GraphicsDevice,
                                                 VertexMultitextured.VertexElements);

                heightmap = new Texture2D(game.GraphicsDevice, builder.BlockSize + 1, builder.BlockSize + 1, 1,
                                          TextureUsage.None, SurfaceFormat.Single);
                heightmap.SetData(texels);



            };

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Keys.Add))
                {
                    if (block.DetailLevel < builder.MaxDetailLevel)
                        builder.ChangeDetailLevel(block, block.DetailLevel + 1);
                }
                else if (game.Keyboard.IsKeyPressed(Keys.Subtract))
                {
                    if (block.DetailLevel > 0)
                        builder.ChangeDetailLevel(block, block.DetailLevel - 1);

                }
            };
            game.DrawEvent += delegate
            {

                game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                shader.Shader.SetParameter("world", Matrix.Identity);
                shader.Shader.SetParameter("viewProjection", game.Camera.ViewProjection);
                shader.Shader.SetParameter("maxHeight", 2);
                shader.Shader.SetParameter("displacementMap", heightmap);
                shader.Shader.SetParameter("heightMapSize", heightmap.Width);

                game.GraphicsDevice.Indices = block.IndexBuffer;
                game.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
                game.GraphicsDevice.VertexDeclaration = decl;

                shader.Shader.SetParameter("heightMapOffset", new Vector2(0, 0));

                shader.Shader.SetTechnique("DrawTexturedPreprocessed");
                shader.Shader.RenderMultipass(delegate
                {
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                              vertices.Length, 0,
                                                              block.TriangleCount);
                });

                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18));

                shader.Shader.SetTechnique("DrawHeightColored");
                shader.Shader.RenderMultipass(delegate
                {
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                              vertices.Length, 0,
                                                              block.TriangleCount);
                });

                shader.Shader.SetParameter("heightMapOffset", new Vector2(1, 1));
                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 34));

                shader.Shader.RenderMultipass(delegate
                {
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                              vertices.Length, 0,
                                                              block.TriangleCount);
                });

                game.GraphicsDevice.Indices = null;
            };


            game.Run();
        }
    }
}

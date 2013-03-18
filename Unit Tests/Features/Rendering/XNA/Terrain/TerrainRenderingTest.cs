using System;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;
using MHGameWork.TheWizards.Terrain;
using MHGameWork.TheWizards.Terrain.Geomipmap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using TreeGenerator.NoiseGenerater;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Terrain
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

                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18 * 1));

                shader.Shader.SetTechnique("DrawHeightColored");
                shader.Shader.RenderMultipass(delegate
                {
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                              vertices.Length, 0,
                                                              block.TriangleCount);
                });

                shader.Shader.SetParameter("heightMapOffset", new Vector2(1, 1));
                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18 * 2));

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

        [Test]
        public void TestNormals()
        {
            var game = new XNAGame();
            var block = new SimpleTerrainBlock();
            var builder = new IndexBufferBuilder(game);
            builder.BlockSize = 16;
            VertexBuffer vb = null;
            VertexDeclaration decl = null;

            var noise = new PerlinNoiseGenerater();

            var heightmap = new HeightMap(17, 17);


            var vertices = new VertexMultitextured[(builder.BlockSize + 1) * (builder.BlockSize + 1)];
            var texels = new float[vertices.Length];
            var normals = new Color[vertices.Length];
            for (int i = 0; i < builder.BlockSize + 1; i++)
                for (int j = 0; j < builder.BlockSize + 1; j++)
                {
                    var height = noise.interpolatedNoise(i, j) * 2 + 5;
                    if (j == 3 && i == 3) height = 10;
                    if (j == 0 || i == 0) height = 0;
                    heightmap.SetHeight(i, j, height);



                }

            for (int i = 0; i < builder.BlockSize + 1; i++)
                for (int j = 0; j < builder.BlockSize + 1; j++)
                {
                    var height = heightmap.GetHeight(i, j);
                    var vIndex = builder.IndexFromCoords(i, j);
                    vertices[vIndex].Position = new Vector3(i, height, j);
                    vertices[vIndex].TextureCoordinate = new Vector2(i, j);



                    texels[i * (builder.BlockSize + 1) + j] = height;
                    normals[i * (builder.BlockSize + 1) + j] = new Color(new Vector4(
                        HeightmapNormalCalculator.CalculateAveragedNormal(heightmap, i, j), 1));

                }




            TerrainShaderNew shader = null;
            Texture2D heightmapTexture = null;
            Texture2D normalTexture = null;

            game.InitializeEvent += delegate
            {
                shader = new TerrainShaderNew(game, new EffectPool());
                builder.ChangeDetailLevel(block, 0);

                vb = new VertexBuffer(game.GraphicsDevice, typeof(VertexMultitextured),
                                          vertices.Length, BufferUsage.None);
                vb.SetData(vertices);

                decl = new VertexDeclaration(game.GraphicsDevice,
                                                 VertexMultitextured.VertexElements);

                heightmapTexture = new Texture2D(game.GraphicsDevice, builder.BlockSize + 1, builder.BlockSize + 1, 1,
                                          TextureUsage.None, SurfaceFormat.Single);
                heightmapTexture.SetData(texels);

                normalTexture = new Texture2D(game.GraphicsDevice, builder.BlockSize + 1, builder.BlockSize + 1, 1,
                                       TextureUsage.None, SurfaceFormat.Color);
                normalTexture.SetData(normals);


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


                shader.Shader.SetParameter("world", Matrix.Identity);
                shader.Shader.SetParameter("viewProjection", game.Camera.ViewProjection);
                shader.Shader.SetParameter("maxHeight", 10);
                shader.Shader.SetParameter("displacementMap", heightmapTexture);
                shader.Shader.SetParameter("normalMap", normalTexture);
                shader.Shader.SetParameter("heightMapSize", heightmapTexture.Width);
                shader.Shader.SetParameter("lightDir", Vector3.Normalize(new Vector3(1, -1, 1)));

                game.GraphicsDevice.Indices = block.IndexBuffer;
                game.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
                game.GraphicsDevice.VertexDeclaration = decl;

                shader.Shader.SetParameter("heightMapOffset", new Vector2(0, 0));


                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18 * 0));

                shader.Shader.SetTechnique("DrawHeightColoredLit");
                shader.Shader.RenderMultipass(delegate
                {
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                              vertices.Length, 0,
                                                              block.TriangleCount);
                });

                shader.Shader.SetParameter("heightMapOffset", new Vector2(1, 1));
                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18 * 1));

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

        [Test]
        public void TestMinDistances()
        {
            var game = new XNAGame();
            var block = new SimpleTerrainBlock();
            var builder = new IndexBufferBuilder(game);
            builder.BlockSize = 16;
            VertexBuffer vb = null;
            VertexDeclaration decl = null;

            var blockSize = builder.BlockSize;


            var vertices = new VertexMultitextured[(builder.BlockSize + 1) * (builder.BlockSize + 1)];
            var texels = new float[vertices.Length];

            var heightmap = new HeightMap(17, 17);

            var noise = new PerlinNoiseGenerater();

            for (int i = 0; i < blockSize + 1; i++)
                for (int j = 0; j < blockSize + 1; j++)
                {
                    var height = noise.interpolatedNoise(i, j) * 2;
                    heightmap.SetHeight(i, j, height);



                }

            for (int i = 0; i < builder.BlockSize + 1; i++)
                for (int j = 0; j < builder.BlockSize + 1; j++)
                {
                    var height = heightmap.GetHeight(i, j);
                    var vIndex = builder.IndexFromCoords(i, j);
                    vertices[vIndex].Position = new Vector3(i, height, j);
                    vertices[vIndex].TextureCoordinate = new Vector2(i, j);



                    texels[i * (builder.BlockSize + 1) + j] = height;

                }

            var minDistancesSq = MinDistanceCalculator.CalculateMinDistancesSquared(game.Camera.Projection, heightmap,
                                                                           builder.BlockSize, 0, 0);
            game.SpectaterCamera.FarClip = 5000;

            TerrainShaderNew shader = null;
            Texture2D heightmapTexture = null;

            game.InitializeEvent += delegate
            {
                shader = new TerrainShaderNew(game, new EffectPool());
                builder.ChangeDetailLevel(block, 0);

                vb = new VertexBuffer(game.GraphicsDevice, typeof(VertexMultitextured),
                                          vertices.Length, BufferUsage.None);
                vb.SetData(vertices);

                decl = new VertexDeclaration(game.GraphicsDevice,
                                                 VertexMultitextured.VertexElements);

                heightmapTexture = new Texture2D(game.GraphicsDevice, builder.BlockSize + 1, builder.BlockSize + 1, 1,
                                          TextureUsage.None, SurfaceFormat.Single);
                heightmapTexture.SetData(texels);



            };

            game.UpdateEvent += delegate
                                {
                                    var distSq = game.Camera.ViewInverse.Translation.LengthSquared();
                                    var newLevel = MinDistanceCalculator.DetermineLowestAllowedDetailLevel(minDistancesSq, distSq, block.DetailLevel, builder.MaxDetailLevel);
                                    if (newLevel != block.DetailLevel) builder.ChangeDetailLevel(block, newLevel);
                                };
            game.DrawEvent += delegate
            {


                shader.Shader.SetParameter("world", Matrix.Identity);
                shader.Shader.SetParameter("viewProjection", game.Camera.ViewProjection);
                shader.Shader.SetParameter("maxHeight", 1);
                shader.Shader.SetParameter("displacementMap", heightmapTexture);
                shader.Shader.SetParameter("heightMapSize", heightmapTexture.Width);
                shader.Shader.SetParameter("lightDir", Vector3.Normalize(new Vector3(1, -1, 1)));

                game.GraphicsDevice.Indices = block.IndexBuffer;
                game.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
                game.GraphicsDevice.VertexDeclaration = decl;

                shader.Shader.SetParameter("heightMapOffset", new Vector2(0, 0));

                game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18 * 0));

                shader.Shader.SetTechnique("DrawHeightColored");
                shader.Shader.RenderMultipass(delegate
                {
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                              vertices.Length, 0,
                                                              block.TriangleCount);
                });

                game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                shader.Shader.SetParameter("world", Matrix.CreateTranslation(Vector3.Right * 18 * 1));

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

        [Test]
        public void TestSimpleGeomipmap()
        {
            var game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            var map = createTestHeightmap(128, 16);
            var terrain = new SimpleTerrain(map, 128, 16);
            Vector3 lightDir = Vector3.Normalize(new Vector3(1, -1, 1));
            game.SpectaterCamera.FarClip = 5000;

            game.InitializeEvent += delegate { terrain.Initialize(game); };
            game.UpdateEvent += delegate
            {
                terrain.Update();
                if (game.Keyboard.IsKeyPressed(Keys.W))
                {
                    if (game.GraphicsDevice.RenderState.FillMode == FillMode.WireFrame)
                        game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                    else
                        game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

                }
                lightDir = temp(terrain, game, lightDir);

            };
            game.DrawEvent += delegate
                              {
                                  terrain.Draw();
                              };

            game.Run();

        }

        private Vector3 temp(SimpleTerrain terrain, XNAGame game, Vector3 lightDir)
        {
            if (game.Keyboard.IsKeyDown(Keys.L))
            {
                var mat = Matrix.CreateRotationX(game.Mouse.RelativeY*0.1f)*
                          Matrix.CreateRotationY(game.Mouse.RelativeX*0.1f);
                lightDir = Vector3.Transform(lightDir, mat);
                terrain.LightDirection = lightDir;
                game.SpectaterCamera.Enabled = false;
            }
            else
            {
                game.SpectaterCamera.Enabled = true;
            }
            return lightDir;
        }

        private HeightMap createTestHeightmap(int blockSize, int size)
        {
            throw new NotImplementedException("Todo:fix this test");
            //var heightmap = new HeightMap(blockSize * size + 1, blockSize * size + 1);

            //var noise = new PerlinNoiseGenerater();

            //for (int i = 0; i < heightmap.Width; i++)
            //    for (int j = 0; j < heightmap.Length; j++)
            //    {
            //        var height = 0f;
            //        float freq, ampl;
            //        freq = 0.002f*3; ampl = 900; height += noise.interpolatedNoise(i * freq, j * freq) * ampl; noise.CreateRandomOffset();
            //        freq = 0.01f*3; ampl = 100; height += noise.interpolatedNoise(i * freq, j * freq) * ampl; noise.CreateRandomOffset();
            //        freq = 0.1f*3; ampl = 15; height += noise.interpolatedNoise(i * freq, j * freq) * ampl; noise.CreateRandomOffset();
            //        freq = 1f*3; ampl = 0.3f; height += noise.interpolatedNoise(i * freq, j * freq) * ampl; noise.CreateRandomOffset();
            //        //freq = 2f; ampl = 0.2f; height += noise.interpolatedNoise(i * freq, j * freq) * ampl;

            //        heightmap.SetHeight(i, j, height);



            //    }
            //return heightmap;
        }
    }
}

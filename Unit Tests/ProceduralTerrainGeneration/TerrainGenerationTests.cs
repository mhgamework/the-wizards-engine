using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.NoiseGenerater;
using NUnit.Framework;

namespace TreeGenerator.TerrrainGeneration
{
    [TestFixture]
    public class TerrainGenerationTests
    {
        [Test]
        public void HeigthMapTest()
        {
            XNAGame game = new XNAGame();
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 15f;
            int width = 256;
            int length = 256;
            HeigthMapGenerater heigthmap = new HeigthMapGenerater();
            Texture2D texture = null;
            float[,] heigthValues;
            game.InitializeEvent +=
                delegate
                {

                    heigthValues = new float[width, length];
                    noise.NumberOfOctaves = 8;
                    noise.persistance = 0.0005f;
                    float max = 0;
                    float min = 0;
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            heigthValues[i, j] = noise.CombinedFractalBrowningAndRidgedMF(i * 5, j * 5, 8, 4, 4, 0.3f, 0.2f, 1.2f, 0.8f);
                            if (heigthValues[i, j] < min)
                                min = heigthValues[i, j];
                            if (heigthValues[i, j] > max)
                                max = heigthValues[i, j];
                        }
                    }

                    texture = heigthmap.CreateTexture(width, length, heigthValues, max - min, game.GraphicsDevice);

                };

            game.DrawEvent +=
                delegate
                {
                    game.SpriteBatch.Begin();
                    game.SpriteBatch.Draw(texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                    game.SpriteBatch.End();

                };
            game.Run();

        }
        [Test]
        public void ErrosionTest()
        {
            XNAGame game = new XNAGame();
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 100;
            int height = 100;
            SimpleTerrain terrain;
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            for (int i = 0; i < (int)(width); i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (noise.GetPerlineNoise(i, j, 8, 0.1f, 0.8f, 0.8f) * 0.8f + noise.GetPerlineNoise(noise.Perturb(i, j, 0.1f, 30).X, noise.Perturb(i, j, 0.1f, 30).Y, 4, 0.2f, 0.5f, 0.5f) * 0.25f) * 60;

                }
            }
            gen.GenerateErosion(heightData, 100, 20.0f);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));
                    colors.Add(Color.White);
                }
            }

            terrain = new SimpleTerrain(game, positions, colors, width, height);
            game.InitializeEvent +=
                delegate
                {

                    terrain.CreateRenderData();

                };
            bool changed = false;
            game.UpdateEvent +=
                delegate
                {

                };
            game.DrawEvent +=
                delegate
                {
                    terrain.Render();

                };
            game.Run();

        }
        [Test]
        public void HydrolicErrosionTest()
        {
            XNAGame game = new XNAGame();
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> positionsbase = new List<Vector3>();
            List<Vector3> positionsbaseDifference = new List<Vector3>();

            List<Color> colors = new List<Color>();
            List<Color> colorsbase = new List<Color>();

            int width = 100;
            int height = 100;
            SimpleTerrain terrain;
            SimpleTerrain terrainbase;
            SimpleTerrain terrainbaseDiffernce;


            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float[,] heightDataErrosion = new float[width, height];
            //float[,] heightDataErrosionDiffernce = new float[width, height];



            for (int i = 0; i < (int)(width); i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (noise.GetPerlineNoise(i, j, 8, 0.1f, 0.8f, 0.8f) * 0.8f + noise.GetPerlineNoise(noise.Perturb(i, j, 0.1f, 30).X, noise.Perturb(i, j, 0.1f, 30).Y, 4, 0.2f, 0.5f, 0.5f) * 0.25f) * 70;

                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positionsbase.Add(new Vector3(i, heightData[i, j], j));
                    colorsbase.Add(Color.White);
                }
            }
            //heightDataErrosionDiffernce = heightData;
            heightDataErrosion = gen.GenerateHydrolicErrosion(heightData, 50 * width * height, width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightDataErrosion[i, j], j));
                    colors.Add(new Color(new Vector3(positions[i * width + j].Y - positionsbase[i * width + j].Y, 50, 50)));
                    //positionsbaseDifference.Add(new Vector3(i, positions[i * width + j].Y - positionsbase[i * width + j].Y, j));
                }
            }

            terrainbase = new SimpleTerrain(game, positionsbase, colorsbase, width, height, Matrix.CreateTranslation(Vector3.Left * (width + 10)));
            terrain = new SimpleTerrain(game, positions, colors, width, height);
            //terrainbaseDiffernce = new SimpleTerrain(game, positionsbaseDifference, colorsbase, width, height, Matrix.CreateTranslation(Vector3.Left*60));
            game.InitializeEvent +=
                delegate
                {

                    terrain.CreateRenderData();
                    terrainbase.CreateRenderData();
                    //terrainbaseDiffernce.CreateRenderData();
                };
            bool changed = false;
            game.UpdateEvent +=
                delegate
                {

                };
            game.DrawEvent +=
                delegate
                {
                    terrain.Render();
                    terrainbase.Render();
                    //terrainbaseDiffernce.Render();
                };
            game.Run();

        }
        [Test]
        public void HydrolicErrosionMultipleTest()
        {
            XNAGame game = new XNAGame();
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();

            List<Color> colors = new List<Color>();


            int width = 50;
            int height = 50;
            List<SimpleTerrain> terrains = new List<SimpleTerrain>();



            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float[,] heightDataErrosion = new float[width, height];
            for (int i = 0; i < (int)(width); i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (noise.GetPerlineNoise(i, j, 8, 0.1f, 0.8f, 0.8f) * 0.8f + noise.GetPerlineNoise(noise.Perturb(i, j, 0.1f, 30).X, noise.Perturb(i, j, 0.1f, 30).Y, 4, 0.2f, 0.5f, 0.5f) * 0.25f) * 70;

                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));
                    colors.Add(Color.White);
                }
            }
            terrains.Add(new SimpleTerrain(game, positions, colors, width, height));
            for (int k = 1; k < 5; k++)
            {

                for (int l = 1; l < 5; l++)
                {
                    positions = new List<Vector3>();
                    colors = new List<Color>();
                    heightDataErrosion = gen.GenerateHydrolicErrosion(heightData, ((int)Math.Pow(l, 2)) * width * height, k, width, height);
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            positions.Add(new Vector3(i, heightDataErrosion[i, j], j));
                            colors.Add(new Color((byte)(k * 50), 50, (byte)(l * 50)));
                        }
                    }
                    terrains.Add(new SimpleTerrain(game, positions, colors, width, height, Matrix.CreateTranslation(new Vector3(k * width + 5, 0, l * height + 5))));
                }
            }

            game.InitializeEvent +=
                delegate
                {

                    for (int i = 0; i < terrains.Count; i++)
                    {
                        terrains[i].CreateRenderData();
                    }
                };
            bool changed = false;
            game.UpdateEvent +=
                delegate
                {

                };
            game.DrawEvent +=
                delegate
                {
                    for (int i = 0; i < terrains.Count; i++)
                    {
                        terrains[i].Render();
                    }
                };
            game.Run();
        }
        [Test]
        public void IslandTest()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.FarClip = 10000000f;
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 10f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Vector3> positionsIsland = new List<Vector3>();
            List<Color> colorsIsland = new List<Color>();
            List<Vector3> positionsFinal = new List<Vector3>();
            List<Color> colorsFinal = new List<Color>();
            int width = 500;
            int height = 500;
            float lengthFactor = 0.2f;
            SimpleTerrain terrain;
            SimpleTerrain terrainIsland;
            SimpleTerrain terrainFinal;

            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float heigestFBM = 0;
            float heigestRidge = 0;

            float lowestIsland = 0;
            float[,] heightData = new float[width, height];
            float[,] heightDataRidge = new float[width, height];
            float[,] heightDataFBM = new float[width, height];
            float[,] heightDataIsland = new float[width, height];
            float[,] heightDataFinal = new float[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //heightData[i, j] = (noise.CombinedFractalBrowningAndRidgedMF(i * lengthFactor, j * lengthFactor, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f) * 0.55f + gen.IslandFactor(i * lengthFactor, j * lengthFactor, new Vector2(width * lengthFactor * 0.5f, height * lengthFactor * 0.5f), width * lengthFactor * 0.45f, width * lengthFactor * 0.22f) * 0.65f) * AmplitudeFactor;//noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f)*0.1f+gen.IslandFactor(i, j, new Vector2(width * 0.45f, height * 0.5f),0,width*0.22f)*0.9f  ;

                    heightDataRidge[i, j] = noise.RidgedMF(i * lengthFactor, j * lengthFactor, 0.15f, 8, 2f, 0.7f, 0.8f);//noise.CombinedFractalBrowningAndRidgedMF(i * lengthFactor, j * lengthFactor, 8, 4,5f, 0.5f, 2f, 0.8f, 1f);
                    heightDataFBM[i, j] = noise.GetFractalBrowningNoise(i * lengthFactor, j * lengthFactor, 8, 1.2f, 1.9f, 1.2f);
                    heightDataIsland[i, j] = gen.IslandFactor(i * lengthFactor, j * lengthFactor, new Vector2(width * lengthFactor * 0.5f, height * lengthFactor * 0.5f), width * lengthFactor * 0.42f, width * lengthFactor * 0.4f);//noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f)*0.1f+gen.IslandFactor(i, j, new Vector2(width * 0.45f, height * 0.5f),0,width*0.22f)*0.9f  ;

                    if (heigestFBM < heightDataFBM[i, j])
                    {
                        heigestFBM = heightDataFBM[i, j];
                    }
                    if (heigestRidge < heightDataRidge[i, j])
                    {
                        heigestRidge = heightDataRidge[i, j];
                    }

                    if (lowestIsland > heightDataIsland[i, j])
                    {
                        lowestIsland = heightDataIsland[i, j];
                    }
                }
            }
            //float diff = heigest - lowest;
            //float diffIsland = heigestIsland - lowestIsland;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (heightDataIsland[i, j] < 0)
                    {

                        heightDataIsland[i, j] = -(heightDataIsland[i, j] / lowestIsland);
                    }
                    heightDataFBM[i, j] = ((heightDataFBM[i, j] / heigestFBM) * (5) + 0.1f);
                    heightDataRidge[i, j] = ((heightDataRidge[i, j] / heigestRidge) * (3) + 0.1f);

                    heightData[i, j] = heightDataFBM[i, j] * heightDataRidge[i, j];
                    heightDataFinal[i, j] = heightData[i, j] * heightDataIsland[i, j] * scale;
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j] * scale, j));
                    colors.Add(Color.LightGreen);
                    positionsIsland.Add(new Vector3(i, heightDataIsland[i, j] * scale, j));
                    if (heightDataIsland[i, j] > 0)
                    {
                        colorsIsland.Add(Color.LightGreen);
                    }
                    else
                    {
                        colorsIsland.Add(Color.Blue);
                    }
                    positionsFinal.Add(new Vector3(i, heightDataFinal[i, j], j));
                    if (heightDataFinal[i, j] > 0)
                    {
                        colorsFinal.Add(Color.LightGreen);
                    }
                    else
                    {
                        colorsFinal.Add(Color.Blue);
                    }
                }
            }
            terrain = new SimpleTerrain(game, positions, colors, width, height, Matrix.CreateTranslation(Vector3.Right * width * 2.1f));
            terrainIsland = new SimpleTerrain(game, positionsIsland, colorsIsland, width, height, Matrix.CreateTranslation(Vector3.Right * width * 1.05f));
            terrainFinal = new SimpleTerrain(game, positionsFinal, colorsFinal, width, height);

            game.InitializeEvent +=
                delegate
                {

                    terrain.CreateRenderData();
                    terrainIsland.CreateRenderData();
                    terrainFinal.CreateRenderData();


                };
            Vector3 ManPosition = new Vector3(width * 0.5f, heightDataFinal[(int)(width * 0.5f), (int)(height * 0.5f)], height * 0.5f);
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        ManPosition = new Vector3(ManPosition.X + 1, heightDataFinal[(int)(ManPosition.X + 1), (int)(ManPosition.Z + 1)], ManPosition.Z + 1);
                    }
                };
            game.DrawEvent +=
                delegate
                {

                    terrain.Render();
                    terrainIsland.Render();
                    terrainFinal.Render();
                    //ManPosition = Vector3.Zero;
                    game.LineManager3D.AddBox(new BoundingBox(Vector3.Zero + ManPosition, new Vector3(0.3f / 8, 1.8f / 8, 0.3f / 8) + ManPosition), Color.Red); ;

                };
            game.Run();

        }
        [Test]
        public void IslandErrosionTest()
        {
            XNAGame game = new XNAGame();
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 200;
            int height = 200;
            SimpleTerrain terrain;
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float heigest = 0;
            float lowest = 0;
            float AmplitudeFactor = 20f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = gen.IslandFactor(i, j, new Vector2(width * 0.5f, height * 0.5f), 1, 50) * noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.3f, 0.2f, 1.2f, 0.8f) * 4f;
                    if (heigest < heightData[i, j])
                    {
                        heigest = heightData[i, j];
                    }
                    if (lowest > heightData[i, j])
                    {
                        lowest = heightData[i, j];
                    }
                }
            }
            float diff = heigest - lowest;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    heightData[i, j] = (((heightData[i, j] - lowest) / (diff)) * 2 - 1) * 20f;
                }
            }

            heightData = gen.IslandFilter(heightData, -10f, 20 * width * height);
            heightData = gen.GenerateHydrolicErrosion(heightData, 50 * width * height, width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));
                    if (heightData[i, j] > 0)
                    {
                        colors.Add(Color.LightGreen);
                    }
                    else
                    {
                        colors.Add(Color.Blue);
                    }

                }
            }

            terrain = new SimpleTerrain(game, positions, colors, width, height);
            game.InitializeEvent +=
                delegate
                {

                    terrain.CreateRenderData();

                };
            bool changed = false;
            game.UpdateEvent +=
                delegate
                {

                };
            game.DrawEvent +=
                delegate
                {
                    terrain.Render();

                };
            game.Run();
        }
        [Test]
        public void RiverSimulationTest()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.FarClip = 10000f;
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 500;
            int height = 500;
            float heightFactor = 50;
            SimpleTerrain terrain;
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float heigest = 0;
            float lowest = 0;
            float AmplitudeFactor = 20f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = gen.IslandFactor(i, j, new Vector2(width * 0.5f, height * 0.5f), 1, 50) * noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.3f, 0.2f, 1.2f, 0.8f) * 4f;
                    if (heigest < heightData[i, j])
                    {
                        heigest = heightData[i, j];
                    }
                    if (lowest > heightData[i, j])
                    {
                        lowest = heightData[i, j];
                    }
                }
            }
            float diff = heigest - lowest;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    heightData[i, j] = (((heightData[i, j] - lowest) / (diff)) * 2 - 1) * heightFactor;
                }
            }


            heightData = gen.IslandFilter(heightData, -10f, 15 * width * height);

            //riverSimulation
            gen.NewWaterSimulation(heightData);


            //terrain setup + coloring
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));
                    if (heightData[i, j] > 0)
                    {
                        colors.Add(Color.LightGreen);
                    }
                    else
                    {
                        colors.Add(Color.Blue);
                    }

                }
            }

            terrain = new SimpleTerrain(game, positions, colors, width, height);
            float[,] waterData;
            game.InitializeEvent +=
                delegate
                {

                    terrain.CreateRenderData();
                    gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.5f), (int)(height * 0.5f)), 0.1f);

                };
            bool changed = false;
            float time = 0f;
            game.UpdateEvent +=
                delegate
                {
                    time += (float)game.GameTime.ElapsedGameTime.TotalSeconds;
                    if (time > 0.01f)
                    {
                        time = 0;
                        gen.WaterCycle();
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                    {
                        gen.AddRandomWaterDrops(200, 1);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        gen.WaterCycle();
                    }
                };
            game.DrawEvent +=
                delegate
                {
                    waterData = gen.GetNewWaterMap();
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (waterData[i, j] > 0.0001f)
                            {
                                //game.LineManager3D.AddLine(new Vector3(i, heightData[i, j], j), new Vector3(i, heightData[i, j] + waterData[i, j], j), Color.Red);
                                game.LineManager3D.AddBox(new BoundingBox(new Vector3(i - 0.5f, heightData[i, j], j - 0.5f), new Vector3(i + 0.5f, heightData[i, j] + waterData[i, j], j + 0.5f)), Color.Blue);
                            }
                        }
                    }
                    terrain.Render();

                };
            game.Run();
        }
        [Test]
        public void RiverSimulationTestNewSystem()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            LineManager3DLines vLines = new LineManager3DLines();
            game.SpectaterCamera.FarClip = 10000f;
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 250;
            int height = 250;
            float heightFactor = 20;
            SimpleTerrain terrain;
            //SimpleTerrain water;
            //List<Vector3> waterPositions = new List<Vector3>();
            //List<Color> waterColors = new List<Color>();            
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float heigest = 0;
            float lowest = 0;
            float AmplitudeFactor = 40f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = gen.IslandFactor(i, j, new Vector2(width * 0.5f, height * 0.5f), 0.85f, 1) * noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.3f, 0.2f, 1.2f, 0.8f) * 4f;
                    if (heigest < heightData[i, j])
                    {
                        heigest = heightData[i, j];
                    }
                    if (lowest > heightData[i, j])
                    {
                        lowest = heightData[i, j];
                    }
                }
            }
            float diff = heigest - lowest;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    heightData[i, j] = (((heightData[i, j] - lowest) / (diff)) * 3 - 1) * heightFactor;// 3=2
                }
            }


            //heightData = gen.IslandFilter(heightData, -10f, 15 * width * height);

            //riverSimulation
            gen.newWaterSystem(heightData, 1, 1f, 0.8f, 1, 0.001f);


            //terrain setup + coloring

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));
                    if (heightData[i, j] > 0)
                    {
                        colors.Add(Color.LightGreen);
                    }
                    else
                    {
                        colors.Add(Color.Blue);
                    }
                    //waterColors.Add(Color.LightBlue);
                    //waterPositions.Add(Vector3.Zero);
                }
            }

            terrain = new SimpleTerrain(game, positions, colors, width, height);
            //water = new SimpleTerrain(game, waterPositions, waterColors, width, height);
            float[,] waterData = new float[width, height]; ;
            game.InitializeEvent +=
                delegate
                {


                    terrain.CreateRenderData();
                    //water.CreateDynamicRenderData(waterPositions, waterColors);
                    gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.5f), (int)(height * 0.5f)), 2f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.2f), (int)(height * 0.2f)), 2f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.8f), (int)(height * 0.8f)), 10f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.2f), (int)(height * 0.7f)), 5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.146f), (int)(height *0.654f)), 5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.687f), (int)(height * 0.9654f)), 5f);



                };
            bool changed = false;
            float time = 0f;
            game.UpdateEvent +=
                delegate
                {
                    time += (float)game.GameTime.ElapsedGameTime.TotalSeconds;
                    if (time > 1f)
                    {
                        time = 0;
                        gen.ComputeOneWaterCycleNew();
                        waterData = gen.GetOldWaterMap();

                        //waterPositions = new List<Vector3>();
                        //for (int i = 0; i < width; i++)
                        //{
                        //    for (int j = 0; j < height; j++)
                        //    {
                        //        waterPositions.Add(new Vector3(i,heightData[i,j]+waterData[i,j],j));

                        //    }
                        //}
                        //water.CreateDynamicRenderData(waterPositions, colors);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                    {
                        gen.AddRandomWaterDrops(10, 1);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        gen.ComputeOneWaterCycleNew();
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.V))
                    {
                        vLines = new LineManager3DLines();
                        gen.ComputeVelocityMap();
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                float heightDat = heightData[i, j] + gen.GetOldWaterMap()[i, j] * 1.2f;
                                vLines.AddLine(new Vector3(i, heightDat, j), new Vector3(i + gen.velocityMap[i, j].X * 30, heightDat, j + gen.velocityMap[i, j].Y * 30), Color.Red);
                            }
                        }
                    }
                };

            game.DrawEvent +=
                delegate
                {
                    LineManager3DLines lines = new LineManager3DLines();
                    for (int i = 0; i < width - 1; i++)
                    {
                        for (int j = 0; j < height - 1; j++)
                        {
                            float height1 = waterData[i, j];
                            if (height1 < 0.01)
                            {
                                height1 = 0;
                            }
                            else
                            {
                                height1 += heightData[i, j];
                            }
                            float height2 = waterData[i + 1, j];
                            if (height2 < 0.01)
                            {
                                height2 = 0;
                            }
                            else
                            {
                                height2 += heightData[i + 1, j];
                            }
                            float height3 = waterData[i, j + 1];
                            if (height3 < 0.01)
                            {
                                height3 = 0;
                            }
                            else
                            {
                                height3 += heightData[i, j + 1];
                            }

                            lines.AddTriangle(new Vector3(i, height1, j), new Vector3(i + 1, height2, j), new Vector3(i, height3, j + 1), Color.Blue);

                            //if (waterData[i, j] > 0.0001f)
                            //{
                            //    //game.LineManager3D.AddLine(new Vector3(i, heightData[i, j], j), new Vector3(i, heightData[i, j] + waterData[i, j], j), Color.Blue);
                            //    //game.LineManager3D..AddBox(new BoundingBox(new Vector3(i - 0.5f, heightData[i, j], j - 0.5f), new Vector3(i + 0.5f, heightData[i, j] + waterData[i, j], j + 0.5f)), Color.Blue);
                            //}
                        }
                    }
                    terrain.Render();
                    game.LineManager3D.Render(lines);
                    game.LineManager3D.Render(vLines);

                    //water.Render();
                };
            game.Run();
        }
        [Test]
        public void RiverSimulationTestNewSystemErrosion()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            LineManager3DLines vLines = new LineManager3DLines();
            game.SpectaterCamera.FarClip = 10000f;
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 200;
            int height = 200;
            float heightFactor = 4;
            SimpleTerrain terrain;
            //SimpleTerrain water;
            //List<Vector3> waterPositions = new List<Vector3>();
            //List<Color> waterColors = new List<Color>();            
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float heigest = 0;
            float lowest = 0;
            float AmplitudeFactor = 40f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f) * gen.IslandFactor(i, j, new Vector2(width * 0.5f, height * 0.5f), 0.95f, 0.1f);
                    if (heigest < heightData[i, j])
                    {
                        heigest = heightData[i, j];
                    }
                    if (lowest > heightData[i, j])
                    {
                        lowest = heightData[i, j];
                    }
                }
            }
            float diff = heigest - lowest;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    heightData[i, j] = (((heightData[i, j] - lowest) / (diff)) * 3 - 1) * heightFactor;// 3=2
                }
            }


            //heightData = gen.IslandFilter(heightData, -10f, 15 * width * height);

            //riverSimulation
            gen.newWaterSystem(heightData, 1, 1f, 0.8f, 1, 0.001f);


            //terrain setup + coloring

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));
                    if (heightData[i, j] > 0)
                    {
                        colors.Add(Color.LightGreen);
                    }
                    else
                    {
                        colors.Add(Color.Blue);
                    }
                    //waterColors.Add(Color.LightBlue);
                    //waterPositions.Add(Vector3.Zero);
                }
            }

            terrain = new SimpleTerrain(game, positions, colors, width, height);
            //water = new SimpleTerrain(game, waterPositions, waterColors, width, height);
            float[,] waterData = new float[width, height]; ;
            game.InitializeEvent +=
                delegate
                {


                    terrain.CreateRenderData();
                    //water.CreateDynamicRenderData(waterPositions, waterColors);
                    gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.5f), (int)(height * 0.5f)), 2f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.2f), (int)(height * 0.2f)), 2f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.8f), (int)(height * 0.8f)), 10f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.2f), (int)(height * 0.7f)), 5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.146f), (int)(height *0.654f)), 5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.687f), (int)(height * 0.9654f)), 5f);



                };
            bool changed = false;
            float time = 0f;
            Vector2[,] errosionData = new Vector2[width, height];


            game.UpdateEvent +=
                delegate
                {
                    time += (float)game.GameTime.ElapsedGameTime.TotalSeconds;
                    if (time > 0.02f)
                    {
                        time = 0;
                        gen.ComputeOneWaterCycleNew();
                        waterData = gen.GetOldWaterMap();
                        gen.ComputeErosion();
                        errosionData = gen.GetOldErrosionMap();

                        //waterPositions = new List<Vector3>();
                        //for (int i = 0; i < width; i++)
                        //{
                        //    for (int j = 0; j < height; j++)
                        //    {
                        //        waterPositions.Add(new Vector3(i,heightData[i,j]+waterData[i,j],j));

                        //    }
                        //}
                        //water.CreateDynamicRenderData(waterPositions, colors);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                    {
                        gen.AddRandomWaterDrops(10, 1);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        gen.ComputeOneWaterCycleNew();
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.V))
                    {
                        vLines = new LineManager3DLines();
                        gen.ComputeVelocityMap();
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                float heightDat = heightData[i, j] + gen.GetOldWaterMap()[i, j] * 1.2f;
                                vLines.AddLine(new Vector3(i, heightDat, j), new Vector3(i + gen.velocityMap[i, j].X * 30, heightDat, j + gen.velocityMap[i, j].Y * 30), Color.Red);
                            }
                        }
                    }
                };

            game.DrawEvent +=
                delegate
                {
                    LineManager3DLines lines = new LineManager3DLines();
                    for (int i = 0; i < width - 1; i++)
                    {
                        for (int j = 0; j < height - 1; j++)
                        {
                            float height1 = waterData[i, j];
                            if (height1 < 0.01)
                            {
                                height1 = 0;
                            }
                            else
                            {
                                height1 += heightData[i, j];
                            }
                            float height2 = waterData[i + 1, j];
                            if (height2 < 0.01)
                            {
                                height2 = 0;
                            }
                            else
                            {
                                height2 += heightData[i + 1, j];
                            }
                            float height3 = waterData[i, j + 1];
                            if (height3 < 0.01)
                            {
                                height3 = 0;
                            }
                            else
                            {
                                height3 += heightData[i, j + 1];
                            }

                            lines.AddTriangle(new Vector3(i, height1, j), new Vector3(i + 1, height2, j), new Vector3(i, height3, j + 1), Color.Blue);
                            lines.AddLine(new Vector3(i, heightData[i, j], j), new Vector3(i, heightData[i, j] + errosionData[i, j].Y * 200, j), Color.White);

                            lines.AddTriangle(new Vector3(i + (width + 10), gen.heightMap[i, j], j), new Vector3(i + 1 + (width + 10), gen.heightMap[i + 1, j], j), new Vector3(i + (width + 10), gen.heightMap[i, j + 1], j + 1), Color.Goldenrod);

                            //if (waterData[i, j] > 0.0001f)
                            //{
                            //    //game.LineManager3D.AddLine(new Vector3(i, heightData[i, j], j), new Vector3(i, heightData[i, j] + waterData[i, j], j), Color.Blue);
                            //    //game.LineManager3D..AddBox(new BoundingBox(new Vector3(i - 0.5f, heightData[i, j], j - 0.5f), new Vector3(i + 0.5f, heightData[i, j] + waterData[i, j], j + 0.5f)), Color.Blue);
                            //}
                        }
                    }
                    terrain.Render();
                    game.LineManager3D.Render(lines);
                    game.LineManager3D.Render(vLines);

                    //water.Render();
                };
            game.Run();
        }
        [Test]
        public void TsunamieTest()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.FarClip = 10000f;
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 100;
            int height = 100;
            float heightFactor = 50;
            SimpleTerrain terrain;
            //SimpleTerrain water;
            //List<Vector3> waterPositions = new List<Vector3>();
            //List<Color> waterColors = new List<Color>();            
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];
            float heigest = 0;
            float lowest = 0;
            float AmplitudeFactor = 40f;
            float diff = heigest - lowest;
            gen.newWaterSystem(heightData, 1, 1f, 0.8f, 1, 0.001f);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j], j));

                    colors.Add(Color.Beige);

                }

            }


            terrain = new SimpleTerrain(game, positions, colors, width, height);

            float[,] waterData = new float[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gen.GetOldWaterMap()[i, j] = 0.0f;
                }
            }
            game.InitializeEvent +=
                delegate
                {


                    terrain.CreateRenderData();


                };
            bool changed = false;
            float time = 0f;
            game.UpdateEvent +=
                delegate
                {
                    time += (float)game.GameTime.ElapsedGameTime.TotalSeconds;
                    if (time > 0.2f)
                    {
                        time = 0;
                        gen.ComputeOneWaterCycleNew();
                        waterData = gen.GetOldWaterMap();


                    }

                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
                    {
                        positions = new List<Vector3>();
                        float step = MathHelper.PiOver2 / (width * 0.5f);
                        for (int i = 0; i < (int)(width * 0.5f); i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                heightData[i, j] += 4 * (float)Math.Sin(i * step);
                                positions.Add(new Vector3(i, heightData[i, j], j));

                            }

                        }

                        terrain = new SimpleTerrain(game, positions, colors, width, height);
                        terrain.CreateRenderData();
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                    {
                        gen.AddRandomWaterDrops(2, 500);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        gen.ComputeOneWaterCycleNew();
                    }
                };
            game.DrawEvent +=
                delegate
                {
                    LineManager3DLines lines = new LineManager3DLines();
                    for (int i = 0; i < width - 1; i++)
                    {
                        for (int j = 0; j < height - 1; j++)
                        {
                            lines.AddTriangle(new Vector3(i, heightData[i, j] + waterData[i, j], j), new Vector3(i + 1, heightData[i + 1, j] + waterData[i + 1, j], j), new Vector3(i, heightData[i, j + 1] + waterData[i, j + 1], j + 1), Color.Blue);

                            if (waterData[i, j] > 0.0001f)
                            {
                                //game.LineManager3D.AddLine(new Vector3(i, heightData[i, j], j), new Vector3(i, heightData[i, j] + waterData[i, j], j), Color.Blue);
                                //game.LineManager3D..AddBox(new BoundingBox(new Vector3(i - 0.5f, heightData[i, j], j - 0.5f), new Vector3(i + 0.5f, heightData[i, j] + waterData[i, j], j + 0.5f)), Color.Blue);
                            }
                        }
                    }
                    terrain.Render();
                    game.LineManager3D.Render(lines);
                    //water.Render();
                };
            game.Run();
        }
        [Test]
        public void MultipleCyclesHydrolicErosionTest()
        {

            XNAGame game = new XNAGame();
            game.DrawFps = true;
            LineManager3DLines vLines = new LineManager3DLines();
            game.SpectaterCamera.FarClip = 10000f;
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 4f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 200;
            int height = 200;
            float lengthFactor = 0.5f;
            float heightFactor = 2;
            SimpleTerrain terrain;
            //SimpleTerrain water;
            //List<Vector3> waterPositions = new List<Vector3>();
            //List<Color> waterColors = new List<Color>();            
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float lowestIsland = 0;
            float[,] heightData = new float[width, height];
            float[,] heightDataRidge = new float[width, height];
            float[,] heightDataFBM = new float[width, height];
            float[,] heightDataIsland = new float[width, height];
            float[,] heightDataFinal = new float[width, height];
            float heigestFBM = 0;
            float heigestRidge = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //heightData[i, j] = (noise.CombinedFractalBrowningAndRidgedMF(i * lengthFactor, j * lengthFactor, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f) * 0.55f + gen.IslandFactor(i * lengthFactor, j * lengthFactor, new Vector2(width * lengthFactor * 0.5f, height * lengthFactor * 0.5f), width * lengthFactor * 0.45f, width * lengthFactor * 0.22f) * 0.65f) * AmplitudeFactor;//noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f)*0.1f+gen.IslandFactor(i, j, new Vector2(width * 0.45f, height * 0.5f),0,width*0.22f)*0.9f  ;

                    heightDataRidge[i, j] = noise.RidgedMF(i * lengthFactor, j * lengthFactor, 0.15f, 8, 2f, 0.7f, 0.8f);//noise.CombinedFractalBrowningAndRidgedMF(i * lengthFactor, j * lengthFactor, 8, 4,5f, 0.5f, 2f, 0.8f, 1f);
                    heightDataFBM[i, j] = noise.GetFractalBrowningNoise(i * lengthFactor, j * lengthFactor, 8, 1.2f, 1.9f, 1.2f);
                    heightDataIsland[i, j] = gen.IslandFactor(i * lengthFactor, j * lengthFactor, new Vector2(width * lengthFactor * 0.5f, height * lengthFactor * 0.5f), width * lengthFactor * 0.42f, width * lengthFactor * 0.4f);//noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f)*0.1f+gen.IslandFactor(i, j, new Vector2(width * 0.45f, height * 0.5f),0,width*0.22f)*0.9f  ;

                    if (heigestFBM < heightDataFBM[i, j])
                    {
                        heigestFBM = heightDataFBM[i, j];
                    }
                    if (heigestRidge < heightDataRidge[i, j])
                    {
                        heigestRidge = heightDataRidge[i, j];
                    }

                    if (lowestIsland > heightDataIsland[i, j])
                    {
                        lowestIsland = heightDataIsland[i, j];
                    }
                }
            }
            //float diff = heigest - lowest;
            //float diffIsland = heigestIsland - lowestIsland;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (heightDataIsland[i, j] < 0)
                    {

                        heightDataIsland[i, j] = -(heightDataIsland[i, j] / lowestIsland);
                    }
                    heightDataFBM[i, j] = ((heightDataFBM[i, j] / heigestFBM) * (5) + 0.1f);
                    heightDataRidge[i, j] = ((heightDataRidge[i, j] / heigestRidge) * (3) + 0.1f);

                    heightData[i, j] = heightDataFBM[i, j] * heightDataRidge[i, j];
                    heightDataFinal[i, j] = heightData[i, j] * heightDataIsland[i, j] * scale;
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightDataFinal[i, j], j));
                    if (heightDataFinal[i, j] > 0)
                    {
                        colors.Add(Color.LightGreen);
                    }
                    else
                    {
                        colors.Add(Color.Blue);
                    }

                }
            }
            terrain = new SimpleTerrain(game, positions, colors, width, height);
            gen.newWaterSystem(heightDataFinal, 1, 1, 0.9f, 1, 0.0001f);

            //water = new SimpleTerrain(game, waterPositions, waterColors, width, height);
            float[,] waterData = new float[width, height]; ;
            game.InitializeEvent +=
                delegate
                {


                    terrain.CreateRenderData();
                    //water.CreateDynamicRenderData(waterPositions, waterColors);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.5f), (int)(height * 0.5f)), 0.5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.2f), (int)(height * 0.2f)), 2f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.8f), (int)(height * 0.8f)), 10f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.2f), (int)(height * 0.7f)), 5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.146f), (int)(height *0.654f)), 5f);
                    //gen.AddSpring(new ProceduralHeigthGenerater.IndexStruct((int)(width * 0.687f), (int)(height * 0.9654f)), 5f);

                    gen.GenerateClouds(4);

                };
            bool changed = false;
            float time = 0f;
            Vector2[,] errosionData = new Vector2[width, height];

            bool pauzed = true;
            bool waterRender = false;
            bool VelocityRender = false;
            bool computeErosion = false;
            int counter = 0;
            int CycleCounter = 0;
            game.UpdateEvent +=
                delegate
                {
                    time += (float)game.GameTime.ElapsedGameTime.TotalSeconds;
                    if (time > 0.02f && !pauzed)
                    {
                        time = 0;

                        counter++;
                        if (counter > 50)
                        {
                            //gen.AddRandomWaterDrops((int)(width * height * 0.08f), 0.02f);
                            gen.GenerateClouds(5);
                            if (computeErosion)
                            {
                                //gen.SingleThermalErosionCycle();

                            }
                            counter = 0;
                        }
                        gen.AnimateClouds();
                        gen.ComputeOneWaterCycleNew();
                        waterData = gen.GetOldWaterMap();




                        if (computeErosion)
                        {
                            CycleCounter++;
                            gen.ComputeErosion();
                            gen.CutOutRivers(0.01f);
                            errosionData = gen.GetOldErrosionMap();
                        }


                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                    {
                        if (pauzed)
                        {
                            pauzed = false;
                        }
                        else { pauzed = true; }
                    }
                    //if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    //{
                    //    gen.ComputeOneWaterCycleNew();
                    //}
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.V))
                    {
                        vLines = new LineManager3DLines();
                        gen.ComputeVelocityMap();
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                float heightDat = heightData[i, j] + gen.GetOldWaterMap()[i, j] * 1.2f;
                                vLines.AddLine(new Vector3(i * lengthFactor, heightDat, j * lengthFactor), new Vector3(i * lengthFactor + gen.velocityMap[i, j].X * 30, heightDat, j * lengthFactor + gen.velocityMap[i, j].Y * 30), Color.Red);
                            }
                        }
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.X))
                    {
                        if (VelocityRender)
                        {
                            VelocityRender = false;
                        }
                        else { VelocityRender = true; }
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.U))
                    {
                        heightData = gen.heightMap;
                        positions = new List<Vector3>();
                        colors = new List<Color>();
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                positions.Add(new Vector3(i * lengthFactor, heightDataFinal[i, j], j * lengthFactor));
                                if (heightDataFinal[i, j] > 0)
                                {
                                    colors.Add(Color.LightGreen);
                                }
                                else
                                {
                                    colors.Add(Color.Blue);
                                }
                                //waterColors.Add(Color.LightBlue);
                                //waterPositions.Add(Vector3.Zero);
                            }
                        }
                        terrain = new SimpleTerrain(game, positions, colors, width, height);
                        terrain.CreateRenderData();
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        if (computeErosion)
                        {
                            computeErosion = false;
                        }
                        else { computeErosion = true; }
                    }


                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.W))
                    {
                        if (waterRender)
                        {
                            waterRender = false;
                        }
                        else { waterRender = true; }
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        gen.CutOutRivers(100);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
                    {
                        gen.SingleThermalErosionCycle();
                    }
                };

            game.DrawEvent +=
                delegate
                {
                    game.SpriteBatch.Begin();
                    game.SpriteBatch.DrawString(game.SpriteFont, CycleCounter.ToString(), new Vector2(0, 20), Color.Black);
                    game.SpriteBatch.End();
                    LineManager3DLines lines = new LineManager3DLines();
                    if (waterRender)
                    {


                        for (int i = 0; i < width - 1; i++)
                        {
                            for (int j = 0; j < height - 1; j++)
                            {
                                if (heightDataFinal[i, j] > 0)
                                {
                                    float height1 = waterData[i, j];
                                    if (height1 < 0.01)
                                    {
                                        height1 = 0;
                                    }
                                    else
                                    {
                                        height1 += heightDataFinal[i, j];
                                    }
                                    float height2 = waterData[i + 1, j];
                                    if (height2 < 0.01)
                                    {
                                        height2 = 0;
                                    }
                                    else
                                    {
                                        height2 += heightDataFinal[i + 1, j];
                                    }
                                    float height3 = waterData[i, j + 1];
                                    if (height3 < 0.01)
                                    {
                                        height3 = 0;
                                    }
                                    else
                                    {
                                        height3 += heightDataFinal[i, j + 1];
                                    }

                                    lines.AddTriangle(new Vector3(i, height1, j), new Vector3(i + 1, height2, j), new Vector3(i, height3, j + 1), Color.Blue);
                                    lines.AddLine(new Vector3(i, heightDataFinal[i, j], j), new Vector3(i, heightDataFinal[i, j] + errosionData[i, j].Y * 200, j), Color.White);


                                }
                                //lines.AddTriangle(new Vector3(i + (width + 10), gen.heightMap[i, j], j), new Vector3(i + 1 + (width + 10), gen.heightMap[i + 1, j], j), new Vector3(i + (width + 10), gen.heightMap[i, j + 1], j + 1), Color.Goldenrod);

                                //if (waterData[i, j] > 0.0001f)
                                //{
                                //    //game.LineManager3D.AddLine(new Vector3(i, heightData[i, j], j), new Vector3(i, heightData[i, j] + waterData[i, j], j), Color.Blue);
                                //    //game.LineManager3D..AddBox(new BoundingBox(new Vector3(i - 0.5f, heightData[i, j], j - 0.5f), new Vector3(i + 0.5f, heightData[i, j] + waterData[i, j], j + 0.5f)), Color.Blue);
                                //}
                            }
                        }
                        for (int i = 0; i < gen.clouds.Count; i++)
                        {
                            lines.AddCenteredBox(new Vector3(gen.clouds[i].Position.X, 20, gen.clouds[i].Position.Y), gen.clouds[i].Width * lengthFactor, Color.Red);
                        }
                        game.LineManager3D.Render(lines);
                    }
                    terrain.Render();
                    if (VelocityRender)
                    {
                        lines = new LineManager3DLines();
                        gen.ComputeVelocityMap();
                        //velocity
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                float heightDat = heightDataFinal[i, j] + gen.GetOldWaterMap()[i, j] * 1.2f;
                                lines.AddLine(new Vector3(i, heightDat, j), new Vector3(i + gen.velocityMap[i, j].X * 30, heightDat, j + gen.velocityMap[i, j].Y * 30), Color.Red);

                            }
                        }
                        game.LineManager3D.Render(lines);
                    }


                    //water.Render();
                };
            game.Run();
        }


        //thermal errosion tests
        [Test]
        public void ThermalErrosionTest()
        {
            XNAGame game = new XNAGame();
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 1f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int width = 100;
            int height = 100;
            SimpleTerrain terrain;
            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float[,] heightData = new float[width, height];

            float heigest = -2000;
            float lowest = 2000;
            float heightFactor = 10;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f) * 0.35f + gen.IslandFactor(i, j, new Vector2(width * 0.5f, height * 0.5f), width * 0.45f, width * 0.22f) * 0.65f;
                    if (heigest < heightData[i, j])
                    {
                        heigest = heightData[i, j];
                    }
                    if (lowest > heightData[i, j])
                    {
                        lowest = heightData[i, j];
                    }
                }
            }
            float diff = heigest - lowest;
            //for (int i = 0; i < width; i++)
            //{
            //    for (int j = 0; j < height; j++)
            //    {

            //        heightData[i, j] = (((heightData[i, j] - lowest) / (diff)) * 2 - 1) * heightFactor;
            //    }
            //}
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions.Add(new Vector3(i, heightData[i, j] * 5f, j));
                    if (heightData[i, j] > 0)
                    {
                        colors.Add(Color.LightGreen);
                    }
                    else
                    {
                        colors.Add(Color.BlueViolet);
                    }

                }
            }
            gen.heightMap = heightData;
            gen.NewWaterSimulation(heightData);
            terrain = new SimpleTerrain(game, positions, colors, width, height);
            game.InitializeEvent +=
                delegate
                {

                    terrain.CreateRenderData();

                };
            bool changed = false;
            int cycles = 2;
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Add))
                    {
                        cycles++;
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.OemMinus))
                    {
                        cycles--;
                        if (cycles < 1)
                        {
                            cycles = 1;
                        }
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.U))
                    {
                        for (int i = 0; i < cycles; i++)
                        {
                            gen.SingleThermalErosionCycle();
                        }

                        positions = new List<Vector3>();
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                positions.Add(new Vector3(i, gen.heightMap[i, j], j));

                            }
                        }
                        terrain = new SimpleTerrain(game, positions, colors, width, height);
                        terrain.CreateRenderData();
                    }
                };
            game.DrawEvent +=
                delegate
                {
                    terrain.Render();

                };
            game.Run();

        }
    }
}

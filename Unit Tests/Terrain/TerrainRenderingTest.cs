using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Terrain
{
    [TestFixture]
    public class TerrainRenderingTest
    {
        [Test]
        public void TestRenderBlock()
        {
            var game = new XNAGame();
            var block = new TerrainBlock();
            block.BlockSize = 16;
            VertexBuffer vb = null;
            VertexDeclaration decl = null;
            var vertices = new VertexMultitextured[(block.BlockSize + 1) * (block.BlockSize + 1)];
            for (int i = 0; i < block.BlockSize + 1; i++)
            {
                for (int j = 0; j < block.BlockSize + 1; j++)
                {
                    vertices[block.IndexFromCoords(i, j)].Position = new Vector3(
                        i, 0, j);
                }
            }


            TerrainShaderNew shader = null;

            game.InitializeEvent += delegate
                                        {
                                            shader = new TerrainShaderNew(game, new EffectPool());
                                            block.BuildIndexBuffer(game.GraphicsDevice);

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
                                            if (block.DetailLevel < block.MaxDetailLevel)
                                                block.ChangeDetailLevel(block.DetailLevel + 1, false);
                                        }
                                        else if (game.Keyboard.IsKeyPressed(Keys.Subtract))
                                        {
                                            if (block.DetailLevel > 0)
                                                block.ChangeDetailLevel(block.DetailLevel - 1, false);

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
                                              vertices.Length, 0, block.TotalBaseTriangles + block.TotalEdgeTriangles);
                                      });


                                      game.GraphicsDevice.Indices = null;
                                  };


            game.Run();
        }
    }
}

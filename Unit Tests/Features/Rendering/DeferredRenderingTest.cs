using System;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;

using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using Buffer = SlimDX.Direct3D11.Buffer;
using DataStream = SlimDX.DataStream;
using SpectaterCamera = MHGameWork.TheWizards.DirectX11.Graphics.SpectaterCamera;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture]
    public class DeferredRenderingTest
    {
        [Test]
        public void TestRenderDefaultModelShader()
        {

            var shaders = new List<BasicShader>();


            DX11Game game = new DX11Game();
            game.InitDirectX();

            var context = game.Device.ImmediateContext;

            var shader = BasicShader.LoadAutoreload(game, DeferredMeshRenderer.DeferredMeshFX);
            shader.SetTechnique("Technique1");

            var gBuffer = new GBuffer(game.Device, 800, 600);


            var layout = new InputLayout(game.Device, shader.GetCurrentPass(0).Description.Signature, DeferredMeshVertex.Elements);
            DeferredMeshVertex[] vertices = new DeferredMeshVertex[4];


            vertices[1] = new DeferredMeshVertex(Vector3.Zero.ToVector4(), Vector2.Zero, MathHelper.Up);
            vertices[0] = new DeferredMeshVertex(MathHelper.Forward.ToVector4(), Vector2.UnitX, MathHelper.Up);
            vertices[2] = new DeferredMeshVertex((MathHelper.Forward + MathHelper.Right).ToVector4(), Vector2.UnitX + Vector2.UnitY, MathHelper.Up);
            vertices[3] = new DeferredMeshVertex(MathHelper.Right.ToVector4(), Vector2.UnitY, MathHelper.Up);

            Buffer vb;
            using (var strm = new DataStream(vertices, true, false))
            {

                vb = new Buffer(game.Device, strm, (int)strm.Length, ResourceUsage.Immutable, BindFlags.VertexBuffer,
                                CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }



            Texture2D tex;
            using (var strm = File.OpenRead(TestFiles.WoodPlanksBareJPG))
                tex = Texture2D.FromStream(game.Device, strm, (int)strm.Length);

            var texRV = new ShaderResourceView(game.Device, tex);

            BasicShader s;
            //s = shader.Clone();
            //s.DiffuseColor = new Vector4(1, 0, 0, 0);
            //s.Technique = DefaultModelShader.TechniqueType.Colored;
            //shaders.Add(s);

            s = shader.Clone();
            s.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(texRV);
            //s.DiffuseTexture = tex;
            //s.Technique = DefaultModelShader.TechniqueType.Textured;
            shaders.Add(s);



            game.GameLoopEvent += delegate
                                  {
                                      gBuffer.Clear();
                                      gBuffer.SetTargetsToOutputMerger();

                                      context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                                      context.InputAssembler.InputLayout = layout;
                                      context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

                                      // Use shared variables
                                      shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
                                      shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);

                                      shader.Apply();
                                      for (int i = 0; i < shaders.Count; i++)
                                      {
                                          //shaders[i].ViewProjection = game.Camera.ViewProjection;
                                          shaders[i].Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Translation(MathHelper.Right * i * 3) * Matrix.Scaling(MathHelper.One * 10));
                                          context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vb, DeferredMeshVertex.SizeInBytes, 0));

                                          shaders[i].Apply();
                                          context.Draw(4, 0);

                                      }

                                      context.ClearState();
                                      game.SetBackbuffer();

                                      DeferredTest.DrawGBuffer(game, gBuffer);
                                  };

            game.Run();
        }




    }
}

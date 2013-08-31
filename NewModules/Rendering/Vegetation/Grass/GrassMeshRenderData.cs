using System.IO;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Grass
{
    /// <summary>
    /// This represents the renderdata of a grasmesh. It's responsible for drawing the grass
    /// </summary>

    public class GrassMeshRenderData
    {
        private Buffer vertexBuffer;
        private Buffer indexBuffer;
        private InputLayout layout;
        private int indexCount;
        private BasicShader shader;
        private ShaderResourceView diffuseTexture;
    
        public GrassMeshRenderData(Buffer vertexBuffer, Buffer indexBuffer, int indexCount, Device device, DX11Game game)
        {
            this.vertexBuffer = vertexBuffer;
            this.indexBuffer = indexBuffer;
            this.indexCount = indexCount;

            shader = BasicShader.LoadAutoreload(game, new FileInfo(CompiledShaderCache.Current.RootShaderPath + "Vegetation\\Grass.fx"));
            shader.SetTechnique("AnimatedGrass");


            var diffuseTex = Texture2D.FromFile(device, @"C:\_MHData\1 - Projecten\The Wizards\_Source\NewModules\ProceduralTerrainGeneration\Grass\Textures\GrassTextures\grass.png");//TODO: none dynamic path finding of the file
            diffuseTexture = new ShaderResourceView(device, diffuseTex);

            layout = new InputLayout(device, shader.GetCurrentPass(0).Description.Signature, DeferredMeshVertex.Elements);
        }

        private float time = 0;
        public void Draw(DeviceContext context, DX11Game game)
        {
            time += game.Elapsed;
            time = time%200;
            shader.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(diffuseTexture);
            shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            shader.Effect.GetVariableByName("Time").AsScalar().Set(time);
            shader.Apply();


            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.SetVertexBuffers(0,new VertexBufferBinding(vertexBuffer,
                                                                                            DeferredMeshVertex.
                                                                                                SizeInBytes, 0));
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetIndexBuffer(indexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);

            context.DrawIndexed(indexCount, 0, 0);

        }

    }
}

    
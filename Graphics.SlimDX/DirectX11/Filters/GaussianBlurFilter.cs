using System;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Filters
{
    /// <summary>
    /// Responsible for implementing a Gaussian Blur on the GPU using ComputeShader
    /// </summary>
    public class GaussianBlurFilter
    {
        private readonly DX11Game game;
        private DeviceContext context;
        private ComputeShader csX;
        private ComputeShader csY;

        private const int ThreadGroupSize = 8; // corresponds to shader

        public GaussianBlurFilter(DX11Game game)
        {
            this.game = game;
            context = game.Device.ImmediateContext;
            csX = loadComputeShader(CompiledShaderCache.Current.RootShaderPath + "Filters\\GaussianBlurCS.hlsl", "CSMAINX");
            csY = loadComputeShader(CompiledShaderCache.Current.RootShaderPath + "Filters\\GaussianBlurCS.hlsl", "CSMAINY");

        }

        public void Blur(ShaderResourceView input, GPUTexture buffer, UnorderedAccessView output)
        {
            var width = buffer.Resource.Description.Width;
            int groupsX = (int)Math.Ceiling(width / (double)ThreadGroupSize);
            var height = buffer.Resource.Description.Height;
            int groupsY = (int)Math.Ceiling(height / (double)ThreadGroupSize);


            context.ClearState();
            context.ComputeShader.Set(csX);
            context.ComputeShader.SetShaderResource(input, 0);
            context.ComputeShader.SetUnorderedAccessView(buffer.UnorderedAccessView, 0);

            context.Dispatch(groupsX, height, 1);

            context.ClearState();
            context.ComputeShader.Set(csY);
            context.ComputeShader.SetShaderResource(buffer.View, 0);
            context.ComputeShader.SetUnorderedAccessView(output, 0);

            context.Dispatch(width, groupsY, 1);


        }

        private ComputeShader loadComputeShader(string file, string entrypoint)
        {
            var bytecode = ShaderBytecode.CompileFromFile(file, entrypoint, "cs_5_0", ShaderFlags.Debug, EffectFlags.None);
            var ret = new ComputeShader(game.Device, bytecode);
            return ret;
        }
    }
}

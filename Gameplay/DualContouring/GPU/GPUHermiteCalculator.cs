using System;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.DXGI;
using SlimDX.Direct3D11;
using ResourceOptionFlags = SlimDX.Direct3D10.ResourceOptionFlags;

namespace MHGameWork.TheWizards.DualContouring.GPU
{
    public class GPUHermiteCalculator
    {
        private readonly DX11Game game;
        private DeviceContext context;
        private ComputeShader csX;
        private ComputeShader csY;
        private GPUTexture3D perlin;
        private SamplerState trilinearSampler;
        private ComputeShader csIntersections;

        private const int ThreadGroupSize = 8; // corresponds to shader

        public GPUHermiteCalculator(DX11Game game)
        {
            this.game = game;
            context = game.Device.ImmediateContext;
            csX = loadComputeShader(CompiledShaderCache.Current.RootShaderPath + "DualContouring\\HermiteTerrain.hlsl", "CSGridSigns");
            csIntersections = loadComputeShader(CompiledShaderCache.Current.RootShaderPath + "DualContouring\\HermiteTerrain.hlsl", "CSCalcIntersections");

            var random = new Random();

            var noiseSize = 64;
            var data = new float[noiseSize * noiseSize * noiseSize];

            /*var data = new float[2 * 2 * 2];
            data[0] = 1;
            data[1] = -1;
            data[2] = -1;
            data[3] = -1;
            data[4] = -1;
            data[5] = -1;
            data[6] = -1;
            data[7] = -1;*/
            for (int i = 0; i < noiseSize * noiseSize * noiseSize; i++)
            {
                data[i] = (float)(random.NextDouble() * 2 - 1);
            }


            var strm = new DataStream(data, true, false);


            perlin = GPUTexture3D.CreateDefault(game, noiseSize, noiseSize, noiseSize, Format.R32_Float);
            game.Device.ImmediateContext.UpdateSubresource(new DataBox(4 * noiseSize, 4 * noiseSize * noiseSize, strm),
                perlin.Resource, 0, new ResourceRegion(0, 0, 0, noiseSize, noiseSize, noiseSize));

            /*perlin = GPUTexture3D.CreateCPUWritable(game, 64, 64, 64, Format.R8G8B8A8_UNorm);
            var random = new Random(0);
            var data = new byte[64 * 64 * 64 * 4];
            var i = 0;
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    for (int z = 0; z < 64; z++)
                    {

                        //var val = ((x/64f)*2 - 1 + (z/64f)*2 - 1)/2.0f;
                        //val = val > 0 ? 1 : -1;
                        //data[i*4] = (byte) ((val/2f + 0.5f)*255f);
                        data[i * 4] = (byte)random.Next(0, 255);
                        i++;

                    }
                }
            }*/
            //perlin.SetTextureRawData(data);
            //perlin.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/perlin"));

            trilinearSampler = SamplerState.FromDescription(game.Device, new SamplerDescription()
                {
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    Filter = Filter.MinMagMipPoint,
                });

        }

        private ComputeShader loadComputeShader(string file, string entrypoint)
        {
            var bytecode = ShaderBytecode.CompileFromFile(file, entrypoint, "cs_5_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization, EffectFlags.None);
            //Console.WriteLine(bytecode.Disassemble());
            var ret = new ComputeShader(game.Device, bytecode);
            return ret;
        }

        public void WriteHermiteSigns(int size, Vector3 offset, Vector3 scaling, string density, GPUTexture3D tx)
        {

            //var tx = GPUTexture3D.CreateUAV(game, size, size, size, Format.R32_Typeless, Format.R32_Float);
            //var tx = CreateDensitySignsTexture(size);//GPUTexture3D.CreateUAV(game, size, size, size, Format.R8G8B8A8_UNorm, Format.R8G8B8A8_UNorm);

            var width = tx.Resource.Description.Width;
            int groupsX = (int)Math.Ceiling(width / (double)ThreadGroupSize);
            var height = tx.Resource.Description.Height;
            int groupsY = (int)Math.Ceiling(height / (double)ThreadGroupSize);


            context.ClearState();
            context.ComputeShader.Set(csX);
            context.ComputeShader.SetShaderResource(perlin.View, 0);
            context.ComputeShader.SetUnorderedAccessView(tx.UnorderedAccessView, 0);
            context.ComputeShader.SetSampler(trilinearSampler, 0);

            context.Dispatch(groupsX, groupsX, groupsX);

            /* context.ClearState();
             context.ComputeShader.Set(csY);
             context.ComputeShader.SetShaderResource(buffer.View, 0);
             context.ComputeShader.SetUnorderedAccessView(output, 0);

             context.Dispatch(width, groupsY, 1);*/

            //context.GenerateMips(tx.View);

        }

        public void WriteHermiteIntersections(int size, GPUTexture3D signsTexture, GPUTexture3D intersectionsTexture, GPUTexture3D normals1, GPUTexture3D normals2, GPUTexture3D normals3)
        {
            int groups = size / ThreadGroupSize;

            context.ClearState();
            context.ComputeShader.Set(csIntersections);
            context.ComputeShader.SetShaderResource(perlin.View, 0);
            context.ComputeShader.SetSampler(trilinearSampler, 0);
            context.ComputeShader.SetUnorderedAccessView(signsTexture.UnorderedAccessView, 0);
            context.ComputeShader.SetUnorderedAccessView(intersectionsTexture.UnorderedAccessView, 1);
            context.ComputeShader.SetUnorderedAccessView(normals1.UnorderedAccessView, 2);
            context.ComputeShader.SetUnorderedAccessView(normals2.UnorderedAccessView, 3);
            context.ComputeShader.SetUnorderedAccessView(normals3.UnorderedAccessView, 4);

            context.Dispatch(groups, groups, groups);

            /* context.ClearState();
             context.ComputeShader.Set(csY);
             context.ComputeShader.SetShaderResource(buffer.View, 0);
             context.ComputeShader.SetUnorderedAccessView(output, 0);

             context.Dispatch(width, groupsY, 1);*/

            //context.GenerateMips(tx.View);
        }

        public byte[] ReadDataThroughStageBuffer(GPUTexture3D texture)
        {
            //texture.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("Temp2"));

            var cache = GPUTexture3D.CreateCPUReadable(game, texture.Resource.Description.Width, texture.Resource.Description.Height, texture.Resource.Description.Depth, texture.Resource.Description.Format);

            game.Device.ImmediateContext.CopySubresourceRegion(texture.Resource, 0, cache.Resource, 0, 0, 0, 0);
            //cache.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("Temp"));
            return cache.GetRawData();

        }

        public GPUTexture3D CreateIntersectionsTexture(int size)
        {
            return CreateDensitySignsTexture(size);
        }
        public GPUTexture3D CreateDensitySignsTexture(int size)
        {
            var desc = new Texture3DDescription()
                {
                    Width = size,
                    Height = size,
                    Depth = size,
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,// | BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.R8G8B8A8_UNorm,
                    MipLevels = 0,
                    //OptionFlags = SlimDX.Direct3D11.ResourceOptionFlags.GenerateMipMaps

                };
            var resource = new Texture3D(game.Device, desc);

            var uav = new UnorderedAccessView(game.Device, resource, new UnorderedAccessViewDescription()
                {
                    Format = Format.R8G8B8A8_UNorm,
                    Dimension = UnorderedAccessViewDimension.Texture3D,
                    ArraySize = -1,
                    DepthSliceCount = -1,
                    ElementCount = -1,
                });

            /*var view = new ShaderResourceView(game.Device, resource, new ShaderResourceViewDescription()
                {
                    Dimension = ShaderResourceViewDimension.Texture3D,
                    Format = Format.R8G8B8A8_UNorm,
                    MipLevels = -1
                });*/

            var tex = new GPUTexture3D(resource, null, uav);




            return tex;
        }

        public AbstractHermiteGrid CalculateHermiteGrid(int size, Vector3 offset, Vector3 scaling, string density)
        {
            throw new NotImplementedException();
        }

        public GPUTexture3D CreateNormalsTexture(int size)
        {
            return CreateDensitySignsTexture(size);
        }
    }
}
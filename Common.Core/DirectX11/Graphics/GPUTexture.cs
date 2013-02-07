using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    /// <summary>
    /// Helper class for working with D3D11 texture resources;
    /// </summary>
    public class GPUTexture : IDisposable
    {
        private UnorderedAccessView unorderedAccessView;
        public Texture2D Resource { get; private set; }
        public ShaderResourceView View { get; private set; }
        public UnorderedAccessView UnorderedAccessView
        {
            get
            {
                if (unorderedAccessView == null)
                    CreateUnorderedAccessView();
                return unorderedAccessView;
            }
        }



        private GPUTexture(DX11Game game, Texture2DDescription desc)
        {
            initialize(new Texture2D(game.Device, desc));
        }
        private GPUTexture(Texture2D resource)
        {
            initialize(resource);
        }

        private void initialize(Texture2D resource)
        {
            Resource = resource;
            View = new ShaderResourceView(resource.Device, Resource);
        }

        private void CreateUnorderedAccessView()
        {
            unorderedAccessView = new UnorderedAccessView(Resource.Device, Resource);
        }


        /// <summary>
        /// Sets given raw byte[] array as data to be used by the texture.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="rgbValues">Array containing the values for each texel, rows placed sequentially</param>
        public void SetTextureRawData(byte[] rgbValues)
        {
            var tex = Resource;

            var width = tex.Description.Width;
            var height = tex.Description.Height;


            var box = tex.Device.ImmediateContext.MapSubresource(tex, 0, 0, MapMode.WriteDiscard,
                                                                  SlimDX.Direct3D11.MapFlags.None);

            for (int iRow = 0; iRow < height; iRow++)
            {
                // The driver can add padding after each row, this code fixes those padding errors
                box.Data.Seek(iRow * box.RowPitch, SeekOrigin.Begin);
                box.Data.Write(rgbValues, iRow * 4 * width, width * 4);
            }

            tex.Device.ImmediateContext.UnmapSubresource(tex, 0);
        }



        public static GPUTexture CreateCPUWritable(DX11Game game, int width, int height, Format format)
        {
            var desc = new Texture2DDescription
            {
                ArraySize = 1,
                MipLevels = 1,
                Format = format,
                Usage = ResourceUsage.Dynamic,
                CpuAccessFlags = CpuAccessFlags.Write,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.ShaderResource
            };
            return new GPUTexture(game, desc);
        }
        public static GPUTexture CreateUAV(DX11Game game, int width, int height, Format format)
        {
            var desc = new Texture2DDescription
            {
                ArraySize = 1,
                MipLevels = 1,
                Format = format,
                Usage = ResourceUsage.Default, // Read and write on gpu
                CpuAccessFlags = CpuAccessFlags.None,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess
            };
            return new GPUTexture(game, desc);
        }

        public static GPUTexture FromFile(DX11Game game, string path)
        {
            var tex = Texture2D.FromFile(game.Device, path);
            return new GPUTexture(tex);
        }

        public void Dispose()
        {
            if (Resource != null)
                Resource.Dispose();
            Resource = null;
            if (View != null)
                View.Dispose();
            View = null;

        }
    }
}

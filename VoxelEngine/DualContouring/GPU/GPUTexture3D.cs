using System;
using System.IO;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.DualContouring.GPU
{
    public class GPUTexture3D
    {
        private UnorderedAccessView unorderedAccessView;
        public Texture3D Resource { get; private set; }
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


        public GPUTexture3D(Texture3D resource, ShaderResourceView view, UnorderedAccessView unorderedAccessView)
        {
            Resource = resource;
            View = view;
        }

        private GPUTexture3D(DX11Game game, Texture3DDescription desc)
        {
            Texture3D resource = new Texture3D(game.Device, desc);
            Resource = resource;
            View = new ShaderResourceView(resource.Device, Resource);

        }

        private GPUTexture3D(Texture3D resource)
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
            var depth = tex.Description.Depth;


            var box = tex.Device.ImmediateContext.MapSubresource(tex, 0, 0, MapMode.WriteDiscard,
                SlimDX.Direct3D11.MapFlags.None);

            for (int iSlice = 0; iSlice < height; iSlice++)
            {
                for (int iRow = 0; iRow < height; iRow++)
                {
                    // The driver can add padding after each row, this code fixes those padding errors
                    box.Data.Seek(iSlice * box.SlicePitch + iRow * box.RowPitch, SeekOrigin.Begin);
                    box.Data.Write(rgbValues, iSlice * 4 * height * width + iRow * 4 * width, width * 4);
                }
            }


            tex.Device.ImmediateContext.UnmapSubresource(tex, 0);
        }

        /// <summary>
        /// Gets given raw byte[] array of the resource
        /// </summary>
        public byte[] GetRawData()
        {
            var tex = Resource;

            var width = tex.Description.Width;
            var height = tex.Description.Height;
            var depth = tex.Description.Depth;


            var box = tex.Device.ImmediateContext.MapSubresource(tex, 0, 0, MapMode.Read,
                SlimDX.Direct3D11.MapFlags.None);

            var ret = new byte[width * height * depth * 4];


            for (int iSlice = 0; iSlice < height; iSlice++)
            {
                for (int iRow = 0; iRow < height; iRow++)
                {
                    // The driver can add padding after each row, this code fixes those padding errors
                    box.Data.Seek(iSlice * box.SlicePitch + iRow * box.RowPitch, SeekOrigin.Begin);
                    box.Data.Read(ret, iSlice * 4 * height * width + iRow * 4 * width, width * 4);
                }
            }


            tex.Device.ImmediateContext.UnmapSubresource(tex, 0);

            return ret;
        }



        public static GPUTexture3D CreateCPUWritable(DX11Game game, int width, int height, int depth, Format format)
        {
            var desc = getBaseDesc(width, height, depth, format);
            desc.Usage = ResourceUsage.Dynamic;
            desc.CpuAccessFlags = CpuAccessFlags.Write;
            desc.BindFlags = BindFlags.ShaderResource;

            return new GPUTexture3D(game, desc);
        }

        private static Texture3DDescription getBaseDesc(int width, int height, int depth, Format format)
        {
            var desc = new Texture3DDescription
            {
                MipLevels = 1,
                Format = format,
                Width = width,
                Height = height,
                Depth = depth,
            };
            return desc;
        }

        public static GPUTexture3D CreateUAV(DX11Game game, int width, int height, int depth, Format format, Format viewFormat)
        {
            var desc = getBaseDesc(width, height, depth, format);
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess;

            var resource = new Texture3D(game.Device, desc);

            var uav = new UnorderedAccessView(resource.Device, resource, new UnorderedAccessViewDescription()
            {
                Format = viewFormat,
                Dimension = UnorderedAccessViewDimension.Texture3D,
                ArraySize = -1,
                DepthSliceCount = -1,
                ElementCount = -1,
            });
            var tex = new GPUTexture3D(resource, null, uav);

            return tex;

        }

        public static GPUTexture3D FromFile(DX11Game game, string path)
        {
            var tex = Texture3D.FromFile(game.Device, path);
            return new GPUTexture3D(tex);
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

        public void SaveToImageSlices(DX11Game game, DirectoryInfo dir)
        {
            for (int i = 0; i < Resource.Description.Depth; i++)
            {
                var tex2d = GPUTexture.CreateCPUWritable(game, Resource.Description.Width, Resource.Description.Height,
              Format.R8G8B8A8_UNorm);
                Resource.Device.ImmediateContext.CopySubresourceRegion(Resource, 0,
                    new ResourceRegion(0, 0, i, Resource.Description.Width, Resource.Description.Height, i + 1), tex2d.Resource, 0, 0, 0, 0);

                Texture2D.SaveTextureToFile(game.Device.ImmediateContext, tex2d.Resource, ImageFileFormat.Bmp,
                    dir.FullName + "/" + i + ".bmp");
            }

        }

        /// <summary>
        /// Creates cpu readable staging texture to copy sameAs into
        /// </summary>
        /// <param name="game"></param>
        /// <param name="sameAs"></param>
        /// <returns></returns>
        public static GPUTexture3D CreateStaging( DX11Game game, GPUTexture3D sameAs )
        {
            return GPUTexture3D.CreateCPUReadable(TW.Graphics, sameAs.Resource.Description.Width, sameAs.Resource.Description.Height, sameAs.Resource.Description.Depth, sameAs.Resource.Description.Format);
        }
        public static GPUTexture3D CreateCPUReadable(DX11Game game, int width, int height, int depth, Format format)
        {
            var desc = getBaseDesc(width, height, depth, format);
            desc.Usage = ResourceUsage.Staging;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            desc.BindFlags = BindFlags.None;

            return new GPUTexture3D(new Texture3D(game.Device, desc), null, null);
        }

        public static GPUTexture3D CreateDefault(DX11Game game, int width, int height, int depth, Format format)
        {
            var desc = getBaseDesc(width, height, depth, format);
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.BindFlags = BindFlags.ShaderResource;

            return new GPUTexture3D(game, desc);
        }

        public void CopyResourceFrom( GPUTexture3D signsTex )
        {
            TW.Graphics.Device.ImmediateContext.CopySubresourceRegion(signsTex.Resource, 0, this.Resource, 0, 0, 0, 0);
        }
    }
}
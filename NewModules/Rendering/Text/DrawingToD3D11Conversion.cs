using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.Rendering.Text
{
    /// <summary>
    /// Responsible for System.Drawing -> DirectD11 conversion
    /// Conversion steps:
    /// - Get bitmap into byte[] using Marshalling (copy native to managed memory)
    /// - Use MapSubresource to get this byte[] onto a Texture2D from D3D11
    /// </summary>
    public class DrawingToD3D11Conversion
    {

        /// <summary>
        /// Sets given raw byte[] array as data to be used by the texture.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="rgbValues">Array containing the values for each texel, rows placed sequentially</param>
        private void setTextureRawData(Texture2D tex, byte[] rgbValues)
        {
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

        private byte[] getBitmapRawBytes(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return rgbValues;
        }


        /// <summary>
        /// The bitmap and the texture should be same size.
        /// The Texture format should be B8G8R8A8_UNorm
        /// Bitmap pixelformat is read as PixelFormat.Format32bppArgb, so if this is the native format maybe speed is higher?
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="tex"></param>
        public void WriteBitmapToTexture(Bitmap bmp, Texture2D tex)
        {
            System.Diagnostics.Debug.Assert(tex.Description.Format == Format.B8G8R8A8_UNorm);

            var bytes = getBitmapRawBytes(bmp);
            setTextureRawData(tex, bytes);

        }
    }
}

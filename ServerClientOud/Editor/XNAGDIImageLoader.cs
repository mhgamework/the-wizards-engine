using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class XNAGDIImageLoader
    {
        public static GraphicsDevice GetDevice( IntPtr wnd )
        {
            return new GraphicsDevice( GraphicsAdapter.DefaultAdapter,
                                                    DeviceType.Hardware, wnd, new PresentationParameters() );
        }


        public unsafe static Image LoadImage( string fileName, IntPtr windowHandle )
        {


            // See the previous toturial on creating a device in memory Here


            GraphicsDevice dev = GetDevice( windowHandle );



            // Now that we have the device, we can use XNA to load a texture
            TextureCreationParameters p = TextureCreationParameters.Default;
            p.Format = SurfaceFormat.Color;

            Texture2D tex = Texture2D.FromFile( dev, fileName,p );

            // Lets allocate room to hold the bits of the texture


            uint[] d = new uint[ tex.Width * tex.Height ];


            // Now we populate the memory we allocated with the actual texture bits


            tex.GetData<uint>( d );


            // Now its time to create the GDI+ bitmap. Since we are only supporting 32 bit at this time we will create a 32 bit image with the same width and height as the texture


            Bitmap bmp = new Bitmap( tex.Width,
                            tex.Height,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb );


            // In order to read the bits from the GDI+ image in memory, we need to lock the bitmap


            System.Drawing.Imaging.BitmapData bmpd =
                                       bmp.LockBits( new System.Drawing.Rectangle( 0, 0,
                                       bmp.Width, bmp.Height ),
                                       System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                       System.Drawing.Imaging.PixelFormat.Format32bppArgb );


            // Lets get a pointer to the pixel bits and copy the XNA texture bits over 


            uint* ptr = (uint*)bmpd.Scan0.ToPointer();

            for ( int x = 0; x < tex.Width; x++ )
                for ( int y = 0; y < tex.Height; y++ )
                {
                    ptr[ x + y * tex.Width ] = d[ x + y * tex.Width ];
                }


            // Dont forget to clean up the GDI+ and XNA images


            bmp.UnlockBits( bmpd );
            dev.Dispose();


            // Finally we create the Image from the bitmap


            return Image.FromHbitmap( bmp.GetHbitmap() );
        }
    }
}

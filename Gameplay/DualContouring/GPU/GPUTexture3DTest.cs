using System;
using System.IO;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.IO;
using Microsoft.SqlServer.Server;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using SlimDX;
using Format = SlimDX.DXGI.Format;
using ImageFileFormat = SlimDX.Direct3D11.ImageFileFormat;
using Texture3D = SlimDX.Direct3D11.Texture3D;

namespace MHGameWork.TheWizards.DualContouring.GPU
{
    [TestFixture]
    public class GPUTexture3DTest
    {
        [Test]
        public void TestTexture3D()
        {
            var game = new DX11Game();
            game.InitDirectX();
            int size = 64;
            saveTest3DTexture(game, size, "X", generateData(size, (x, y, z) => x % 2 == 0));
            saveTest3DTexture(game, size, "Y", generateData(size, (x, y, z) => y % 2 == 0));
            saveTest3DTexture(game, size, "Z", generateData(size, (x, y, z) => z % 2 == 0));
            saveTest3DTexture(game, size, "Boundaries", generateDataBoxes(size));
        }

        private static void saveTest3DTexture(DX11Game game, int size, string name, byte[] data)
        {
            var texture3D = GPUTexture3D.CreateCPUWritable(game, size, size, size, Format.R8G8B8A8_UNorm);
            texture3D.SetTextureRawData(data);
            Texture3D.SaveTextureToFile(game.Device.ImmediateContext, texture3D.Resource, ImageFileFormat.Dds,
                TWDir.Test.CreateSubdirectory("GPU").CreateFile(name + ".dds").FullName);
            texture3D.SaveToImageSlices(game,TWDir.Test.CreateSubdirectory("GPU").CreateSubdirectory(name));
        }

        private static byte[] generateData(int size, Func<int, int, int, bool> red)
        {
            byte[] data = new byte[size * size * size * 4];
            for (int i = 0; i < size * size * size; i++)
            {
                int x = i % size;
                int y = i % (size * size) / size;
                int z = i / (size * size);
                //data[i] = 128;
                /*data[i * 4] = x == 0 || x == 64 ? (byte)255 : (byte)0;
                data[i * 4 + 1] = y == 0 || y == 64 ? (byte)255 : (byte)0;
                data[i * 4 + 2] = z == 0 || z == 64 ? (byte)255 : (byte)0;
                data[i * 4 + 3] = 255;*/
                data[i * 4] = red(x, y, z) ? (byte)255 : (byte)0;
                /*data[i] = i%64 == 0
                    ? (byte) 255
                    : (byte) 0
                      + (i + 1)%(64*64) == 0
                        ? (byte) 255
                        : (byte) 0;*/
                //+ (i+2) % 64 == 0 ? (byte)255 : (byte)0;
            }
            return data;
        }
        private static byte[] generateDataBoxes(int size)
        {
            byte[] data = new byte[size * size * size * 4];
            for (int i = 0; i < size * size * size; i++)
            {
                int x = i % size;
                int y = i % (size * size) / size;
                int z = i / (size * size);
                //data[i] = 128;
                /*data[i * 4] = x == 0 || x == 64 ? (byte)255 : (byte)0;
                data[i * 4 + 1] = y == 0 || y == 64 ? (byte)255 : (byte)0;
                data[i * 4 + 2] = z == 0 || z == 64 ? (byte)255 : (byte)0;
                data[i * 4 + 3] = 255;*/
                data[i * 4] = x == 0 || x == size - 1 || y == 0 || y == size - 1 || z == 0 || z == size - 1 ? (byte)255 : (byte)0;
                /*data[i] = i%64 == 0
                    ? (byte) 255
                    : (byte) 0
                      + (i + 1)%(64*64) == 0
                        ? (byte) 255
                        : (byte) 0;*/
                //+ (i+2) % 64 == 0 ? (byte)255 : (byte)0;
            }
            return data;
        }

    }


}
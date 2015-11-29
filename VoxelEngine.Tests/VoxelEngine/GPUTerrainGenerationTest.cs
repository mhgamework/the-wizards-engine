using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.GPU;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.VoxelEngine
{
    public class GPUTerrainGenerationTest : EngineTestFixture
    {
        [Test]
        public void TestCalculateDensityGridSigns()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var size = 512;
            var offset = new Vector3(0, 0, 0);
            var scaling = new Vector3(1, 1, 1);
            var density = "20-v.y";
            var target = new GPUHermiteCalculator(game);

            var tex = target.CreateDensitySignsTexture(size);
            target.WriteHermiteSigns(size, offset, scaling, density, tex);


            tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Signs"));
        }

        [Test]
        public void TestCalculateIntersections()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var size = 64;
            var target = new GPUHermiteCalculator(game);

            var signsTex = target.CreateDensitySignsTexture(size);
            target.WriteHermiteSigns(size, new Vector3(), new Vector3(), "", signsTex);

            var intersectionsTex = target.CreateIntersectionsTexture(size);
            var normals1Tex = target.CreateNormalsTexture(size);
            var normals2Tex = target.CreateNormalsTexture(size);
            var normals3Tex = target.CreateNormalsTexture(size);
            target.WriteHermiteIntersections(size, signsTex, intersectionsTex, normals1Tex, normals2Tex, normals3Tex);


            signsTex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Signs"));
            intersectionsTex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Intersections"));
            normals1Tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Normals1"));
            normals2Tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Normals2"));
            normals3Tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Normals3"));
        }

        [Test]
        public void TestToHermiteGrid()
        {
            var size = 64;
            var offset = new Vector3(0, 0, 0);
            var scaling = new Vector3(1, 1, 1);
            var density = "20-v.y";
            var target = new GPUHermiteCalculator(TW.Graphics);

            var signsTex = target.CreateDensitySignsTexture(size);
            target.WriteHermiteSigns(size, offset, scaling, density, signsTex);

            var intersectionsTex = target.CreateIntersectionsTexture(size);
            var normals1Tex = target.CreateNormalsTexture(size);
            var normals2Tex = target.CreateNormalsTexture(size);
            var normals3Tex = target.CreateNormalsTexture(size);
            target.WriteHermiteIntersections(size, signsTex, intersectionsTex, normals1Tex, normals2Tex, normals3Tex);

            signsTex.SaveToImageSlices(TW.Graphics, TWDir.Test.CreateSubdirectory("DualContouring.GPU/ToHermite/Signs"));
            intersectionsTex.SaveToImageSlices(TW.Graphics, TWDir.Test.CreateSubdirectory("DualContouring.GPU/ToHermite/Intersections"));
            normals1Tex.SaveToImageSlices(TW.Graphics, TWDir.Test.CreateSubdirectory("DualContouring.GPU/ToHermite/Normals1"));
            normals2Tex.SaveToImageSlices(TW.Graphics, TWDir.Test.CreateSubdirectory("DualContouring.GPU/ToHermite/Normals2"));
            normals3Tex.SaveToImageSlices(TW.Graphics, TWDir.Test.CreateSubdirectory("DualContouring.GPU/ToHermite/Normals3"));

            var signs = target.ReadDataThroughStageBuffer(signsTex);
            var intersections = target.ReadDataThroughStageBuffer(intersectionsTex);
            var normals1 = target.ReadDataThroughStageBuffer(normals1Tex);
            var normals2 = target.ReadDataThroughStageBuffer(normals2Tex);
            var normals3 = target.ReadDataThroughStageBuffer(normals3Tex);

            TW.Graphics.Device.ImmediateContext.ClearState();
            TW.Graphics.SetBackbuffer();

            var grid = HermiteDataGrid.CopyGrid(new DelegateHermiteGrid(
                                                    delegate(Point3 p)
                                                    {
                                                        p.Y = size - 1 - p.Y;
                                                        return //(p.X >= size || p.Y >= size || p.Z >= size)
                                                                (p.X >= size || p.Y < 0 || p.Z >= size)
                                                                   ? false
                                                                   : signs[(p.X + size * (p.Y + size * p.Z)) * 4] == 0;
                                                    },
                                                    delegate(Point3 p, int edge)
                                                    {
                                                        p.Y = size - 1 - p.Y;
                                                        if (edge < 0 || edge > 2) throw new InvalidOperationException();
                                                        // Assume edges 0,1,2 are 0+x,0+y,0+z
                                                        int bufferOffset = (p.X + size * (p.Y + size * p.Z)) * 4;

                                                        var norm = new Vector3();
                                                        byte[] nBuffer = null;
                                                        switch (edge)
                                                        {
                                                            case 0:
                                                                nBuffer = normals1;
                                                                break;
                                                            case 1:
                                                                nBuffer = normals2;
                                                                break;
                                                            case 2:
                                                                nBuffer = normals3;
                                                                break;
                                                        }

                                                        norm.X = nBuffer[bufferOffset + 0] / 255f;
                                                        norm.Y = nBuffer[bufferOffset + 1] / 255f;
                                                        norm.Z = nBuffer[bufferOffset + 2] / 255f;
                                                        norm = (norm - new Vector3(0.5f)) * 2;


                                                        /*
                                                        // packing code with z removal, for later use maybe
                                                        var sign = (normals2[bufferOffset + 2] & (1 << edge)) > 0 ? 1 : -1;
                                                        var xsqysq = norm.X * norm.X + norm.Y * norm.Y;
                                                        if (xsqysq > 1) xsqysq = 1;
                                                        if (xsqysq < 0) xsqysq = 0;
                                                        norm.Z = sign * (float)Math.Sqrt(1 - xsqysq);*/

                                                        //if (Math.Abs(norm.Length() - 1) > 0.01) throw new InvalidOperationException();

                                                        float intersection = intersections[bufferOffset + edge] / 255f;
                                                        return new Vector4(norm, intersection);
                                                    },
                new Point3(size, size, size)));




            var env = new DualContouringTestEnvironment();
            env.CameraLightSimulator.Light.LightRadius = 300;
            env.CameraLightSimulator.Light.LightIntensity = 1;
            env.Grid = grid;
            env.AddToEngine(EngineFactory.CreateEngine());
        }


        [Test]
        public void TestInteractive()
        {
            var engine = EngineFactory.CreateEngine();
            var game = TW.Graphics;
            var size = 32;
            var offset = new Vector3(0, 0, 0);
            var scaling = new Vector3(1, 1, 1);
            var density = "20-v.y";
            var target = new GPUHermiteCalculator(game);

            var tex3d = target.CreateDensitySignsTexture(size);
            engine.AddSimulator(() =>
                {
                    target.WriteHermiteSigns(size, offset, scaling, density, tex3d);
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
                        tex3d.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU"));

                    TW.Graphics.Device.ImmediateContext.ClearState();
                    TW.Graphics.SetBackbuffer();
                }, "Generate");

            //tex3d.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU"));
        }


        [Test]
        public void TestRender()
        {
            var size = 128;
            var offset = new Vector3(0, 0, 0);
            var scaling = new Vector3(1, 1, 1);
            var density = "20-v.y";
            var target = new GPUHermiteCalculator(TW.Graphics);

            var grid = target.CalculateHermiteGrid(size, offset, scaling, density);

            var env = new DualContouringTestEnvironment();
            env.Grid = grid;
            env.AddToEngine(engine);

        }

        /// <summary>
        /// @128
        /// GPU: 15,45 ms => should be 5 perlin noise
        /// CPU: 6.71 s => should be 5 perlin noise
        /// x430
        /// @256
        /// GPU: 99.65 ms
        /// CPU: 55.20 s
        /// x553
        /// </summary>
        [Test]
        public void TestPerformanceSigns()
        {
            var size = 128;
            var offset = new Vector3(0, 0, 0);
            var scaling = new Vector3(1, 1, 1);
            var density = "20-v.y";
            var target = new GPUHermiteCalculator(TW.Graphics);

            var signsTex = target.CreateDensitySignsTexture(size);

            var times = 10f;
            var texture = signsTex;
            var cache = GPUTexture3D.CreateCPUReadable(TW.Graphics, texture.Resource.Description.Width, texture.Resource.Description.Height, texture.Resource.Description.Depth, texture.Resource.Description.Format);

            var gpuSpeed = PerformanceHelper.Measure(() =>
             {
                 for (int i = 0; i < times; i++)
                 {

                     //cache.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("Temp"));

                     target.WriteHermiteSigns(size, offset, scaling, density, signsTex);
                     TW.Graphics.Device.ImmediateContext.CopySubresourceRegion(texture.Resource, 0, cache.Resource, 0, 0, 0, 0);

                     //var signs = target.ReadDataThroughStageBuffer(signsTex);    
                     var signs = cache.GetRawData();
                 }
             }).Multiply(1 / times);
            gpuSpeed.PrettyPrint().Print();
            Console.WriteLine("Cube of {0:#.0}³ per second", Math.Pow(size * size * size / gpuSpeed.TotalSeconds, 1 / 3f));

            //return;

            // Should be roughly the same calculation if the gpu is alculating 5-noise

            var dens = VoxelTerrainGenerationTest.createDensityFunction5Perlin(11, 10);


            var outs = new bool[size * size * size];
            PerformanceHelper.Measure(() =>
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int z = 0; z < size; z++)
                        {
                            outs[z + size * (y + size * x)] = dens(new Vector3(x, y, z)) > 0;
                        }
                    }
                }
            }).PrettyPrint().Print();


        }

        /// <summary>
        /// Extracts sign data and renders as voxel mesh, uses 0 for edge info
        /// </summary>
        [Test]
        public void TestRenderSimple()
        {
            var size = 128;
            var offset = new Vector3(0, 0, 0);
            var scaling = new Vector3(1, 1, 1);
            var density = "20-v.y";
            var target = new GPUHermiteCalculator(TW.Graphics);

            var signsTex = target.CreateDensitySignsTexture(size);

            var times = 10f;
            var texture = signsTex;
            var cache = GPUTexture3D.CreateCPUReadable(TW.Graphics, texture.Resource.Description.Width, texture.Resource.Description.Height, texture.Resource.Description.Depth, texture.Resource.Description.Format);


            //cache.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("Temp"));

            target.WriteHermiteSigns(size, offset, scaling, density, signsTex);
            TW.Graphics.Device.ImmediateContext.CopySubresourceRegion(texture.Resource, 0, cache.Resource, 0, 0, 0, 0);

            //var signs = target.ReadDataThroughStageBuffer(signsTex);    
            var signs = cache.GetRawData();
            var contents = toRunLengthString( signs.Where( ( k, i ) => i%4 == 0 ) );
            File.WriteAllText(TestDirectory.CreateFile("TestRenderSimpleSigns.txt").FullName, contents);

            // Use -1 because we have a grid of size-1, with size number of corners = signs
            var grid = new DelegateHermiteGrid(p => signs[(p.X + size * (p.Y + size * p.Z)) * 4] > 128, (p, i) => new Vector4(), new Point3(size-1, size-1, size-1)); 


            var env = new DualContouringTestEnvironment();
            env.Grid = HermiteDataGrid.CopyGrid(grid);
            env.AddToEngine(engine);


        }

        /// <summary>
        /// Converts byte array to run length string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="runlengthThreshold">Only group after x equal entries</param>
        /// <returns></returns>
        private static string toRunLengthString( IEnumerable<byte> data, int runlengthThreshold = 10 )
        {
            var builder = new StringBuilder();
            var last = -1;
            var count = 0;
            data.ForEach( b =>
            {
                if ( b == last ) count++;
                else
                {
                    if ( last != -1 )
                    {
                        if ( count > runlengthThreshold )
                            builder.Append( last ).Append( ':' ).Append( count ).Append( ' ' );
                        else
                            for ( int i = 0; i < count; i++ )
                            {
                                builder.Append( last ).Append( ' ' );
                            }
                    }
                    count = 1;
                    last = b;
                }
            } );


            builder.Append( last ).Append( ':' ).Append( count ).Append( ' ' );
            var contents = builder.ToString();
            return contents;
        }
    }
}
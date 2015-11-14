using System;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.GPU;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.VoxelEngine
{
    public class GPUTest : EngineTestFixture
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
            target.WriteHermiteIntersections(size,signsTex, intersectionsTex, normals1Tex, normals2Tex,normals3Tex);


            signsTex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Signs"));
            intersectionsTex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Intersections"));
            normals1Tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Normals1"));
            normals2Tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Normals2"));
            normals3Tex.SaveToImageSlices(game, TWDir.Test.CreateSubdirectory("DualContouring.GPU/Normals3"));
        }

        [Test]
        public void TestToHermiteGrid()
        {
            var size = 128;
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
            target.WriteHermiteIntersections(size, signsTex,intersectionsTex, normals1Tex, normals2Tex,normals3Tex);

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
                                                        norm = (norm - new Vector3(0.5f))*2;


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
            env.AddToEngine(EngineFactory.CreateEngine());

        }
    }
}
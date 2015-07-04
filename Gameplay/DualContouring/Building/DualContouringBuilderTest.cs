using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    /// <summary>
    /// Showcase for building with the dual contouring algorithm!
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class DualContouringBuilderTest
    {
        private VoxelCustomRenderer surfaceRenderer;
        private int chunkSize = -1;
        private float voxelSize = -1;
        private Point3 NumChunks;
        private Array3D<Chunk> chunks;

        public DualContouringBuilderTest()
        {
            chunkSize = BuilderConfiguration.ChunkNumVoxels;
            voxelSize = BuilderConfiguration.VoxelSize;
            NumChunks = BuilderConfiguration.NumChunks;
            surfaceRenderer = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(surfaceRenderer);

            initDefaultWorld();

            EngineFactory.CreateEngine().AddSimulator(processUserInput, "UserInput");
            EngineFactory.CreateEngine().AddSimulator(new WorldRenderingSimulator());

            PlaceInWorld(createUnitBox(), new Point3(0, 20, 0));
        }

        private void processUserInput()
        {
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();

            var raycaster = new Raycaster<Chunk>();

            chunks.ForEach((c, p) =>
                {
                    TW.Graphics.LineManager3D.AddBox(c.Box, Color.Black);
                    c.Raycast(raycaster, ray);
                });
            if (raycaster.GetClosest().IsHit)
            {
                Vector3 hitpoint = ray.GetPoint(raycaster.GetClosest().Distance);
                TW.Graphics.LineManager3D.AddCenteredBox(hitpoint, 0.1f,
                                                          Color.Yellow);

                Vector3 placePosition = (hitpoint / 0.5f).ToFloored().ToVector3() * .5f;
                TW.Graphics.LineManager3D.AddBox(new BoundingBox(placePosition + new Vector3(0.5f / 5 / 2), placePosition + new Vector3(0.5f + 0.5f / 5 / 2)), Color.GreenYellow);
                if (TW.Graphics.Mouse.LeftMouseJustPressed)
                {
                    var resolution = 10;
                    var placer = new BasicShapeBuilder().CreateCube(5);
                    PlaceInWorld(placer, ToFlooredWorldPoint(placePosition));

                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    var resolution = 10;
                    var placer = new BasicShapeBuilder().CreateCube(5);
                    RemoveFromWorld(placer, ToFlooredWorldPoint(placePosition));

                }
            }

        }
        private Point3 ToFlooredWorldPoint(Vector3 v)
        {
            return (v / voxelSize).ToFloored();
        }

        private static HermiteDataGrid createUnitBox()
        {
            var placer = new BasicShapeBuilder().CreateSphere(5);
            return placer;
        }

        public void PlaceInWorld(HermiteDataGrid source, Point3 offset)
        {
            var bb = new BoundingBox(offset.ToVector3() * BuilderConfiguration.VoxelSize, (offset + source.Dimensions).ToVector3() * BuilderConfiguration.VoxelSize);
            /*var c = chunks[new Point3()];
            //c.SetGrid(source);
            c.SetGrid(HermiteDataGrid.CopyGrid(new UnionGrid(c.Grid, source, offset)));
            c.UpdateSurface(surfaceRenderer);*/
            chunks.ForEach((c, p) =>
                {
                    if (c.Box.xna().Contains(bb.xna()) == ContainmentType.Disjoint) return;
                    c.SetGrid(HermiteDataGrid.CopyGrid(new UnionGrid(c.Grid, source, offset - c.ChunkCoord * BuilderConfiguration.ChunkNumVoxels)));
                    c.UpdateSurface(surfaceRenderer);
                });
        }
        public void RemoveFromWorld(HermiteDataGrid source, Point3 offset)
        {
            var bb = new BoundingBox(offset.ToVector3() * BuilderConfiguration.VoxelSize, (offset + source.Dimensions).ToVector3() * BuilderConfiguration.VoxelSize);
            /*var c = chunks[new Point3()];
            //c.SetGrid(source);
            c.SetGrid(HermiteDataGrid.CopyGrid(new UnionGrid(c.Grid, source, offset)));
            c.UpdateSurface(surfaceRenderer);*/
            chunks.ForEach((c, p) =>
            {
                if (c.Box.xna().Contains(bb.xna()) == ContainmentType.Disjoint) return;
                c.SetGrid(HermiteDataGrid.CopyGrid(new DifferenceGrid(c.Grid, source, offset - c.ChunkCoord * BuilderConfiguration.ChunkNumVoxels)));
                c.UpdateSurface(surfaceRenderer);
            });
        }

        private void initDefaultWorld()
        {
            chunks = new Array3D<Chunk>(NumChunks);

            var totalTime = new TimeSpan(0);
            var i = 0;
            chunks.ForEach((c, p) =>
                {
                    i++;
                    totalTime.Add(PerformanceHelper.Measure(() =>
                        {
                            var grid = HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(v =>
                                {
                                    v += p.ToVector3()*chunkSize;
                                    return 20 - v.Y;
                                }, new Point3(chunkSize + 1, chunkSize + 1, chunkSize + 1)));

                            var chunk = new Chunk(p);
                            chunk.SetGrid(grid);
                            chunk.UpdateSurface(surfaceRenderer);
                            chunks[p] = chunk;


                        }));


                });
            totalTime.Multiply(1f / i);


        }

        [Test]
        public void Run()
        {

        }
    }
}
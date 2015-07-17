using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring._Test;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;
using VoxelEngine.Tests;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    /// <summary>
    /// Showcase for building with the dual contouring algorithm!
    /// </summary>
    public class DualContouringBuilderTest : EngineTestFixture
    {
        private VoxelCustomRenderer surfaceRenderer;
        private int chunkSize = -1;
        private float voxelSize = -1;
        private Point3 NumChunks;
        private Array3D<Chunk> chunks;
        private int placementGridSize = 4;
        private InteractiveTestingEnvironment interactiveTestingEnv;
        [SetUp]
        public void SetUp()
        {
            chunkSize = BuilderConfiguration.ChunkNumVoxels;
            voxelSize = BuilderConfiguration.VoxelSize;
            NumChunks = BuilderConfiguration.NumChunks;
            surfaceRenderer = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(surfaceRenderer);

            initDefaultWorld();

            EngineFactory.CreateEngine().AddSimulator(processUserInput, "UserInput");
            interactiveTestingEnv = new InteractiveTestingEnvironment();
            interactiveTestingEnv.LoadIntoEngine(EngineFactory.CreateEngine());
            EngineFactory.CreateEngine().AddSimulator(new WorldRenderingSimulator());


            //TODO: add commands!

            //PlaceInWorld(createUnitBox(), new Point3(0, 20, 0));
        }

        private void processUserInput()
        {
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();

            if (TW.Graphics.Mouse.RelativeScrollWheel > 0) placementGridSize *= 2;
            if (TW.Graphics.Mouse.RelativeScrollWheel < 0) placementGridSize /= 2;
            placementGridSize = (int)MathHelper.Clamp(placementGridSize, 1, 20);

            var raycaster = new Raycaster<Chunk>();

            chunks.ForEach((c, p) =>
                {
                    TW.Graphics.LineManager3D.AddBox(c.Box, Color.Black);
                    c.Raycast(raycaster, ray);
                });
            if (raycaster.GetClosest().IsHit)
            {
                Vector3 hitpoint = ray.GetPoint(raycaster.GetClosest().Distance);
                Vector3 normal = -raycaster.GetClosest().HitNormal; //TODO: invert hack

                TW.Graphics.LineManager3D.AddCenteredBox(hitpoint, 0.02f, Color.Yellow);
                TW.Graphics.LineManager3D.AddLine(hitpoint, hitpoint + normal * 0.06f, Color.CadetBlue);


                var targetCube = CalculatePlacementCube(hitpoint - normal * voxelSize * 0.1f, placementGridSize);

                var halfVoxel = new Vector3(voxelSize / 2);
                var placementWorldSize = placementGridSize * voxelSize;
                var targetBoundingBox = new BoundingBox((Vector3)targetCube.ToVector3() * placementWorldSize + halfVoxel,
                    (targetCube + new Vector3(1, 1, 1)) * placementWorldSize + halfVoxel);

                TW.Graphics.LineManager3D.AddBox(targetBoundingBox, Color.GreenYellow);



                if (TW.Graphics.Mouse.LeftMouseJustPressed)
                {
                    var addCube = CalculatePlacementCube(hitpoint + normal * 0.06f, placementGridSize);
                    Point3 placeOffset = (addCube.ToVector3() * placementWorldSize / voxelSize).ToPoint3Rounded();

                    var placer = new BasicShapeBuilder().CreateCube(placementGridSize);
                    PlaceInWorld(placer, placeOffset);

                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    Point3 placeOffset = (targetCube.ToVector3() * placementWorldSize / voxelSize).ToPoint3Rounded();

                    var placer = new BasicShapeBuilder().CreateCube(placementGridSize);
                    RemoveFromWorld(placer, placeOffset);

                }
            }

        }

        public Point3 CalculatePlacementCube(Vector3 hitpoint, int cubeSize)
        {
            var halfVoxel = new Vector3(voxelSize / 2);
            var placementWorldSize = placementGridSize * voxelSize;
            var placementCubeCoord = ((hitpoint - halfVoxel) / placementWorldSize).ToFloored().ToVector3();
            return placementCubeCoord.ToPoint3Rounded();
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
                                    v += (Vector3)p.ToVector3() * chunkSize;
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
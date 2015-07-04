using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DualContouring._Test
{
    /// <summary>
    /// Showcase for building with the dual contouring algorithm!
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class DualContouringBuilderTest
    {
        private VoxelCustomRenderer surfaceRenderer;
        private int chunkSize = 32;
        private float voxelSize = 0.1f;
        private Point3 NumChunks = new Point3(5, 1, 5);
        private Array3D<Chunk> chunks;

        public DualContouringBuilderTest()
        {
            surfaceRenderer = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(surfaceRenderer);

            initDefaultWorld();

            EngineFactory.CreateEngine().AddSimulator(processUserInput, "UserInput");
            EngineFactory.CreateEngine().AddSimulator(new WorldRenderingSimulator());
        }

        private void processUserInput()
        {
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();

            var raycaster = new Raycaster<Chunk>();

            chunks.ForEach((c, p) =>
                {
                    TW.Graphics.LineManager3D.AddBox(c.Box, Color.Black);
                    float? dist = ray.xna().Intersects(c.Box.xna());
                    if (!dist.HasValue) return;
                    if (c.Surface == null) return;

                    raycaster.AddIntersection(ray, c.Surface.Mesh.Positions, c.Surface.WorldMatrix, c);
                });
            if (raycaster.GetClosest().IsHit)
                TW.Graphics.LineManager3D.AddCenteredBox(ray.GetPoint(raycaster.GetClosest().Distance), 0.1f, Color.Yellow);
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
            {

                //var grid = HermiteDataGrid.CopyGrid(new);

            }
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
                            var grid = HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(v => 20 - v.Y, new Point3(chunkSize + 1, chunkSize + 1, chunkSize + 1)));
                            var surface = surfaceRenderer.CreateSurface(grid,
                                                           Matrix.Scaling(new Vector3(voxelSize)) *
                                                           Matrix.Translation(p.ToVector3() * chunkSize * voxelSize));

                            chunks[p] = new Chunk(grid, p.ToVector3() * voxelSize * chunkSize, voxelSize * chunkSize) { Surface = surface };

                        }));


                });
            totalTime.Multiply(1f / i);


        }

        [Test]
        public void Run()
        {

        }


        public class Chunk
        {
            private readonly HermiteDataGrid grid;
            public Vector3 Position;
            public BoundingBox Box;
            public VoxelSurface Surface;

            public Chunk(HermiteDataGrid grid, Vector3 position, float size)
            {
                this.grid = grid;
                Position = position;
                Box = new BoundingBox(position, position + new Vector3(size));
            }
        }

        public class Raycaster<T>
        {
            private RaycastResult closest = new RaycastResult();
            private RaycastResult cache = new RaycastResult();

            public void AddIntersection(Ray ray, Vector3[] vertices, Matrix world, object obj)
            {
                var localRay = ray.Transform(Matrix.Invert(world));
                Vector3 v0;
                Vector3 v1;
                Vector3 v2;
                var dist = MeshRaycaster.RaycastMeshPart(vertices, localRay, out v0, out v1, out v2);
                if (!dist.HasValue) return;
                var point = Vector3.TransformCoordinate(localRay.GetPoint(dist.Value), world);
                cache.Set(Vector3.Distance(ray.Position, point), obj);
                if (!cache.IsCloser(closest)) return;
                closest.Set(cache.Distance, cache.Object);
                closest.V1 = Vector3.TransformCoordinate(v0, world);
                closest.V2 = Vector3.TransformCoordinate(v1, world);
                closest.V3 = Vector3.TransformCoordinate(v2, world);
            }

            public void AddIntersection(float? dist, T obj)
            {
                cache.Set(dist, obj);
                if (cache.IsCloser(closest))
                    closest = cache;
            }

            public RaycastResult GetClosest()
            {
                return closest;
            }
        }
    }
}
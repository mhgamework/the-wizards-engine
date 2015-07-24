using System;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    public class FiniteWorld
    {
        private Point3 NumChunks { get { return BuilderConfiguration.NumChunks; } }
        public Array3D<Chunk> Chunks;
        public void InitDefaultWorld()
        {
            Chunks = new Array3D<Chunk>(NumChunks);

            var totalTime = new TimeSpan(0);
            var i = 0;
            Chunks.ForEach((c, p) =>
            {
                i++;
                totalTime.Add(PerformanceHelper.Measure(() =>
                {
                    var grid = HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(v =>
                    {
                        v += (Vector3)p.ToVector3() * BuilderConfiguration.ChunkNumVoxels;
                        return 20 - v.Y;
                    }, new Point3(BuilderConfiguration.ChunkNumVoxels + 1, BuilderConfiguration.ChunkNumVoxels + 1, BuilderConfiguration.ChunkNumVoxels + 1)));

                    var chunk = new Chunk(p);
                    chunk.SetGrid(grid);
                    Chunks[p] = chunk;


                }));


            });
            totalTime.Multiply(1f / i);


        }
    }
}
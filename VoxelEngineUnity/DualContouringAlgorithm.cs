using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.Voxels;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Generates a surface from a Hermite Grid as in the Dual Contouring algorithm.
    /// </summary>
    public class DualContouringAlgorithm
    {
        public void GenerateSurface(List<global::MHGameWork.TheWizards.Vector3_Adapter> vertices, List<int> indices, AbstractHermiteGrid grid)
        {
            var mats = new List<global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter>();
            GenerateSurface(vertices, indices, mats, grid);
        }
        public void GenerateSurface(List<global::MHGameWork.TheWizards.Vector3_Adapter> vertices, List<int> indices, List<global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter> triangleMaterials, AbstractHermiteGrid grid)
        {
            var vIndex = new Dictionary<global::DirectX11.Point3_Adapter, int>();

            createQEFVertices(vertices, grid, vIndex);

            buildTriangleIndices(indices, grid, vIndex, triangleMaterials);
        }

        private static void buildTriangleIndices(List<int> indices, AbstractHermiteGrid grid, Dictionary<global::DirectX11.Point3_Adapter, int> vIndex, List<global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter> triangleMaterials)
        {
            // Possible quads
            var offsets = new[] { global::DirectX11.Point3_Adapter.UnitX(), global::DirectX11.Point3_Adapter.UnitY(), global::DirectX11.Point3_Adapter.UnitZ(), };
            var rights = new[] { global::DirectX11.Point3_Adapter.UnitY(), global::DirectX11.Point3_Adapter.UnitZ(), global::DirectX11.Point3_Adapter.UnitX(), };
            var ups = new[] { global::DirectX11.Point3_Adapter.UnitZ(), global::DirectX11.Point3_Adapter.UnitX(), global::DirectX11.Point3_Adapter.UnitY(), };
            var unitEdges = offsets.Select(o => grid.GetEdgeId(new global::DirectX11.Point3_Adapter(), o)).ToArray();
            //TODO: should be unit test
            for (int i = 0; i < 3; i++)
            {
                Debug.Assert(grid.GetEdgeOffsets(unitEdges[i])[0] == new global::DirectX11.Point3_Adapter());
                Debug.Assert(grid.GetEdgeOffsets(unitEdges[i])[1] == offsets[i]);
            }

            grid.ForEachCube(o =>
                {
                    if (!vIndex.ContainsKey(o)) return; // No sign changes so no relevant edges here

                    for (int i = 0; i < 3; i++)
                    {
                        var edgeId = unitEdges[i];
                        if (!grid.HasEdgeData(o, edgeId)) continue;
                        // Generate quad

                        global::DirectX11.Point3_Adapter right = rights[i];
                        global::DirectX11.Point3_Adapter up = ups[i];

                        global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter mat;
                        var signs = grid.GetEdgeSigns(o, edgeId);
                        // Face towards air by swapping right and up
                        if (signs[1])
                        {
                            global::DirectX11.Point3_Adapter swap = right;
                            right = up;
                            up = swap;
                            mat = grid.GetMaterial(o + offsets[i]);
                        }
                        else
                        {
                            mat = grid.GetMaterial(o);
                        }

                        // build quad faces

                        global::DirectX11.Point3_Adapter a = o - right;
                        global::DirectX11.Point3_Adapter b = o - up;
                        global::DirectX11.Point3_Adapter ab = o - right - up;
                        if (!new[] { a, b, ab }.All(vIndex.ContainsKey))
                            continue; // This should never happen unless on the side of the field, maybe add a check for this?

                        indices.AddRange(new[] { vIndex[o], vIndex[a], vIndex[ab] });
                        indices.AddRange(new[] { vIndex[o], vIndex[ab], vIndex[b] });
                        triangleMaterials.Add(mat);
                        triangleMaterials.Add(mat);
                    }
                });
        }

        private static void createQEFVertices(List<global::MHGameWork.TheWizards.Vector3_Adapter> vertices, AbstractHermiteGrid grid, Dictionary<global::DirectX11.Point3_Adapter, int> vIndex)
        {
            var cubeSigns = new bool[8];

            var edgeVertexIds = grid.GetAllEdgeIds().Select(e => grid.GetEdgeVertexIds(e)).ToArray();
            var edgeOffsets = grid.GetAllEdgeIds().Select(e => grid.GetEdgeOffsets(e)).ToArray();

            int changingEdgeCount = 0;
            int[] changingEdges = new int[12];
            global::MHGameWork.TheWizards.Vector3_Adapter[] positions = new global::MHGameWork.TheWizards.Vector3_Adapter[12];
            global::MHGameWork.TheWizards.Vector3_Adapter[] normals = new global::MHGameWork.TheWizards.Vector3_Adapter[12];

            grid.ForEachCube(curr =>
                {
                    grid.GetCubeSigns(curr, cubeSigns);
                    bool allTrue = true;
                    bool allFalse = true;
                    for (int i = 0; i < 8; i++)
                    {
                        var sign = cubeSigns[i];
                        allTrue = sign && allTrue;
                        allFalse = !sign && allFalse;
                    }
                    if (allTrue || allFalse) return;// no sign changes
                    //if ( cubeSigns.All( v => v ) || !cubeSigns.Any( v => v ) ) return; // no sign changes

                    changingEdgeCount = 0;
                    for (int i = 0; i < edgeVertexIds.Length; i++)
                    {
                        var ids = edgeVertexIds[i];
                        if (cubeSigns[ids[0]] == cubeSigns[ids[1]]) continue;

                        changingEdges[changingEdgeCount] = i;
                        changingEdgeCount++;

                    }

                    for (int i = 0; i < changingEdgeCount; i++)
                    {
                        var iEdgeId = changingEdges[i];
                        var iEdgeOffsets = edgeOffsets[iEdgeId];
                        global::MHGameWork.TheWizards.Vector4_Adapter iEdgeData = grid.getEdgeData(curr, iEdgeId);
                        positions[i] = global::MHGameWork.TheWizards.Vector3_Adapter.Lerp(iEdgeOffsets[0], iEdgeOffsets[1], iEdgeData.W);
                        normals[i] = iEdgeData.TakeXYZ();
                    }


                    global::MHGameWork.TheWizards.Vector3_Adapter meanIntersectionPoint = new global::MHGameWork.TheWizards.Vector3_Adapter();
                    for (int i = 0; i < changingEdgeCount; i++)
                    {
                        meanIntersectionPoint = meanIntersectionPoint + positions[i];
                    }
                    meanIntersectionPoint = meanIntersectionPoint * (1f / changingEdgeCount);

                    global::MHGameWork.TheWizards.Vector3_Adapter qefPoint1 = new global::MHGameWork.TheWizards.Vector3_Adapter();

                    try
                    {
                        var leastsquares = QEFCalculator.CalculateCubeQEF(normals, positions, changingEdgeCount, meanIntersectionPoint);
                        qefPoint1 = new global::MHGameWork.TheWizards.Vector3_Adapter(leastsquares[0], leastsquares[1], leastsquares[2]);
                        if (qefPoint1[0] < 0 || qefPoint1[1] < 0 || qefPoint1[2] < 0
                            || qefPoint1[0] > 1 || qefPoint1[1] > 1 || qefPoint1[2] > 1)
                        {
                            qefPoint1 = meanIntersectionPoint; // I found someone online who does this too: http://ngildea.blogspot.be/2014/11/implementing-dual-contouring.html
                            //TODO: should probably fix the QEF, maybe by removing singular values

                            //ERROR!
                            //throw new InvalidOperationException("QEF returned solution outside of cube");
                        }
                    }
                    catch (Exception ex)
                    {
                        qefPoint1 = meanIntersectionPoint;
                        // Ignore error for now!!!
                    }




                    vIndex[curr] = vertices.Count;
                    vertices.Add(qefPoint1 + (global::MHGameWork.TheWizards.Vector3_Adapter)curr.ToVector3());
                });
        }

        /// <summary>
        /// Duplicate of the inlined version!!!!!! DANGEROUS
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="signs"></param>
        /// <param name="cube"></param>
        /// <returns></returns>
        public static global::MHGameWork.TheWizards.Vector3_Adapter calculateQefPoint(AbstractHermiteGrid grid, bool[] signs, global::DirectX11.Point3_Adapter cube)
        {
            //TODO: this can be optimized probably!
            var changingEdges = grid.GetAllEdgeIds().Where(e =>
                {
                    var ids = grid.GetEdgeVertexIds(e);
                    return signs[ids[0]] != signs[ids[1]];
                }).ToArray();


            var positions = changingEdges.Select(e => grid.GetEdgeIntersectionCubeLocal(cube, e)).ToArray();
            var normals = changingEdges.Select(e => grid.GetEdgeNormal(cube, e)).ToArray();

            global::MHGameWork.TheWizards.Vector3_Adapter meanIntersectionPoint = positions.Aggregate((a, b) => a + b) * (1f / positions.Length);
            var leastsquares = QEFCalculator.CalculateCubeQEF(normals, positions, meanIntersectionPoint);

            global::MHGameWork.TheWizards.Vector3_Adapter qefPoint = new global::MHGameWork.TheWizards.Vector3_Adapter(leastsquares[0], leastsquares[1], leastsquares[2]);

            if (qefPoint[0] < 0 || qefPoint[1] < 0 || qefPoint[2] < 0
                || qefPoint[0] > 1 || qefPoint[1] > 1 || qefPoint[2] > 1)
            {
                qefPoint = meanIntersectionPoint; // I found someone online who does this too: http://ngildea.blogspot.be/2014/11/implementing-dual-contouring.html
                //TODO: should probably fix the QEF, maybe by removing singular values

                //ERROR!
                //throw new InvalidOperationException("QEF returned solution outside of cube");
            }
            return qefPoint;
        }


    }
}
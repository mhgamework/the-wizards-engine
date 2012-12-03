using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;
using M = System.Math;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Implemented roughly according to
    /// 
    /// A Fast Voxel Traversal Algorithm for Ray Tracing
    /// John Amanatides, Andrew Woo
    /// Dept. of Computer Science
    /// University of Toronto
    /// Toronto, Ontario, Canada M5S 1A4
    /// </summary>
    public class GridTraverser
    {
        public Vector3 GridOffset { get; set; }
        public float NodeSize { get; set; }

        /// <summary>
        /// Trace should be in map coordinates
        /// </summary>
        /// <param name="trace"></param>
        /// <returns></returns>
        public bool Traverse(RayTrace trace, Func<Point3, bool> nodeCallback)
        {
            //trace.Ray = new Ray(trace.Ray.Position - GridOffset, trace.Ray.Direction);

            Vector3 tDelta;
            tDelta.X = NodeSize / M.Abs(trace.Ray.Direction.X); // how far we must move in the ray direction before we encounter a new voxel in x-direction
            tDelta.Y = NodeSize / M.Abs(trace.Ray.Direction.Y); // same but y-direction
            tDelta.Z = NodeSize / M.Abs(trace.Ray.Direction.Z);


            var start = trace.Ray.Position + trace.Ray.Direction * trace.Start;
            var end = trace.Ray.Position + trace.Ray.Direction * trace.End;
            var dir = trace.Ray.Direction;


            // We work in grid coordinates, with grid size rescaled to 1

            start -= GridOffset;
            end -= GridOffset;

            start /= NodeSize;
            end /= NodeSize;


            // start voxel coordinates
            Point3 pos = new Point3((int)start.X, (int)start.Y, (int)start.Z);

            // end voxel coordinates
            Point3 endPos = new Point3((int)end.X, (int)end.Y, (int)end.Z);

            // decide which direction to start walking in
            var step = new Point3(M.Sign(dir.X), M.Sign(dir.Y), M.Sign(dir.Z));


            // Distance to the next intersection
            Vector3 dist;

            // calculate distance to first intersection in the voxel we start from
            dist = pos;
            if (dir.X > 0)
                dist.X = dist.X + 1;
            if (dir.Y > 0)
                dist.Y = dist.Y + 1;
            if (dir.Z > 0)
                dist.Z = dist.Z + 1;

            Vector3 dirInverse = new Vector3(1 / dir.X, 1 / dir.Y, 1 /dir.Z);
            dist -= start;
            //dist = Vector3.Modulate(dist, step.ToVector3()); Signs are in the dirInverse :))
            dist = Vector3.Modulate(dist, dirInverse*NodeSize);
            if (dir.X == 0)
                dist.X = float.PositiveInfinity;
            if (dir.Y == 0)
                dist.Y = float.PositiveInfinity;
            if (dir.Z == 0)
                dist.Z = float.PositiveInfinity;

            bool reachedX=false, reachedY=false, reachedZ=false;
            while (true)
            {
                if (nodeCallback(pos)) break;



                if (step.X > 0)
                {
                    if (pos.X >= endPos.X)
                        reachedX = true;
                }
                else
                    if (pos.X <= endPos.X)
                        reachedX = true;


                if (step.Y > 0)
                {
                    if (pos.Y >= endPos.Y)
                        reachedY = true;
                }
                else
                    if (pos.Y <= endPos.Y)
                        reachedY = true;


                if (step.Z > 0)
                {
                    if (pos.Z >= endPos.Z)
                        reachedZ = true;
                }
                else
                    if (pos.Z <= endPos.Z)
                        reachedZ = true;


                if (reachedX && reachedY && reachedZ) break;

                if (dist.X < dist.Y && dist.X < dist.Z)
                {
                    // progress X
                    pos.X += step.X;
                    dist.X += tDelta.X;
                }
                else if (dist.Y < dist.Z)
                {
                    // progress Y
                    pos.Y += step.Y;
                    dist.Y += tDelta.Y;
                }
                else
                {
                    // progress Z
                    pos.Z += step.Z;
                    dist.Z += tDelta.Z;

                }




                
            }

            return true;
        }
    }
}

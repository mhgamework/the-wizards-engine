using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for placing/removing skew walls in dynamic blocks and making sure the adjacent blocks are modified correctly.
    /// </summary>
    public class SkewWallPlacer
    {
        private readonly DynamicBlockFactory dynBlockFactory;
        private readonly DynamicBlockResolver resolver;

        public SkewWallPlacer(DynamicBlockFactory dynBlockFactory, DynamicBlockResolver resolver)
        {
            this.dynBlockFactory = dynBlockFactory;
            this.resolver = resolver;
        }

        public void PlaceSkewWall(Point3 pos, WallType type)
        {
            var b = dynBlockFactory.GetBlockAtPosition(pos);
            if (b == null)
                b = dynBlockFactory.CreateNewDynamicBlock(pos);

            if (b.hasStraightWalls)
                return;
            b.hasSkewWalls = true;

            resolver.ResolveWalls(b, type);

            var x = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitZ));

            if (x != null)
                resolver.ResolveWalls(x, type);
            if (minX != null)
                resolver.ResolveWalls(minX, type);
            if (z != null)
                resolver.ResolveWalls(z, type);
            if (minZ != null)
                resolver.ResolveWalls(minZ, type);
        }

        public void RemoveSkewWall(Point3 pos)
        {
            var b = dynBlockFactory.GetBlockAtPosition(pos);
            if (b == null)
                return;

            b.SetSkew(DynamicBlockDirection.XMinZ, 0, null);
            b.SetSkew(DynamicBlockDirection.XMinZ, 1, null);
            b.SetSkew(DynamicBlockDirection.MinXMinZ, 0, null);
            b.SetSkew(DynamicBlockDirection.MinXMinZ, 1, null);
            b.SetSkew(DynamicBlockDirection.MinXZ, 0, null);
            b.SetSkew(DynamicBlockDirection.MinXZ, 1, null);
            b.SetSkew(DynamicBlockDirection.XZ, 0, null);
            b.SetSkew(DynamicBlockDirection.XZ, 1, null);

            b.hasSkewWalls = false;


            var x = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitZ));

            if (x != null)
                resolver.ResolveWalls(x, null);
            if (minX != null)
                resolver.ResolveWalls(minX, null);
            if (z != null)
                resolver.ResolveWalls(z, null);
            if (minZ != null)
                resolver.ResolveWalls(minZ, null);

            if (b.IsEmpty())
                dynBlockFactory.RemoveBlock(b);
        }
    }
}

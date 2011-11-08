using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for placing/removing straight walls in dynamic blocks and making sure the adjacent blocks are modified correctly.
    /// </summary>
    public class StraightWallPlacer
    {
        private readonly DynamicBlockFactory dynBlockFactory;
        private readonly DynamicBlockResolver resolver;

        public StraightWallPlacer(DynamicBlockFactory dynBlockFactory, DynamicBlockResolver resolver)
        {
            this.dynBlockFactory = dynBlockFactory;
            this.resolver = resolver;
        }

        public void PlaceStraightWall(Point3 pos, WallType type)
        {
            var b = dynBlockFactory.GetBlockAtPosition(pos);
            if (b == null)
                b = dynBlockFactory.CreateNewDynamicBlock(pos);

            if (b.hasSkewWalls)
                return;
            b.hasStraightWalls = true;

            b.SetPillar(type.PillarUnit);

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

        public void RemoveStraightWall(Point3 pos)
        {

            var b = dynBlockFactory.GetBlockAtPosition(pos);
            if (b == null)
                return;

            b.SetStraight(DynamicBlockDirection.X, 0, null);
            b.SetStraight(DynamicBlockDirection.X, 1, null);
            b.SetStraight(DynamicBlockDirection.MinX, 0, null);
            b.SetStraight(DynamicBlockDirection.MinX, 1, null);
            b.SetStraight(DynamicBlockDirection.Z, 0, null);
            b.SetStraight(DynamicBlockDirection.Z, 1, null);
            b.SetStraight(DynamicBlockDirection.MinZ, 0, null);
            b.SetStraight(DynamicBlockDirection.MinZ, 1, null);
            b.SetPillar(null);

            b.hasStraightWalls = false;

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

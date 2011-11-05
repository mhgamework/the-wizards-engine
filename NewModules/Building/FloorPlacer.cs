using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for adding floors to the DynamicBlock at given position. It is not possible to add floors to a block which has walls.
    /// </summary>
    public class FloorPlacer
    {
        private readonly DynamicBlockFactory dynBlockFactory;
        private readonly DynamicBlockResolver resolver;

        public FloorPlacer(DynamicBlockFactory dynBlockFactory, DynamicBlockResolver resolver)
        {
            this.dynBlockFactory = dynBlockFactory;
            this.resolver = resolver;
        }

        public void PlaceFloor(Point3 pos, FloorType type)
        {
            var b = dynBlockFactory.GetBlockAtPosition(pos);
            if (b == null)
                b = dynBlockFactory.CreateNewDynamicBlock(pos);

            if (b.HasWalls())
                return;

            b.SetFloor(DynamicBlockDirection.MinXMinZ, 0, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.MinXMinZ, 1, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.XMinZ, 0, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.XMinZ, 1, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.MinXZ, 0, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.MinXZ, 1, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.XZ, 0, type.DefaultUnit);
            b.SetFloor(DynamicBlockDirection.XZ, 1, type.DefaultUnit);

            b.hasFullFloor = true;

            var x = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitZ));
            var xMinZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX - Vector3.UnitZ));
            var xz = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX + Vector3.UnitZ));
            var minXMinZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX - Vector3.UnitZ));
            var minXZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX + Vector3.UnitZ));

            if (x != null)
                resolver.ResolveFloors(x, type);
            if (minX != null)
                resolver.ResolveFloors(minX, type);
            if (z != null)
                resolver.ResolveFloors(z, type);
            if (minZ != null)
                resolver.ResolveFloors(minZ, type);
            if (xMinZ != null)
                resolver.ResolveFloors(xMinZ, type);
            if (minXZ != null)
                resolver.ResolveFloors(minXZ, type);
            if (xz != null)
                resolver.ResolveFloors(xz, type);
            if (minXMinZ != null)
                resolver.ResolveFloors(minXMinZ, type);
        }

        public void RemoveFloor(Point3 pos)
        {
            var b = dynBlockFactory.GetBlockAtPosition(pos);
            if (b == null)
                b = dynBlockFactory.CreateNewDynamicBlock(pos);

            if (b.HasWalls())
                return;

            b.SetFloor(DynamicBlockDirection.MinXMinZ, 0, null);
            b.SetFloor(DynamicBlockDirection.MinXMinZ, 1, null);
            b.SetFloor(DynamicBlockDirection.XMinZ, 0, null);
            b.SetFloor(DynamicBlockDirection.XMinZ, 1, null);
            b.SetFloor(DynamicBlockDirection.MinXZ, 0, null);
            b.SetFloor(DynamicBlockDirection.MinXZ, 1, null);
            b.SetFloor(DynamicBlockDirection.XZ, 0, null);
            b.SetFloor(DynamicBlockDirection.XZ, 1, null);

            b.hasFullFloor = false;

            var x = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitZ));
            var xMinZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX - Vector3.UnitZ));
            var xz = dynBlockFactory.GetBlockAtPosition(new Point3(pos + Vector3.UnitX + Vector3.UnitZ));
            var minXMinZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX - Vector3.UnitZ));
            var minXZ = dynBlockFactory.GetBlockAtPosition(new Point3(pos - Vector3.UnitX + Vector3.UnitZ));

            if (x != null)
                resolver.ResolveFloors(x, null);
            if (minX != null)
                resolver.ResolveFloors(minX, null);
            if (z != null)
                resolver.ResolveFloors(z, null);
            if (minZ != null)
                resolver.ResolveFloors(minZ, null);
            if (xMinZ != null)
                resolver.ResolveFloors(xMinZ, null);
            if (minXZ != null)
                resolver.ResolveFloors(minXZ, null);
            if (xz != null)
                resolver.ResolveFloors(xz, null);
            if (minXMinZ != null)
                resolver.ResolveFloors(minXMinZ, null);
        }
    }
}

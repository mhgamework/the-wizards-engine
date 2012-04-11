using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for checking if the slots of a given DynamicBlock are correctly filled in accordance to the adjacent blocks.
    /// </summary>
    public class DynamicBlockResolver
    {
        private readonly DynamicBlockFactory dynBlockFactory;

        public DynamicBlockResolver(DynamicBlockFactory dynBlockFactory)
        {
            this.dynBlockFactory = dynBlockFactory;
        }

        public void ResolveWalls(DynamicBlock b, WallType type)
        {
            if (b == null)
                return;
            if (b.hasStraightWalls)
                resolveStraightWalls(b, type);
            if (b.hasSkewWalls)
                resolveSkewWalls(b, type);
        }

        private void resolveStraightWalls(DynamicBlock b, WallType type)
        {
            if (!b.hasStraightWalls)
                return;

            if(b.getPillarSlot().BuildUnit == null)
                b.SetPillar(type.PillarUnit);

            var x = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitZ));

            if (x != null && x.HasWalls())
            {
                if (b.GetStraightSlot(DynamicBlockDirection.X, 0).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.X, 0, type.StraightUnit);
                if (b.GetStraightSlot(DynamicBlockDirection.X, 1).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.X, 1, type.StraightUnit);
            }
            else
            {
                b.SetStraight(DynamicBlockDirection.X, 0, null);
                b.SetStraight(DynamicBlockDirection.X, 1, null);
            }
            if (minX != null && minX.HasWalls())
            {
                if (b.GetStraightSlot(DynamicBlockDirection.MinX, 0).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.MinX, 0, type.StraightUnit);
                if (b.GetStraightSlot(DynamicBlockDirection.MinX, 1).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.MinX, 1, type.StraightUnit);
            }
            else
            {
                b.SetStraight(DynamicBlockDirection.MinX, 0, null);
                b.SetStraight(DynamicBlockDirection.MinX, 1, null);
            }
            if (z != null && z.HasWalls())
            {
                if (b.GetStraightSlot(DynamicBlockDirection.Z, 0).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.Z, 0, type.StraightUnit);
                if (b.GetStraightSlot(DynamicBlockDirection.Z, 1).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.Z, 1, type.StraightUnit);
            }
            else
            {
                b.SetStraight(DynamicBlockDirection.Z, 0, null);
                b.SetStraight(DynamicBlockDirection.Z, 1, null);
            }
            if (minZ != null && minZ.HasWalls())
            {
                if (b.GetStraightSlot(DynamicBlockDirection.MinZ, 0).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.MinZ, 0, type.StraightUnit);
                if (b.GetStraightSlot(DynamicBlockDirection.MinZ, 1).BuildUnit == null)
                    b.SetStraight(DynamicBlockDirection.MinZ, 1, type.StraightUnit);
            }
            else
            {
                b.SetStraight(DynamicBlockDirection.MinZ, 0, null);
                b.SetStraight(DynamicBlockDirection.MinZ, 1, null);
            }
        }

        private void resolveSkewWalls(DynamicBlock b, WallType type)
        {
            var x = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitZ));

            if (x != null && minZ != null && x.HasWalls() && minZ.HasWalls())
            {
                if (b.GetSkewSlot(DynamicBlockDirection.XMinZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XMinZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.XMinZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XMinZ, 1, type.SkewUnit);
            }
            else
            {
                b.SetSkew(DynamicBlockDirection.XMinZ, 0, null);
                b.SetSkew(DynamicBlockDirection.XMinZ, 1, null);
            }
            if (minZ != null && minX != null && minZ.HasWalls() && minX.HasWalls())
            {
                if (b.GetSkewSlot(DynamicBlockDirection.MinXMinZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXMinZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.MinXMinZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXMinZ, 1, type.SkewUnit);
            }
            else
            {
                b.SetSkew(DynamicBlockDirection.MinXMinZ, 0, null);
                b.SetSkew(DynamicBlockDirection.MinXMinZ, 1, null);
            }
            if (minX != null && z != null && minX.HasWalls() && z.HasWalls())
            {
                if (b.GetSkewSlot(DynamicBlockDirection.MinXZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.MinXZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXZ, 1, type.SkewUnit);
            }
            else
            {
                b.SetSkew(DynamicBlockDirection.MinXZ, 0, null);
                b.SetSkew(DynamicBlockDirection.MinXZ, 1, null);
            }
            if (z != null && x != null && z.HasWalls() && x.HasWalls())
            {
                if (b.GetSkewSlot(DynamicBlockDirection.XZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.XZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XZ, 1, type.SkewUnit);
            }
            else
            {
                b.SetSkew(DynamicBlockDirection.XZ, 0, null);
                b.SetSkew(DynamicBlockDirection.XZ, 1, null);
            }
            if ((x != null && minX != null && x.HasWalls() && minX.HasWalls()) || (z != null && minZ != null && z.HasWalls() && minZ.HasWalls()))
            {
                if (b.GetSkewSlot(DynamicBlockDirection.XMinZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XMinZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.XMinZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XMinZ, 1, type.SkewUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.MinXMinZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXMinZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.MinXMinZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXMinZ, 1, type.SkewUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.MinXZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.MinXZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.MinXZ, 1, type.SkewUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.XZ, 0).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XZ, 0, type.SkewUnit);
                if (b.GetSkewSlot(DynamicBlockDirection.XZ, 1).BuildUnit == null)
                    b.SetSkew(DynamicBlockDirection.XZ, 1, type.SkewUnit);
            }
        }

        //PROBABLY NOT USED ANYMORE
        public void ResolveFloors(DynamicBlock b, FloorType type)
        {
            var x = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitX));
            var minX = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitX));
            var z = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitZ));
            var minZ = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitZ));
            var xMinZ = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitX - Vector3.UnitZ));
            var xz = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position + Vector3.UnitX + Vector3.UnitZ));
            var minXMinZ = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitX - Vector3.UnitZ));
            var minXZ = dynBlockFactory.GetBlockAtPosition(new Point3(b.Position - Vector3.UnitX + Vector3.UnitZ));



            if ((x != null && x.hasFullFloor) || (xMinZ != null && xMinZ.hasFullFloor) || (minZ != null && minZ.hasFullFloor))
            {
                if (b.GetFloorSlot(DynamicBlockDirection.XMinZ, 1).BuildUnit == null)
                    b.SetFloor(DynamicBlockDirection.XMinZ, 1, type.DefaultUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.XMinZ, 0).BuildUnit == null)
                {
                    if (b.GetFloorSlot(DynamicBlockDirection.XMinZ, 0).BuildUnit == null)
                        b.SetFloor(DynamicBlockDirection.XMinZ, 0, type.DefaultUnit);
                }
            }
            else
            {
                b.SetFloor(DynamicBlockDirection.XMinZ, 0, null);
                b.SetFloor(DynamicBlockDirection.XMinZ, 1, null);
            }
            if ((minZ != null && minZ.hasFullFloor) || (minXMinZ != null && minXMinZ.hasFullFloor) || (minX != null && minX.hasFullFloor))
            {
                if (b.GetFloorSlot(DynamicBlockDirection.MinXMinZ, 1).BuildUnit == null)
                    b.SetFloor(DynamicBlockDirection.MinXMinZ, 1, type.DefaultUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.MinXMinZ, 0).BuildUnit == null)
                {
                    if (b.GetFloorSlot(DynamicBlockDirection.MinXMinZ, 0).BuildUnit == null)
                        b.SetFloor(DynamicBlockDirection.MinXMinZ, 0, type.DefaultUnit);
                }
            }
            else
            {
                b.SetFloor(DynamicBlockDirection.MinXMinZ, 0, null);
                b.SetFloor(DynamicBlockDirection.MinXMinZ, 1, null);
            }
            if ((minX != null && minX.hasFullFloor) || (minXZ != null && minXZ.hasFullFloor) || (z != null && z.hasFullFloor))
            {
                if (b.GetFloorSlot(DynamicBlockDirection.MinXZ, 1).BuildUnit == null)
                    b.SetFloor(DynamicBlockDirection.MinXZ, 1, type.DefaultUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.MinXZ, 0).BuildUnit == null)
                {
                    if (b.GetFloorSlot(DynamicBlockDirection.MinXZ, 0).BuildUnit == null)
                        b.SetFloor(DynamicBlockDirection.MinXZ, 0, type.DefaultUnit);
                }
            }
            else
            {
                b.SetFloor(DynamicBlockDirection.MinXZ, 0, null);
                b.SetFloor(DynamicBlockDirection.MinXZ, 1, null);
            }
            if ((z != null && z.hasFullFloor) || (xz != null && xz.hasFullFloor) || (x != null && x.hasFullFloor))
            {
                if (b.GetFloorSlot(DynamicBlockDirection.XZ, 1).BuildUnit == null)
                    b.SetFloor(DynamicBlockDirection.XZ, 1, type.DefaultUnit);

                if (b.GetSkewSlot(DynamicBlockDirection.XZ, 0).BuildUnit == null)
                {
                    if (b.GetFloorSlot(DynamicBlockDirection.XZ, 0).BuildUnit == null)
                        b.SetFloor(DynamicBlockDirection.XZ, 0, type.DefaultUnit);
                }
            }
            else
            {
                b.SetFloor(DynamicBlockDirection.XZ, 0, null);
                b.SetFloor(DynamicBlockDirection.XZ, 1, null);
            }

            if (b.GetFloorSlot(DynamicBlockDirection.MinXMinZ, 0).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.MinXMinZ, 1).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.MinXZ, 0).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.MinXZ, 1).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.XMinZ, 0).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.XMinZ, 1).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.XZ, 0).BuildUnit != null
                                    && b.GetFloorSlot(DynamicBlockDirection.XZ, 1).BuildUnit != null)
            {
                b.hasFullFloor = true;
            }
            else
                b.hasFullFloor = false;
        }
    }
}

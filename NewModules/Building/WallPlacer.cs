using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for placing/removing walls in dynamic blocks.
    /// </summary>
    public class WallPlacer
    {
        private readonly DynamicBlockFactory dynBlockFactory;
        private readonly DynamicBlockResolver resolver;

        public WallPlacer(DynamicBlockFactory dynBlockFactory, DynamicBlockResolver resolver)
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

        public void RotateSkewWall(Point3 pos)
        {
            var b = dynBlockFactory.GetBlockAtPosition(pos);

            if (b == null || b.hasStraightWalls)
                return;

            var MinXMinZ = b.GetSkewSlot(DynamicBlockDirection.MinXMinZ, 0).BuildUnit;
            var MinXZ = b.GetSkewSlot(DynamicBlockDirection.MinXZ, 0).BuildUnit;
            var XMinZ = b.GetSkewSlot(DynamicBlockDirection.XMinZ, 0).BuildUnit;
            var XZ = b.GetSkewSlot(DynamicBlockDirection.XZ, 0).BuildUnit;

            b.SetSkew(DynamicBlockDirection.XZ, 0, XMinZ);
            b.SetSkew(DynamicBlockDirection.XZ, 1, XMinZ);
            b.SetSkew(DynamicBlockDirection.MinXZ, 0, XZ);
            b.SetSkew(DynamicBlockDirection.MinXZ, 1, XZ);
            b.SetSkew(DynamicBlockDirection.MinXMinZ, 0, MinXZ);
            b.SetSkew(DynamicBlockDirection.MinXMinZ, 1, MinXZ);
            b.SetSkew(DynamicBlockDirection.XMinZ, 0, MinXMinZ);
            b.SetSkew(DynamicBlockDirection.XMinZ, 1, MinXMinZ);
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

        public void PlaceStraightWallsPointToPoint(Vector3 start, Vector3 end, WallType type)
        {
            var offset = end - start;
            var startBlock =
                dynBlockFactory.GetBlockAtPosition(
                    dynBlockFactory.CalculateBlockPos(start + Vector3.Normalize(offset) * 0.1f));
            if (startBlock == null)
                startBlock = dynBlockFactory.CreateNewDynamicBlock(dynBlockFactory.CalculateBlockPos(start + Vector3.Normalize(offset) * 0.1f));
            var endBlock =
                dynBlockFactory.GetBlockAtPosition(
                    dynBlockFactory.CalculateBlockPos(end - Vector3.Normalize(offset) * 0.1f));
            if (endBlock == null)
                endBlock = dynBlockFactory.CreateNewDynamicBlock(dynBlockFactory.CalculateBlockPos(end - Vector3.Normalize(offset) * 0.1f));

            startBlock.hasStraightWalls = true;
            endBlock.hasStraightWalls = true;

            List<DynamicBlock> blockList = getBlockListPointToPoint(startBlock, endBlock);

            for (int i = 0; i < blockList.Count; i++)
            {
                blockList[i].hasStraightWalls = true;
            }

            for (int i = 0; i < blockList.Count; i++)
            {
                resolver.ResolveWalls(blockList[i], type);
            }

            startBlock.SetStraight(DynamicBlock.GetDirectionFromVector(start - startBlock.Position), 0, type.StraightUnit);
            startBlock.SetStraight(DynamicBlock.GetDirectionFromVector(start - startBlock.Position), 1, type.StraightUnit);
            resolver.ResolveWalls(
                dynBlockFactory.GetBlockAtPosition(
                    dynBlockFactory.CalculateBlockPos(start - Vector3.Normalize(offset) * 0.1f)), type);
            endBlock.SetStraight(DynamicBlock.GetDirectionFromVector(end - endBlock.Position), 0, type.StraightUnit);
            endBlock.SetStraight(DynamicBlock.GetDirectionFromVector(end - endBlock.Position), 1, type.StraightUnit);
            resolver.ResolveWalls(
                dynBlockFactory.GetBlockAtPosition(
                    dynBlockFactory.CalculateBlockPos(end + Vector3.Normalize(offset) * 0.1f)), type);
        }

        public void PlaceSkewWallsPointToPoint(Vector3 start, Vector3 end, WallType type)
        {
            if (start.X == end.X && start.Z == end.Z) return;

            var offset = end - start;
            var startBlock =
                dynBlockFactory.GetBlockAtPosition(
                    dynBlockFactory.CalculateBlockPos(start + Vector3.Normalize(offset)*0.1f));
            if (startBlock == null)
                startBlock =
                    dynBlockFactory.CreateNewDynamicBlock(
                        dynBlockFactory.CalculateBlockPos(start + Vector3.Normalize(offset)*0.1f));
            var endBlock =
                dynBlockFactory.GetBlockAtPosition(
                    dynBlockFactory.CalculateBlockPos(end - Vector3.Normalize(offset)*0.1f));
            if (endBlock == null)
                endBlock =
                    dynBlockFactory.CreateNewDynamicBlock(
                        dynBlockFactory.CalculateBlockPos(end - Vector3.Normalize(offset)*0.1f));

            startBlock.hasSkewWalls = true;
            endBlock.hasSkewWalls = true;
            
            var startDir = DynamicBlock.GetDirectionFromVector(start - startBlock.Position);

            if (startDir == DynamicBlockDirection.X)
            {
                if (Vector3.Distance(end, startBlock.Position + new Vector3(0, 0, 0.5f)) <
                    Vector3.Distance(end, (startBlock.Position + new Vector3(0, 0, -0.5f))))
                {
                    startBlock.SetSkew(DynamicBlockDirection.XZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.XZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(0, 0, 0.5f), end, type);
                }
                else
                {
                    startBlock.SetSkew(DynamicBlockDirection.XMinZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.XMinZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(0, 0, -0.5f), end, type);
                }
            }
            else if (startDir == DynamicBlockDirection.MinX)
            {
                if (Vector3.Distance(end, startBlock.Position + new Vector3(0, 0, 0.5f)) <
                    Vector3.Distance(end, (startBlock.Position + new Vector3(0, 0, -0.5f))))
                {
                    startBlock.SetSkew(DynamicBlockDirection.MinXZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.MinXZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(0, 0, 0.5f), end, type);
                }
                else
                {
                    startBlock.SetSkew(DynamicBlockDirection.MinXMinZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.MinXMinZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(0, 0, -0.5f), end, type);
                }
            }
            else if (startDir == DynamicBlockDirection.Z)
            {
                if (Vector3.Distance(end, startBlock.Position + new Vector3(0.5f, 0, 0)) <
                    Vector3.Distance(end, (startBlock.Position + new Vector3(-0.5f, 0, 0))))
                {
                    startBlock.SetSkew(DynamicBlockDirection.XZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.XZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(0.5f, 0, 0), end, type);
                }
                else
                {
                    startBlock.SetSkew(DynamicBlockDirection.MinXZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.MinXZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(-0.5f, 0, 0), end, type);
                }
            }
            else if (startDir == DynamicBlockDirection.MinZ)
            {
                if (Vector3.Distance(end, startBlock.Position + new Vector3(0.5f, 0, 0)) <
                    Vector3.Distance(end, (startBlock.Position + new Vector3(-0.5f, 0, 0))))
                {
                    startBlock.SetSkew(DynamicBlockDirection.XMinZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.XMinZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(0.5f, 0, 0), end, type);
                }
                else
                {
                    startBlock.SetSkew(DynamicBlockDirection.MinXMinZ, 0, type.SkewUnit);
                    startBlock.SetSkew(DynamicBlockDirection.MinXMinZ, 1, type.SkewUnit);
                    PlaceSkewWallsPointToPoint(startBlock.Position + new Vector3(-0.5f, 0, 0), end, type);
                }
            }

        }


        private List<DynamicBlock> getBlockListPointToPoint(DynamicBlock startBlock, DynamicBlock endBlock)
        {
            List<DynamicBlock> blockList = new List<DynamicBlock>();
            var endPos = endBlock.Position;
            var cPos = startBlock.Position;
            DynamicBlock cBlock;
            while (cPos != endBlock.Position)
            {
                if (cPos.X < endPos.X && cPos.Z < endPos.Z)
                {
                    cPos.X++;
                    cBlock = dynBlockFactory.GetBlockAtPosition(cPos);
                    if (cBlock == null)
                        cBlock = dynBlockFactory.CreateNewDynamicBlock(cPos);
                    blockList.Add(cBlock);

                    cPos.Z++;
                }
                else if (cPos.X < endPos.X)
                    cPos.X++;
                else if (cPos.Z < endPos.Z)
                    cPos.Z++;

                if (cPos.X > endPos.X && cPos.Z > endPos.Z)
                {
                    cPos.X--;
                    cBlock = dynBlockFactory.GetBlockAtPosition(cPos);
                    if (cBlock == null)
                        cBlock = dynBlockFactory.CreateNewDynamicBlock(cPos);
                    blockList.Add(cBlock);

                    cPos.Z--;
                }
                else if (cPos.X > endPos.X)
                    cPos.X--;
                else if (cPos.Z > endPos.Z)
                    cPos.Z--;


                cBlock = dynBlockFactory.GetBlockAtPosition(cPos);
                if (cBlock == null)
                    cBlock = dynBlockFactory.CreateNewDynamicBlock(cPos);
                blockList.Add(cBlock);
            }

            blockList.Add(startBlock);
            blockList.Add(endBlock);

            return blockList;
        }
       

       
    }
}

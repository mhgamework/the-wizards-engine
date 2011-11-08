using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for containing all Slots. It has a world-position. Origin in the middle of the block.
    /// INVAR: it shouldn't be allowed to have both straight and skew walls.
    /// </summary>
    public class DynamicBlock
    {
        public Point3 Position;
        private readonly DeferredRenderer renderer;

        public bool hasStraightWalls;
        public bool hasSkewWalls;
        public bool hasFullFloor;

        private Dictionary<DynamicBlockDirection, BuildSlot> straightSlotsLow = new Dictionary<DynamicBlockDirection, BuildSlot>();
        private Dictionary<DynamicBlockDirection, BuildSlot> straightSlotsHigh = new Dictionary<DynamicBlockDirection, BuildSlot>();
        private Dictionary<DynamicBlockDirection, BuildSlot> floorSlotsClose = new Dictionary<DynamicBlockDirection, BuildSlot>();
        private Dictionary<DynamicBlockDirection, BuildSlot> floorSlotsFar = new Dictionary<DynamicBlockDirection, BuildSlot>();
        private Dictionary<DynamicBlockDirection, BuildSlot> skewSlotsLow = new Dictionary<DynamicBlockDirection, BuildSlot>();
        private Dictionary<DynamicBlockDirection, BuildSlot> skewSlotsHigh = new Dictionary<DynamicBlockDirection, BuildSlot>();
        private BuildSlot pillarh;
        private BuildSlot pillar;

        public DynamicBlock(Point3 pos, DeferredRenderer renderer)
        {
            Position = pos;
            this.renderer = renderer;
            initializeSlots();
        }

        public void SetStraight(DynamicBlockDirection dir, int highLow, BuildUnit buildUnit)
        {
            GetStraightSlot(dir, highLow).SetBuildUnit(buildUnit);
        }

        public void SetPillar(BuildUnit buildUnit)
        {
            pillar.SetBuildUnit(buildUnit);
            pillarh.SetBuildUnit(buildUnit);
        }

        public void SetFloor(DynamicBlockDirection dir, int closeFar, BuildUnit buildUnit)
        {
            GetFloorSlot(dir, closeFar).SetBuildUnit(buildUnit);
        }

        public void SetSkew(DynamicBlockDirection dir, int highLow, BuildUnit buildUnit)
        {
            GetSkewSlot(dir, highLow).SetBuildUnit(buildUnit);
        }

        /// <summary>
        /// returns the slot located in given direction and height. height 0 = low, 1 = high
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="highLow"></param>
        public BuildSlot GetStraightSlot(DynamicBlockDirection dir, int highLow)
        {
            if (dir == DynamicBlockDirection.XZ || dir == DynamicBlockDirection.MinXZ || dir == DynamicBlockDirection.XMinZ || dir == DynamicBlockDirection.MinXMinZ)
                throw new Exception("Invalid direction given");

            BuildSlot ret;
            if (highLow == 0)
                straightSlotsLow.TryGetValue(dir, out ret);
            else
                straightSlotsHigh.TryGetValue(dir, out ret);

            return ret;
        }

        /// <summary>
        /// returns the floor-slot located in given direction and closeness. height 0 = close, 1 = far
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="closeFar"></param>
        /// <returns></returns>
        public BuildSlot GetFloorSlot(DynamicBlockDirection dir, int closeFar)
        {
            if (dir == DynamicBlockDirection.X || dir == DynamicBlockDirection.MinX || dir == DynamicBlockDirection.Z || dir == DynamicBlockDirection.MinZ)
                throw new Exception("Invalid direction given");

            BuildSlot ret;
            if (closeFar == 0)
                floorSlotsClose.TryGetValue(dir, out ret);
            else
                floorSlotsFar.TryGetValue(dir, out ret);

            return ret;
        }

        /// <summary>
        /// returns the slot located in given direction and height. height 0 = low, 1 = high
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="highLow"></param>
        /// <returns></returns>
        public BuildSlot GetSkewSlot(DynamicBlockDirection dir, int highLow)
        {
            if (dir == DynamicBlockDirection.X || dir == DynamicBlockDirection.MinX || dir == DynamicBlockDirection.Z || dir == DynamicBlockDirection.MinZ)
                throw new Exception("Invalid direction given");

            BuildSlot ret;
            if (highLow == 0)
                skewSlotsLow.TryGetValue(dir, out ret);
            else
                skewSlotsHigh.TryGetValue(dir, out ret);

            return ret;
        }

        /// <summary>
        /// Default position = 000
        /// Default rotation is direction of x-axis
        /// </summary>
        private void initializeSlots()
        {
            var straightX = new BuildSlot(this, new Vector3(0.25f, -0.25f, 0), 0, renderer);
            var straightXh = new BuildSlot(this, new Vector3(0.25f, 0.25f, 0), 0, renderer);
            var straightMinX = new BuildSlot(this, new Vector3(-0.25f, -0.25f, 0), (float)Math.PI, renderer);
            var straightMinXh = new BuildSlot(this, new Vector3(-0.25f, 0.25f, 0), (float)Math.PI, renderer);
            var straightZ = new BuildSlot(this, new Vector3(0, -0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var straightZh = new BuildSlot(this, new Vector3(0, 0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var straightMinZ = new BuildSlot(this, new Vector3(0, -0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);
            var straightMinZh = new BuildSlot(this, new Vector3(0, 0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);

            straightSlotsLow.Add(DynamicBlockDirection.X, straightX);
            straightSlotsLow.Add(DynamicBlockDirection.MinX, straightMinX);
            straightSlotsLow.Add(DynamicBlockDirection.Z, straightZ);
            straightSlotsLow.Add(DynamicBlockDirection.MinZ, straightMinZ);

            straightSlotsHigh.Add(DynamicBlockDirection.X, straightXh);
            straightSlotsHigh.Add(DynamicBlockDirection.MinX, straightMinXh);
            straightSlotsHigh.Add(DynamicBlockDirection.Z, straightZh);
            straightSlotsHigh.Add(DynamicBlockDirection.MinZ, straightMinZh);

            pillar = new BuildSlot(this, new Vector3(0, -0.25f, 0), 0, renderer);
            pillarh = new BuildSlot(this, new Vector3(0, 0.25f, 0), 0, renderer);

            var floorXMinZC = new BuildSlot(this, new Vector3(0.25f, -0.5f, -0.25f), 0, renderer);
            var floorXMinZF = new BuildSlot(this, new Vector3(0.25f, -0.5f, -0.25f), (float)Math.PI, renderer);
            var floorMinXZC = new BuildSlot(this, new Vector3(-0.25f, -0.5f, 0.25f), (float)Math.PI, renderer);
            var floorMinXZF = new BuildSlot(this, new Vector3(-0.25f, -0.5f, 0.25f), 0, renderer);
            var floorXZC = new BuildSlot(this, new Vector3(0.25f, -0.5f, 0.25f), (float)Math.PI, new Vector3(-1, 1, 1), renderer);
            var floorXZF = new BuildSlot(this, new Vector3(0.25f, -0.5f, 0.25f), 0, new Vector3(-1, 1, 1), renderer);
            var floorMinXMinZC = new BuildSlot(this, new Vector3(-0.25f, -0.5f, -0.25f), (float)Math.PI, new Vector3(1, 1, -1), renderer);
            var floorMinXMinZF = new BuildSlot(this, new Vector3(-0.25f, -0.5f, -0.25f), 0, new Vector3(1, 1, -1), renderer);

            floorSlotsClose.Add(DynamicBlockDirection.XMinZ, floorXMinZC);
            floorSlotsClose.Add(DynamicBlockDirection.XZ, floorXZC);
            floorSlotsClose.Add(DynamicBlockDirection.MinXZ, floorMinXZC);
            floorSlotsClose.Add(DynamicBlockDirection.MinXMinZ, floorMinXMinZC);

            floorSlotsFar.Add(DynamicBlockDirection.XMinZ, floorXMinZF);
            floorSlotsFar.Add(DynamicBlockDirection.XZ, floorXZF);
            floorSlotsFar.Add(DynamicBlockDirection.MinXZ, floorMinXZF);
            floorSlotsFar.Add(DynamicBlockDirection.MinXMinZ, floorMinXMinZF);

            var skewXMinZ = new BuildSlot(this, new Vector3(0.25f, -0.25f, -0.25f), 0, renderer);
            var skewXMinZh = new BuildSlot(this, new Vector3(0.25f, 0.25f, -0.25f), 0, renderer);
            var skewMinXZ = new BuildSlot(this, new Vector3(-0.25f, -0.25f, 0.25f), (float)Math.PI, renderer);
            var skewMinXZh = new BuildSlot(this, new Vector3(-0.25f, 0.25f, 0.25f), (float)Math.PI, renderer);
            var skewXZ = new BuildSlot(this, new Vector3(0.25f, -0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var skewXZh = new BuildSlot(this, new Vector3(0.25f, 0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var skewMinXMinZ = new BuildSlot(this, new Vector3(-0.25f, -0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);
            var skewMinXMinZh = new BuildSlot(this, new Vector3(-0.25f, 0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);

            skewSlotsLow.Add(DynamicBlockDirection.XMinZ, skewXMinZ);
            skewSlotsLow.Add(DynamicBlockDirection.MinXZ, skewMinXZ);
            skewSlotsLow.Add(DynamicBlockDirection.XZ, skewXZ);
            skewSlotsLow.Add(DynamicBlockDirection.MinXMinZ, skewMinXMinZ);

            skewSlotsHigh.Add(DynamicBlockDirection.XMinZ, skewXMinZh);
            skewSlotsHigh.Add(DynamicBlockDirection.MinXZ, skewMinXZh);
            skewSlotsHigh.Add(DynamicBlockDirection.XZ, skewXZh);
            skewSlotsHigh.Add(DynamicBlockDirection.MinXMinZ, skewMinXMinZh);
        }

        internal bool HasUnitOfType(string p)
        {
            var it01 = straightSlotsLow.Values.GetEnumerator();
            while (it01.Current != straightSlotsLow.Values.Last())
            {
                it01.MoveNext();
                if (it01.Current.BuildUnit != null && it01.Current.BuildUnit.buildUnitType == p)
                    return true;
            }

            var it02 = straightSlotsHigh.Values.GetEnumerator();
            while (it02.Current != straightSlotsHigh.Values.Last())
            {
                it02.MoveNext();
                if (it02.Current.BuildUnit != null && it02.Current.BuildUnit.buildUnitType == p)
                    return true;
            }

            var it03 = floorSlotsClose.Values.GetEnumerator();
            while (it03.Current != floorSlotsClose.Values.Last())
            {
                it03.MoveNext();
                if (it03.Current.BuildUnit != null && it03.Current.BuildUnit.buildUnitType == p)
                    return true;
            }

            var it04 = floorSlotsFar.Values.GetEnumerator();
            while (it04.Current != floorSlotsFar.Values.Last())
            {
                it04.MoveNext();
                if (it04.Current.BuildUnit != null && it04.Current.BuildUnit.buildUnitType == p)
                    return true;
            }

            var it05 = skewSlotsLow.Values.GetEnumerator();
            while (it05.Current != skewSlotsLow.Values.Last())
            {
                it05.MoveNext();
                if (it05.Current.BuildUnit != null && it05.Current.BuildUnit.buildUnitType == p)
                    return true;
            }

            var it06 = skewSlotsHigh.Values.GetEnumerator();
            while (it06.Current != skewSlotsHigh.Values.Last())
            {
                it06.MoveNext();
                if (it06.Current.BuildUnit != null && it06.Current.BuildUnit.buildUnitType == p)
                    return true;
            }

            if (pillar.BuildUnit != null && pillar.BuildUnit.buildUnitType == p)
                return true;

            if (pillarh.BuildUnit != null && pillarh.BuildUnit.buildUnitType == p)
                return true;

            return false;
        }

        public bool HasWalls()
        {
            return hasSkewWalls || hasStraightWalls;
        }

        internal bool IsEmpty()
        {
            var it01 = straightSlotsLow.Values.GetEnumerator();
            while (it01.Current != straightSlotsLow.Values.Last())
            {
                it01.MoveNext();
                if (it01.Current.BuildUnit != null)
                    return false;
            }

            var it02 = straightSlotsHigh.Values.GetEnumerator();
            while (it02.Current != straightSlotsHigh.Values.Last())
            {
                it02.MoveNext();
                if (it02.Current.BuildUnit != null)
                    return false;
            }

            var it03 = floorSlotsClose.Values.GetEnumerator();
            while (it03.Current != floorSlotsClose.Values.Last())
            {
                it03.MoveNext();
                if (it03.Current.BuildUnit != null)
                    return false;
            }

            var it04 = floorSlotsFar.Values.GetEnumerator();
            while (it04.Current != floorSlotsFar.Values.Last())
            {
                it04.MoveNext();
                if (it04.Current.BuildUnit != null)
                    return false;
            }

            var it05 = skewSlotsLow.Values.GetEnumerator();
            while (it05.Current != skewSlotsLow.Values.Last())
            {
                it05.MoveNext();
                if (it05.Current.BuildUnit != null)
                    return false;
            }

            var it06 = skewSlotsHigh.Values.GetEnumerator();
            while (it06.Current != skewSlotsHigh.Values.Last())
            {
                it06.MoveNext();
                if (it06.Current.BuildUnit != null)
                    return false;
            }

            if (pillar.BuildUnit != null)
                return false;

            if (pillarh.BuildUnit != null)
                return false;

            return true;
        }
    }
}

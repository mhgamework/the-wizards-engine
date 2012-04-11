using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
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

        private ModelContainer.ModelContainer container;

        public DynamicBlock(Point3 pos, DeferredRenderer renderer, ModelContainer.ModelContainer container)
        {
            Position = pos;
            this.renderer = renderer;
            this.container = container;
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

        public BuildSlot getPillarSlot()
        {
            return pillar;
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
            var straightX = createBuildSlot(this, new Vector3(0.25f, -0.25f, 0), 0, renderer);
            var straightXh = createBuildSlot(this, new Vector3(0.25f, 0.25f, 0), 0, renderer);
            var straightMinX = createBuildSlot(this, new Vector3(-0.25f, -0.25f, 0), (float)Math.PI, renderer);
            var straightMinXh = createBuildSlot(this, new Vector3(-0.25f, 0.25f, 0), (float)Math.PI, renderer);
            var straightZ = createBuildSlot(this, new Vector3(0, -0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var straightZh = createBuildSlot(this, new Vector3(0, 0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var straightMinZ = createBuildSlot(this, new Vector3(0, -0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);
            var straightMinZh = createBuildSlot(this, new Vector3(0, 0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);

            straightSlotsLow.Add(DynamicBlockDirection.X, straightX);
            straightSlotsLow.Add(DynamicBlockDirection.MinX, straightMinX);
            straightSlotsLow.Add(DynamicBlockDirection.Z, straightZ);
            straightSlotsLow.Add(DynamicBlockDirection.MinZ, straightMinZ);

            straightSlotsHigh.Add(DynamicBlockDirection.X, straightXh);
            straightSlotsHigh.Add(DynamicBlockDirection.MinX, straightMinXh);
            straightSlotsHigh.Add(DynamicBlockDirection.Z, straightZh);
            straightSlotsHigh.Add(DynamicBlockDirection.MinZ, straightMinZh);

            pillar = createBuildSlot(this, new Vector3(0, -0.25f, 0), 0, renderer);
            pillarh = createBuildSlot(this, new Vector3(0, 0.25f, 0), 0, renderer);

            var floorXMinZC = createBuildSlot(this, new Vector3(0.25f, -0.5f, -0.25f), 0, renderer);
            var floorXMinZF = createBuildSlot(this, new Vector3(0.25f, -0.5f, -0.25f), (float)Math.PI, renderer);
            var floorMinXZC = createBuildSlot(this, new Vector3(-0.25f, -0.5f, 0.25f), (float)Math.PI, renderer);
            var floorMinXZF = createBuildSlot(this, new Vector3(-0.25f, -0.5f, 0.25f), 0, renderer);
            var floorXZC = createBuildSlot(this, new Vector3(0.25f, -0.5f, 0.25f), (float)Math.PI, new Vector3(-1, 1, 1), renderer);
            var floorXZF = createBuildSlot(this, new Vector3(0.25f, -0.5f, 0.25f), 0, new Vector3(-1, 1, 1), renderer);
            var floorMinXMinZC = createBuildSlot(this, new Vector3(-0.25f, -0.5f, -0.25f), (float)Math.PI, new Vector3(1, 1, -1), renderer);
            var floorMinXMinZF = createBuildSlot(this, new Vector3(-0.25f, -0.5f, -0.25f), 0, new Vector3(1, 1, -1), renderer);

            floorSlotsClose.Add(DynamicBlockDirection.XMinZ, floorXMinZC);
            floorSlotsClose.Add(DynamicBlockDirection.XZ, floorXZC);
            floorSlotsClose.Add(DynamicBlockDirection.MinXZ, floorMinXZC);
            floorSlotsClose.Add(DynamicBlockDirection.MinXMinZ, floorMinXMinZC);

            floorSlotsFar.Add(DynamicBlockDirection.XMinZ, floorXMinZF);
            floorSlotsFar.Add(DynamicBlockDirection.XZ, floorXZF);
            floorSlotsFar.Add(DynamicBlockDirection.MinXZ, floorMinXZF);
            floorSlotsFar.Add(DynamicBlockDirection.MinXMinZ, floorMinXMinZF);

            var skewXMinZ = createBuildSlot(this, new Vector3(0.25f, -0.25f, -0.25f), 0, renderer);
            var skewXMinZh = createBuildSlot(this, new Vector3(0.25f, 0.25f, -0.25f), 0, renderer);
            var skewMinXZ = createBuildSlot(this, new Vector3(-0.25f, -0.25f, 0.25f), (float)Math.PI, renderer);
            var skewMinXZh = createBuildSlot(this, new Vector3(-0.25f, 0.25f, 0.25f), (float)Math.PI, renderer);
            var skewXZ = createBuildSlot(this, new Vector3(0.25f, -0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var skewXZh = createBuildSlot(this, new Vector3(0.25f, 0.25f, 0.25f), (float)-Math.PI * 0.5f, renderer);
            var skewMinXMinZ = createBuildSlot(this, new Vector3(-0.25f, -0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);
            var skewMinXMinZh = createBuildSlot(this, new Vector3(-0.25f, 0.25f, -0.25f), (float)Math.PI * 0.5f, renderer);

            skewSlotsLow.Add(DynamicBlockDirection.XMinZ, skewXMinZ);
            skewSlotsLow.Add(DynamicBlockDirection.MinXZ, skewMinXZ);
            skewSlotsLow.Add(DynamicBlockDirection.XZ, skewXZ);
            skewSlotsLow.Add(DynamicBlockDirection.MinXMinZ, skewMinXMinZ);

            skewSlotsHigh.Add(DynamicBlockDirection.XMinZ, skewXMinZh);
            skewSlotsHigh.Add(DynamicBlockDirection.MinXZ, skewMinXZh);
            skewSlotsHigh.Add(DynamicBlockDirection.XZ, skewXZh);
            skewSlotsHigh.Add(DynamicBlockDirection.MinXMinZ, skewMinXMinZh);
        }

        public BuildSlot createBuildSlot(DynamicBlock block, Vector3 RelativeTranslation, float RelativeRotationY, DeferredRenderer renderer)
        {
            var ret = new BuildSlot(block, RelativeTranslation, RelativeRotationY, renderer);
            container.AddObject(ret);
            return ret;
        }

        public BuildSlot createBuildSlot(DynamicBlock block, Vector3 RelativeTranslation, float RelativeRotationY, Vector3 scaling, DeferredRenderer renderer)
        {
            var ret = new BuildSlot(block, RelativeTranslation, RelativeRotationY, scaling, renderer);
            container.AddObject(ret);
            return ret;
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

        public static DynamicBlockDirection GetDirectionFromVector(Vector3 v)
        {

            if (v.X == 0)
            {
                if (v.Z == 0)
                    throw new Exception("invalid vector given");
                if (v.Z < 0)
                    return DynamicBlockDirection.MinZ;
                if (v.Z > 0)
                    return DynamicBlockDirection.Z;
            }
            if (v.X < 0)
            {
                if (v.Z == 0)
                    return DynamicBlockDirection.MinX;
                if (v.Z < 0)
                    return DynamicBlockDirection.MinXMinZ;
                if (v.Z > 0)
                    return DynamicBlockDirection.MinXZ;
            }
            if (v.X > 0)
            {
                if (v.Z == 0)
                    return DynamicBlockDirection.X;
                if (v.Z < 0)
                    return DynamicBlockDirection.XMinZ;
                if (v.Z > 0)
                    return DynamicBlockDirection.XZ;
            }

            return DynamicBlockDirection.X; // wont happen
        }

        public int GetNbSkewWalls()
        {
            throw new NotImplementedException();
        }

        public void EmptyAllSkewSlots()
        {
            throw new NotImplementedException();
        }
    }
}

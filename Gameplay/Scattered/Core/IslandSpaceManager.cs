using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProceduralBuilder.Building;
using ProceduralBuilder.Shapes;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class IslandSpaceManager
    {
        public List<IBuildingElement> BuildAreaMeshes;
        private List<BoundingBox> reservedSpots = new List<BoundingBox>();
        private Random rnd = new Random(0);

        /// <summary>
        /// Returns the position (island-space) of a free buildingspot of specified size.
        /// Returns null if no space is available.
        /// </summary>
        /// <param name="buildBox"></param>
        /// <returns></returns>
        public Vector3? GetBuildPosition(BoundingBox buildBox)
        {
            var testBox = new BoundingBox(buildBox.Minimum, buildBox.Maximum + new Vector3(0, 1, 0));

            var maxNbTries = 100f;
            while (maxNbTries > 0)
            {
                var pos = getRandomPositionOnMesh();
                var bb = getTranslatedBoundingBox(testBox, pos);
                if (isValidBuildLocation(bb))
                    return pos;

                maxNbTries--;
            }
            return null;
        }

        private Vector3 getRandomPositionOnMesh()
        {
            var index = rnd.Next(0, BuildAreaMeshes.Count);
            var randY = rnd.Next(1, 100) * 0.01f;
            var randX = rnd.Next(1, 100) * 0.01f;

            var selectedMesh = (Face)BuildAreaMeshes[index];
            var faceSize = selectedMesh.Size;
            var localPos = new Vector3(randX * faceSize.X, randY * faceSize.Y, 0);
            var transformedPos = Vector3.Transform(localPos, selectedMesh.WorldMatrix);

            return new Vector3(transformedPos.X, transformedPos.Y, transformedPos.Z);
        }

        /// <summary>
        /// Marks the specified buildspot as taken.
        /// Returns whether marking was succesfull.
        /// </summary>
        /// <param name="buildPos"></param>
        /// <param name="buildBox"></param>
        /// <returns></returns>
        public bool TakeBuildingSpot(Vector3 buildPos, BoundingBox buildBox)
        {
            var testBox = new BoundingBox(buildBox.Minimum, buildBox.Maximum + new Vector3(0, 1, 0));
            var bb = getTranslatedBoundingBox(testBox, buildPos);

            if (!isValidBuildLocation(bb))
                return false;

            reservedSpots.Add(bb);
            return true;
        }

        private BoundingBox getTranslatedBoundingBox(BoundingBox bb, Vector3 translation)
        {
            return new BoundingBox(bb.Minimum + translation, bb.Maximum + translation);
        }

        private bool isValidBuildLocation(BoundingBox bb)
        {
            foreach (var box in reservedSpots)
            {
                if (BoundingBox.Intersects(box, bb))
                    return false;
            }

            for (float i = bb.Minimum.X; i < bb.Maximum.X; i++)
            {
                for (float j = bb.Minimum.Z; j < bb.Maximum.Z; j++)
                {
                    if (!isPositionOnBuildMesh(new Vector2(i, j)))
                        return false;
                }
            }

            if (!isPositionOnBuildMesh(new Vector2(bb.Maximum.X, bb.Maximum.Z)))
                return false;

            return true;
        }

        private bool isPositionOnBuildMesh(Vector2 pos)
        {
            var ray = new Ray(new Vector3(pos.X, -1000, pos.Y), Vector3.UnitY);
            foreach (var el in BuildAreaMeshes)
            {
                var bb = el.GetBoundingBox();
                float dist;
                if (BoundingBox.Intersects(bb, ray, out dist))
                    return true;
            }
            return false;
        }

        public void ClearBuildingSpotReservations()
        {
            reservedSpots = new List<BoundingBox>();
        }
    }
}

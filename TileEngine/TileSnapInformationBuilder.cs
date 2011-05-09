using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class TileSnapInformationBuilder
    {
        private Dictionary<TileFaceType, SnapType> typesMap = new Dictionary<TileFaceType, SnapType>();
        Random randomNbGenerator = new Random();


        public SnapInformation CreateFromTile(TileData data)
        {


            SnapInformation information = new SnapInformation();

            for (int i = 0; i < 6; i++)
            {
                var tileFace = (TileFace)(i + 1);
                if (data.GetFaceType(tileFace) == null) continue;
                var faceType = data.GetFaceType(tileFace);

                SnapPoint point = GetPoint(data, tileFace, faceType, data.GetLocalWinding(tileFace) ^ faceType.GetTotalWinding());
                point.TileFaceType = faceType;

                information.addSnapObject(point);
            }

            return information;
        }

        public SnapPoint GetPoint(TileData data, TileFace tileFace, TileFaceType faceType, bool winding)
        {
            var point = new SnapPoint();

            point.Position = Math.Abs(Vector3.Dot((data.Dimensions * 0.5f), getFaceNormal(tileFace))) * getFaceNormal(tileFace);
            if (faceType != null)
                point.SnapType = getSnapType(faceType);
            point.Normal = getFaceNormal(tileFace);
            point.Up = getFaceUp(tileFace);
            point.ClockwiseWinding = winding;
            return point;
        }

        private SnapType getSnapType(TileFaceType faceType)
        {
            if (!typesMap.ContainsKey(faceType.GetRoot()))
            {
                typesMap.Add(faceType.GetRoot(), new SnapType("TileFaceType: " + randomNbGenerator.Next(10) + faceType.Name));
            }
            return typesMap[faceType.GetRoot()];
        }

        private static Vector3[] faceDirections;
        private static Vector3[] upDirections;

        static TileSnapInformationBuilder()
        {
            faceDirections = new Vector3[7];
            faceDirections[5] = Vector3.Backward;
            faceDirections[6] = Vector3.Forward;
            faceDirections[1] = Vector3.Up;
            faceDirections[2] = Vector3.Down;
            faceDirections[3] = Vector3.Left;
            faceDirections[4] = Vector3.Right;

            upDirections = new Vector3[7];
            upDirections[5] = Vector3.Up;
            upDirections[6] = Vector3.Up;
            upDirections[1] = Vector3.Forward;
            upDirections[2] = Vector3.Forward;
            upDirections[3] = Vector3.Up;
            upDirections[4] = Vector3.Up;
        }

        public static Vector3 getFaceNormal(TileFace face)
        {
            return faceDirections[(int)face];
        }
        public static Vector3 getFaceUp(TileFace face)
        {
            return upDirections[(int)face];
        }

    }
}

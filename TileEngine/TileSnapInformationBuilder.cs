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

        public SnapInformation CreateFromTile(TileData data)
        {
           

            SnapInformation information = new SnapInformation();

            for (int i = 0 ; i < 6 ; i++)
            {
                var tileFace = (TileFace)(i+1);
                var faceType = data.GetFaceType(tileFace);

                if (data.GetFaceType(tileFace) == null) continue;
                if (!typesMap.ContainsKey(faceType))
                {
                    typesMap.Add(faceType, new SnapType("TileFaceType: " + faceType.Name));
                }

                var point = new SnapPoint();

                point.Position = Math.Abs(Vector3.Dot((data.Dimensions * 0.5f), faceDirections[i])) * faceDirections[i];
                point.SnapType = typesMap[faceType];
                point.Normal = faceDirections[i];
                point.Up = upDirections[i];
                point.ClockwiseWinding = data.GetWinding(tileFace);
                

                information.addSnapObject(point);
            }

            return information;
        }

        private static Vector3[] faceDirections;
        private static Vector3[] upDirections;

        static TileSnapInformationBuilder ()
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

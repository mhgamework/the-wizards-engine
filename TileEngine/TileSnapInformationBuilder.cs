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
            Vector3[] faceDirections = new Vector3[6];
            faceDirections[(int)TileFace.Back - 1] = Vector3.Backward;
            faceDirections[(int)TileFace.Front - 1] = Vector3.Forward;
            faceDirections[(int)TileFace.Top - 1] = Vector3.Up;
            faceDirections[(int)TileFace.Bottom - 1] = Vector3.Down;
            faceDirections[(int)TileFace.Left - 1] = Vector3.Left;
            faceDirections[(int)TileFace.Right - 1] = Vector3.Right;

            Vector3[] upDirections = new Vector3[6];
            upDirections[(int)TileFace.Back - 1] = Vector3.Up;
            upDirections[(int)TileFace.Front - 1] = Vector3.Up;
            upDirections[(int)TileFace.Top - 1] = Vector3.Forward;
            upDirections[(int)TileFace.Bottom - 1] = Vector3.Forward;
            upDirections[(int)TileFace.Left - 1] = Vector3.Up;
            upDirections[(int)TileFace.Right - 1] = Vector3.Up;

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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.TileEngine
{
    public class SnapInformation
    {
        public List<SnapPoint> SnapList = new List<SnapPoint>();        
                
        public void addSnapObject(SnapPoint item)
        {
            SnapList.Add(item);
        }

        
        //Doesn't belong here, only used in first tests
        public void Snap(SnapPoint source, SnapPoint target)
        {
            Vector3 snapPointPos = source.Position;

            //Rotate by Normal            
            Vector3 sNormal = source.Normal;
            Vector3 tNormal = target.Normal;

            float normalRotationAngle = (float)Math.Acos((Vector3.Dot(sNormal, tNormal))) + MathHelper.Pi;
            Vector3 normalRotationAxis = Vector3.Cross(sNormal, tNormal);

            Matrix normalRotationMatrix = Matrix.CreateFromAxisAngle(normalRotationAxis, normalRotationAngle);

            for (int i = 0; i < SnapList.Count; i++)
            {
                SnapPoint point = SnapList[i];
                point.Position = point.Position - snapPointPos;

                point.Position = Vector3.Transform(point.Position, normalRotationMatrix);
                point.Normal = Vector3.Transform(point.Normal, normalRotationMatrix);
                point.Up = Vector3.Transform(point.Up, normalRotationMatrix);

                point.Position = point.Position + snapPointPos;
            }


            //Rotate by Up
            Vector3 sUp = source.Up;
            Vector3 tUp = target.Up;

            float upRotationAngle = (float)Math.Acos((Vector3.Dot(sUp, tUp)));

            Vector3 upRotationAxis = new Vector3();
            if (sUp == tUp)
            {
                upRotationAxis = target.Normal * (-1);
            }
            else
            {
                upRotationAxis = Vector3.Cross(sUp, tUp);
            }

            Matrix upRotationMatrix = Matrix.CreateFromAxisAngle(upRotationAxis, upRotationAngle);

            for (int i = 0; i < SnapList.Count; i++)
            {
                SnapPoint point = SnapList[i];
                point.Position = point.Position - snapPointPos;

                point.Position = Vector3.Transform(point.Position, upRotationMatrix);
                point.Normal = Vector3.Transform(point.Normal, upRotationMatrix);
                point.Up = Vector3.Transform(point.Up, upRotationMatrix);

                point.Position = point.Position + snapPointPos;
            }


            //Translate
            Vector3 translation = target.Position - source.Position;

            for (int i = 0; i < SnapList.Count; i++)
            {
                SnapPoint point = SnapList[i];
                point.Position = point.Position + translation;
            }

        }

        

        

        
    }
}

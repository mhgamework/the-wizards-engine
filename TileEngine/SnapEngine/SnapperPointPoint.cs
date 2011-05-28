using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public class SnapperPointPoint : ISnapObjectSnapper
    {

        #region ISnapObjectSnapper Members

        public Type SnapObjectTypeA
        {
            get { return typeof(SnapPoint); }
        }

        public Type SnapObjectTypeB
        {
            get { return typeof(SnapPoint); }
        }

        public void SnapAToB(object A, object B, Transformation transformationB, List<Transformation> outTransformations)
        {

            

            SnapPoint PointA = (SnapPoint)A;
            SnapPoint PointB = (SnapPoint)B;

            if (Math.Abs(Vector3.Dot(PointA.Up, PointA.Normal)) >0.001f)
                throw new InvalidOperationException();
            if (Math.Abs(Vector3.Dot(PointB.Up, PointB.Normal)) > 0.001f)
                throw new InvalidOperationException();

            var newPoint = new SnapPoint();
            newPoint.ClockwiseWinding = PointB.ClockwiseWinding;
            newPoint.Position = Vector3.Transform(PointB.Position, transformationB.CreateMatrix());
            newPoint.Normal = Vector3.Transform(PointB.Normal, transformationB.Rotation);
            newPoint.Up = Vector3.Transform(PointB.Up, transformationB.Rotation);

            PointB = newPoint;

            if (PointA.ClockwiseWinding == PointB.ClockwiseWinding) return;

            Vector3 sNormal = PointA.Normal;
            Vector3 tNormal = PointB.Normal;
            float normalRotationAngle = (float)Math.Acos((Vector3.Dot(sNormal, tNormal))) + MathHelper.Pi;
            Vector3 normalRotationAxis = Vector3.Normalize(Vector3.Cross(sNormal, tNormal));

            if (Math.Abs(Vector3.Dot(sNormal, tNormal)) > 0.999)
                normalRotationAxis = PointB.Up* (-1);

            Quaternion normalRotation = Quaternion.CreateFromAxisAngle(normalRotationAxis, normalRotationAngle);

            Vector3 sUp = Vector3.Transform(PointA.Up, normalRotation);
            Vector3 tUp =PointB.Up;
            float upRotationAngle = (float)Math.Acos((Vector3.Dot(sUp, tUp)));
            
            Vector3 upRotationAxis;
            if (Math.Abs(Vector3.Dot(sUp, tUp)) > 0.999)
            {
                upRotationAxis = PointB.Normal* (-1);
            }
            else
            {
                upRotationAxis = Vector3.Normalize(Vector3.Cross(sUp, tUp));
            }

            if (Math.Abs(Vector3.Dot(upRotationAxis , tNormal)) < 0.999f)
            {
                throw new InvalidOperationException();
            }

            Quaternion upRotationMatrix = Quaternion.CreateFromAxisAngle(upRotationAxis, upRotationAngle);
            Quaternion Rotation = upRotationMatrix * normalRotation;

            Vector3 translation = PointB.Position - Vector3.Transform(PointA.Position, Rotation);

            Quaternion RotQuat = Rotation;
            Transformation transformation = new Transformation(new Vector3(1, 1, 1), RotQuat, translation);


            //TODO: SCALING NOT SUPPORTED: transformation.Scaling *= transformationB.Scaling;
            /*transformation.Rotation = transformation.Rotation * transformationB.Rotation;
            transformation.Translation += transformationB.Translation;*/


            outTransformations.Add(transformation);
        }

        public void SnapBToA(object A, object B, Transformation transformationA, List<Transformation> outTransformations)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

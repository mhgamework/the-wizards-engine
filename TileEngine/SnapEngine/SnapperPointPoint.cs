using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public class SnapperPointPoint: ISnapObjectSnapper
    {

        #region ISnapObjectSnapper Members

        public Type SnapObjectTypeA
        {
            get { return typeof (SnapPoint); }
        }

        public Type SnapObjectTypeB
        {
            get { return typeof (SnapPoint); }
        }

        public void SnapAToB(object A, object B,Transformation transformationB, List<Transformation> outTransformations)
        {
            


            SnapPoint PointA = (SnapPoint) A;
            SnapPoint PointB = (SnapPoint) B;

            var newPoint = new SnapPoint();
            newPoint.ClockwiseWinding = PointB.ClockwiseWinding;
            newPoint.Position = Vector3.Transform(PointB.Position, transformationB.CreateMatrix());
            newPoint.Normal = Vector3.Transform(PointB.Normal, transformationB.Rotation);
            newPoint.Up = Vector3.Transform(PointB.Up, transformationB.Rotation);

            PointB = newPoint;

            if (PointA.ClockwiseWinding == PointB.ClockwiseWinding)
            {
                return;
            }
            
            Vector3 sNormal = PointA.Normal;
            Vector3 tNormal = PointB.Normal;
            float normalRotationAngle = (float)Math.Acos((Vector3.Dot(sNormal, tNormal))) + MathHelper.Pi;
            Vector3 normalRotationAxis = Vector3.Cross(sNormal, tNormal);

            if (Math.Abs(Vector3.Dot(sNormal, tNormal)) > 0.999)
                normalRotationAxis = PointB.Up * (-1);
            
            Matrix normalRotationMatrix = Matrix.CreateFromAxisAngle(normalRotationAxis, normalRotationAngle);                        
            
            Vector3 sUp = PointA.Up;
            Vector3 tUp = PointB.Up;
            float upRotationAngle = (float)Math.Acos((Vector3.Dot(sUp, tUp)));
            Vector3 upRotationAxis = new Vector3();
            if (Math.Abs(Vector3.Dot(sUp,tUp)) > 0.999)
            {
                upRotationAxis = PointB.Normal * (-1);
            }
            else
            {
                upRotationAxis = Vector3.Cross(sUp, tUp);
            }
            Matrix upRotationMatrix = Matrix.CreateFromAxisAngle(upRotationAxis, upRotationAngle);
            Matrix Rotation = normalRotationMatrix * upRotationMatrix;
            
            Vector3 translation = PointB.Position -Vector3.Transform( PointA.Position,Rotation);        

            Quaternion RotQuat = Quaternion.CreateFromRotationMatrix(Rotation);
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

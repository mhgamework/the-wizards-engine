using System;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.SceneObjects
{
    public class TransformedSceneObject : ISceneObject
    {
        private Matrix transformation;
        public Matrix Transformation
        {
            get { return transformation; }
            set { transformation = value; }
        }

        private ISceneObject Object { get; set; }
        public TransformedSceneObject(ISceneObject obj)
        {
            Object = obj;
            Transformation = Matrix.Identity;
        }

        public BoundingBox BoundingBox
        {
            get { throw new NotImplementedException(); }
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            Matrix inverse;
            Matrix.Invert(ref transformation, out inverse);

            var oriRay = trace.Ray;
            Vector3 pos = (trace.Ray.Position + trace.Ray.Direction * trace.Start);
            Vector3.TransformCoordinate(ref pos, ref inverse, out trace.Ray.Position);
            Vector3.TransformNormal(ref trace.Ray.Direction, ref  inverse, out trace.Ray.Direction);

            Object.Intersects(ref trace, ref result);

            trace.Ray = oriRay;
            result.Normal = Vector3.TransformNormal(result.Normal, transformation);

        }
    }
}
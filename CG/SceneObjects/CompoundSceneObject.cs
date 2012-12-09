using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public class CompoundSceneObject : ISceneObject
    {
        public List<ISceneObject> IncludeObjects { get; set; }
        public List<ISceneObject> ExcludeObjects { get; set; }
        public CompoundSceneObject()
        {
            IncludeObjects = new List<ISceneObject>();
            ExcludeObjects = new List<ISceneObject>();
        }

        public BoundingBox BoundingBox
        {
            get { throw new NotImplementedException(); }
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            //TODO
        }
    }
}
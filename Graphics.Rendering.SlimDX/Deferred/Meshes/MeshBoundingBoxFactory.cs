using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes
{
    /// <summary>
    /// Responsible for calculating and caching bounding boxes for meshes
    /// </summary>
    public class MeshBoundingBoxFactory
    {
        private Dictionary<IMesh, Vector3[]> cornersCache = new Dictionary<IMesh, Vector3[]>();
        public Vector3[] GetCorners(IMesh mesh )
        {
            if (cornersCache.ContainsKey(mesh)) return cornersCache[mesh];
            var ret = CalculateMeshBoundingBox(mesh).GetCorners();
            cornersCache[mesh] = ret;
            return ret;
        }

        public BoundingBox CalculateMeshBoundingBox(IMesh mesh)
        {
            //TODO: optimize!
            //Lol!
            // EDIT: MOAR LOL!
            return
                mesh.GetCoreData()
                    .Parts.Select(
                        part =>
                        (part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length == 0
                             ? new Microsoft.Xna.Framework.BoundingBox()
                             : Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(
                                 part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position))))
                    .Aggregate(new Microsoft.Xna.Framework.BoundingBox(), (current, t) => current.MergeWith(t)).dx();
        }

        public void ClearCache(IMesh mesh)
        {
            cornersCache.Remove(mesh);
        }
    }
}
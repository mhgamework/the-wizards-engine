using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// Responsible for creating an Optimized version of an IMesh (optimzed for use with deferredmeshrenderer atm)
    /// </summary>
    public class MeshOptimizer
    {

        private Dictionary<MeshCoreData.Material, IMeshPart> materials =
            new Dictionary<MeshCoreData.Material, IMeshPart>();
        private RAMMesh mesh;


        /// <summary>
        /// This might not be a deep copy, so changes to the original may affect the created mesh (this is bug but im lazy)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public IMesh CreateOptimized(IMesh original)
        {
            clearBuffers();

            mesh = new RAMMesh();
            copyCollisionData(mesh, original);

            foreach (var oriPart in original.GetCoreData().Parts)
            {
                var part = findOrCreatePart(oriPart.MeshMaterial);
                addPartToPart(oriPart.MeshPart, part, oriPart.ObjectMatrix);
            }

            return mesh;
        }

        private void addPartToPart(IMeshPart source, IMeshPart dest, Matrix sourceTransform)
        {
            var positions = source.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            var normals = source.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            var texcoords = source.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);

            Vector3.Transform(positions, ref sourceTransform, positions);
            Vector3.TransformNormal(normals, ref sourceTransform, normals);

            var destPositions = dest.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            if (destPositions == null) destPositions = new Vector3[0];

            positions = positions.Concat(destPositions).ToArray();

            var destNormals = dest.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            if (destNormals == null) destNormals = new Vector3[0];
            normals = normals.Concat(destNormals).ToArray();

            var destTexcoords = dest.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
            if (destTexcoords == null) destTexcoords = new Vector2[0];
            texcoords = texcoords.Concat(destTexcoords).ToArray();


            dest.GetGeometryData().SetSource(MeshPartGeometryData.Semantic.Position, positions);
            dest.GetGeometryData().SetSource(MeshPartGeometryData.Semantic.Normal, normals);
            dest.GetGeometryData().SetSource(MeshPartGeometryData.Semantic.Texcoord, texcoords);
        }

        private IMeshPart findOrCreatePart(MeshCoreData.Material original)
        {
            foreach (var mat in materials)
                if (isIdenticalMaterial(original, mat.Key)) return mat.Value;

            var copy = createMaterialCopy(original);
            var ret = createPart(copy);
            materials.Add(copy, ret);

            return ret;

        }

        private IMeshPart createPart(MeshCoreData.Material material)
        {
            var ret = new RAMMeshPart();
            var part = new MeshCoreData.Part();
            part.MeshMaterial = material;
            part.ObjectMatrix = Matrix.Identity;
            part.MeshPart = ret;

            mesh.GetCoreData().Parts.Add(part);

            return ret;
        }

        private bool isIdenticalMaterial(MeshCoreData.Material a, MeshCoreData.Material b)
        {
            if (a.DiffuseColor != b.DiffuseColor) return false;
            if (a.DiffuseMap != b.DiffuseMap) return false;
            return true;
        }

        private MeshCoreData.Material createMaterialCopy(MeshCoreData.Material original)
        {
            return new MeshCoreData.Material { DiffuseMap = original.DiffuseMap, DiffuseColor = original.DiffuseColor };
        }

        private void clearBuffers()
        {
            materials.Clear();
        }

        private void copyCollisionData(RAMMesh ret, IMesh original)
        {
            ret.GetCollisionData().Boxes.AddRange(original.GetCollisionData().Boxes);
            ret.GetCollisionData().ConvexMeshes.AddRange(original.GetCollisionData().ConvexMeshes);
            ret.GetCollisionData().TriangleMesh = original.GetCollisionData().TriangleMesh;
        }
    }
}

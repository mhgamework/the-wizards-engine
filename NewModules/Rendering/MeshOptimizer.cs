using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// Responsible for creating an Optimized version of an IMesh (optimzed for use with deferredmeshrenderer atm)
    /// Merges parts with identical materials.
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
            var sPositions = source.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            var sNormals = source.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            var sTexcoords = source.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);

            var dPositions = dest.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            if (dPositions == null) dPositions = new Vector3[0];
            var dNormals = dest.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            if (dNormals == null) dNormals = new Vector3[0];
            var dTexcoords = dest.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
            if (dTexcoords == null) dTexcoords = new Vector2[0];


            var nPositions = new Vector3[sPositions.Length + dPositions.Length];
            var nNormals = new Vector3[sNormals.Length + dNormals.Length];
            var nTexcoords = new Vector2[sTexcoords.Length + dTexcoords.Length];


            dPositions.CopyTo(nPositions, 0);
            dNormals.CopyTo(nNormals, 0);
            dTexcoords.CopyTo(nTexcoords, 0);


            Vector3.Transform(sPositions, 0, ref sourceTransform, nPositions, dPositions.Length, sPositions.Length);
            Vector3.TransformNormal(sNormals, 0, ref sourceTransform, nNormals, dNormals.Length, sNormals.Length);
            sTexcoords.CopyTo(nTexcoords, dTexcoords.Length);


            dest.GetGeometryData().SetSource(MeshPartGeometryData.Semantic.Position, nPositions);
            dest.GetGeometryData().SetSource(MeshPartGeometryData.Semantic.Normal, nNormals);
            dest.GetGeometryData().SetSource(MeshPartGeometryData.Semantic.Texcoord, nTexcoords);
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
            if (a.Name != b.Name) return false;
            return true;
        }

        private MeshCoreData.Material createMaterialCopy(MeshCoreData.Material original)
        {
            return new MeshCoreData.Material { DiffuseMap = original.DiffuseMap, DiffuseColor = original.DiffuseColor, Name = original.Name };
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

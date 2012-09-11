using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Pickup
{
    public class ItemEntity
    {
        private WorldRendering.Entity ent;
        public bool RayTraceable;
        public BoundingBox BB;

        public ItemEntity()
        {
            ent = new WorldRendering.Entity();
            ent.Visible = true;
            BB = new BoundingBox();
        }

        public void SetMesh(IMesh mesh)
        {
            ent.Mesh = mesh;
            calculateBoundingBox();
        }

        public IMesh GetMesh()
        {
            return ent.Mesh;
        }

        public void SetPosition(Matrix matrix)
        {
            ent.WorldMatrix = matrix;
            calculateBoundingBox();
        }

        public Vector3 GetPosition()
        {
            Vector3 scale = new Vector3();
            Quaternion rotation = new Quaternion();
            Vector3 translation = new Vector3();
            ent.WorldMatrix.Decompose(out scale, out rotation, out translation);

            return translation;
        }

        public Quaternion GetRotation()
        {
            Vector3 scale = new Vector3();
            Quaternion rotation = new Quaternion();
            Vector3 translation = new Vector3();
            ent.WorldMatrix.Decompose(out scale, out rotation, out translation);

            return rotation;
        }

        public void SetVisibility(bool visible)
        {
            ent.Visible = visible;
        }

        public void Delete()
        {
            TW.Model.RemoveObject(ent);
        }

        private void calculateBoundingBox()
        {
            var mesh = ent.Mesh;
            if (mesh == null)
                return;

            var points = new List<Vector3>();
            foreach (var part in mesh.GetCoreData().Parts)
            {
                MeshCoreData.Part part1 = part;
                points.AddRange(
                    part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Select(o => Vector3.TransformCoordinate(o.dx(), part1.ObjectMatrix.dx())));
            }

            BB = BoundingBox.FromPoints(points.ToArray());
        }


    }
}

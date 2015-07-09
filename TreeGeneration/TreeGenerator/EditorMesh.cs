using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldDatabase;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.EntityOud.Editor
{
    [Obsolete("RamMesh is probably the same")]
    public class EditorMesh : IMesh
    {

        public MeshCoreData CoreData = new MeshCoreData();
        public MeshAdditionalData AdditionalData = new MeshAdditionalData();

        private List<Part> parts = new List<Part>();

        private bool changed = false;

        

        /// <summary>
        /// Adds a new part to this mesh
        /// </summary>
        /// <returns></returns>
        public Part AddPart(EditorMeshPart eMeshPart)
        {
            Part p = new Part();
            p.MeshPart = eMeshPart;
            p.ObjectMatrix = Matrix.Identity;

            parts.Add(p);

            MeshCoreData.Part part = new MeshCoreData.Part();
            part.ObjectMatrix = Matrix.Identity;
            part.MeshPart = eMeshPart;
            CoreData.Parts.Add(part);

            return p;
        }

        public class Part
        {
            public EditorMeshPart MeshPart;
            public Matrix ObjectMatrix;
            //Material


        }

        public Guid Guid { get; set; }

        public MeshCoreData GetCoreData()
        {
            return CoreData;
        }

        public MeshCollisionData GetCollisionData()
        {
            throw new NotImplementedException();
        }
    }
}

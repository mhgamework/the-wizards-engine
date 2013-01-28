using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldDatabase;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.EntityOud.Editor
{
    public class EditorMesh : IMesh
    {
        private DataItem dataItem;

        private WorldDatabase.WorldDatabase database;

        public MeshCoreData CoreData = new MeshCoreData();
        public MeshAdditionalData AdditionalData = new MeshAdditionalData();

        private List<Part> parts = new List<Part>();

        private bool changed = false;


        public EditorMesh()
        {
            database = null;
            dataItem = null;

        }

        private EditorMesh(WorldDatabase.WorldDatabase _database, DataItem item)
        {
            database = _database;
            dataItem = item;

        }

        public static EditorMesh CreateNew(WorldDatabase.WorldDatabase database)
        {
            DataItem item = database.CreateNewDataItem(database.FindOrCreateDataItemType("Mesh"));

            EditorMesh eMesh = new EditorMesh(database, item);
            eMesh.changed = true;
            return eMesh;
        }

        /// <summary>
        /// Set this to true if you make any changes to this object. Only when set, the changes will be stored to the database
        /// </summary>
        /// <param name="value"></param>
        public void setChanged(bool value)
        {
            changed = value;
        }

        /// <summary>
        /// Flushes all cached changes and stores them in the dataelements. and puts them in the database.
        /// Also calls flushCache on the meshParts
        /// </summary>
        public void FlushCache()
        {
            database.WorkingCopy.PutDataElement(dataItem, CoreData);
            database.WorkingCopy.PutDataElement(dataItem, AdditionalData);

            //TODO
        }

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

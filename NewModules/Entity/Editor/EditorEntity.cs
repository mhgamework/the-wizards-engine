using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities.Editor;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.ServerClient.Database;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entities.Editor
{
    /// <summary>
    /// An EditorObject acts as a type. An Entity is an instance of an Object that resides in the world.
    /// Therefore it has a transform, whereas the EditorObject has not.
    /// </summary>
    public class EditorEntity : MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedEntity>, Raycast.IRaycastable<EditorEntity, Raycast.GeometryRaycastResult<EditorEntity>>, IEntity
    {
        public EntityCoreData CoreData = new EntityCoreData();

        private EntityFullData fullData;

        public EntityFullData FullData
        {
            get { return fullData; }
            set { fullData = value; }
        }

        private EditorObject editorObject;

        public EditorObject EditorObject
        {
            get { return editorObject; }
            set
            {
                editorObject = value; //fullData.TaggedObject = editorObject.TaggedObject;
            }
        }

        private TaggedEntity taggedEntity;
        public TaggedEntity TaggedEntity
        {
            get { return taggedEntity; }
            set { taggedEntity = value; }
        }

        public EditorEntity()//EditorObject obj)
        {

            //editorObject = obj;
            //fullData = new EntityFullData();
            //fullData.ObjectFullData = obj.FullData;
        }


        void ISimpleTag<TaggedEntity>.InitTag(TaggedEntity obj)
        {
            taggedEntity = obj;
            fullData = obj.GetTag<EntityFullData>();
            if (fullData.TaggedObject != null)
                editorObject = fullData.TaggedObject.GetTag<EditorObject>();
            //throw new System.NotImplementedException();
        }

        void ISimpleTag<TaggedEntity>.AddReference(TaggedEntity obj)
        {
            //throw new System.NotImplementedException();
        }


        public float? RaycastEntity(Ray ray, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3)
        {
            return editorObject.RaycastObject(ray, fullData.Transform.CreateMatrix(), out vertex1, out vertex2, out vertex3);
        }

        public void SaveToDisk()
        {
            fullData.SaveToDisk(editorObject.TaggedObject.TagManager.Database);
        }


        public MHGameWork.TheWizards.Raycast.GeometryRaycastResult<EditorEntity> Raycast(Ray ray)
        {
            Vector3 vertex1, vertex2, vertex3;
            float? distance = editorObject.RaycastObject(ray, fullData.Transform.CreateMatrix(), out vertex1, out vertex2, out vertex3);
            if (distance.HasValue == false) return null;
            TheWizards.Raycast.GeometryRaycastResult<EditorEntity> result =
                new MHGameWork.TheWizards.Raycast.GeometryRaycastResult<EditorEntity>(
                    distance,
                    this,
                    ray.Position + ray.Direction * distance.Value, vertex1, vertex2, vertex3);

            return result;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.WorldDatabase;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity
{
    /// <summary>
    /// TODO: Maybe this class should be splitted into sections, now seperated by comments
    /// TODO: This may prevents unnecessary data to be loaded
    /// </summary>
    public class ObjectFullData : MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>, IDataElement
    {
        private TaggedObject taggedObject;

        public TaggedObject TaggedObject
        {
            get { return taggedObject; }
        }

        /**
         * 
         * Physical Data
         * 
         **/

        //IMPORTANT!!!! An Object does not have a transform!!!! an entity does
        /*private Transformation transform;

        public Transformation Transform
        {
            get { return transform; }
            set { transform = value; }
        }*/

        private BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        private BoundingSphere boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
            set { boundingSphere = value; }
        }




        /**
         * 
         * Various
         * 
         **/

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        private List<ModelFullData> models;

        public List<ModelFullData> Models
        {
            get { return models; }
            set { models = value; }
        }

        public ObjectFullData()
        {
            models = new List<ModelFullData>();
            name = "Noname";
            //transform = Transformation.Identity;
        }

        /// <summary>
        /// Note, now this handles the file it serializes to itself. Maybe this should be managed by a tag in TaggedObject?
        /// </summary>
        /// <param name="database"></param>
        public void SaveToDisk(Database.Database database)
        {
            throw new NotImplementedException();
            //MHGameWork.TheWizards.ServerClient.Database.DiskSerializerService dss = database.FindService<MHGameWork.TheWizards.ServerClient.Database.DiskSerializerService>();
            //MHGameWork.TheWizards.ServerClient.Database.IXMLFile file = dss.OpenXMLFile("Objects/" + this.TaggedObject.UniqueID + "-FullData.txt", "Entity.ObjectFullData");
            //TWXmlNode node = file.RootNode;
            //node.Clear();

            //ObjectFullDataFactory.SaveToXML(this, node);

            //file.SaveToDisk();
        }
        private void LoadFromDisk(Database.Database database)
        {
            throw new NotImplementedException();
            //MHGameWork.TheWizards.ServerClient.Database.DiskSerializerService dss = database.FindService<MHGameWork.TheWizards.ServerClient.Database.DiskSerializerService>();
            //MHGameWork.TheWizards.ServerClient.Database.IXMLFile file = dss.OpenXMLFile("Objects/" + this.TaggedObject.UniqueID + "-FullData.txt", "Entity.ObjectFullData");
            //TWXmlNode node = file.RootNode;

            //ObjectFullDataFactory.LoadFromXML(this, node);
        }



        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.InitTag(TaggedObject obj)
        {
            taggedObject = obj;
            LoadFromDisk(obj.TagManager.Database);
            //throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.AddReference(TaggedObject obj)
        {
            //throw new Exception( "The method or operation is not implemented." );
        }


        public bool Equals(ObjectFullData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.boundingBox.Equals(boundingBox) && other.boundingSphere.Equals(boundingSphere) && Equals(other.name, name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ObjectFullData)) return false;
            return Equals((ObjectFullData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = boundingBox.GetHashCode();
                result = (result*397) ^ boundingSphere.GetHashCode();
                result = (result*397) ^ (name != null ? name.GetHashCode() : 0);
                return result;
            }
        }
    }
}
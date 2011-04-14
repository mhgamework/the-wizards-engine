using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Entity
{
    /// <summary>
    /// TODO: Maybe this class should be splitted into sections, now seperated by comments
    /// TODO: This may prevents unnecessary data to be loaded
    /// </summary>
    public class ObjectFullData : Database.ISimpleTag<TaggedObject>
    {
        private TaggedObject taggedObject;

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
            //transform = Transformation.Identity;
        }

        /// <summary>
        /// Note, now this handles the file it serializes to itself. Maybe this should be managed by a tag in TaggedObject?
        /// </summary>
        /// <param name="database"></param>
        public void SaveToDisk( Database.Database database )
        {
            Database.DiskSerializerService dss = database.FindService<Database.DiskSerializerService>();
            Database.IXMLFile file = dss.OpenXMLFile( "Objects/" + taggedObject.UniqueID + "-FullData.txt" ,"Entity.ObjectFullData");
            TWXmlNode node = file.RootNode;
            node.Clear();
            ObjectFullData data = this;

            node.AddChildNode( "Name", data.Name );
            XMLSerializer.WriteBoundingBox( node.CreateChildNode( "BoundingBox" ), data.BoundingBox );
            XMLSerializer.WriteBoundingSphere( node.CreateChildNode( "BoundingSphere" ), data.BoundingSphere );


            TWXmlNode modelsNode = node.CreateChildNode( "Models" );
            modelsNode.AddAttributeInt( "Count", data.Models.Count );

            for ( int i = 0; i < data.Models.Count; i++ )
            {
                ModelFullData model = data.Models[ i ];
                TWXmlNode modelNode = modelsNode.CreateChildNode( "EditorModelFullData" );
                SaveEditorModelFullData( model, modelNode );
            }

            file.SaveToDisk();

        }
        private void LoadFromDisk( Database.Database database )
        {
            Database.DiskSerializerService dss = database.FindService<Database.DiskSerializerService>();
            Database.IXMLFile file = dss.OpenXMLFile( "Objects/" + taggedObject.UniqueID + "-FullData.txt", "Entity.ObjectFullData" );
            TWXmlNode node = file.RootNode;
            ObjectFullData FullData = this;

            TWXmlNode nameNode = node.FindChildNode( "Name" );
            if ( nameNode == null ) return;

            
            FullData.Name = node.ReadChildNodeValue( "Name" );
            FullData.BoundingBox = XMLSerializer.ReadBoundingBox( node.FindChildNode( "BoundingBox" ) );
            FullData.BoundingSphere = XMLSerializer.ReadBoundingSphere( node.FindChildNode( "BoundingSphere" ) );

            TWXmlNode modelsNode = node.FindChildNode( "Models" );
            int count = modelsNode.GetAttributeInt( "Count" );

            TWXmlNode[] modelNodes = modelsNode.GetChildNodes();

            for ( int i = 0; i < modelNodes.Length; i++ )
            {
                TWXmlNode modelNode = modelNodes[ i ];

                ModelFullData model = LoadEditorModelFullData( modelNode );

                FullData.Models.Add( model );
            }
        }

        private void SaveEditorModelFullData( ModelFullData data, TWXmlNode node )
        {
            XMLSerializer.WriteVector3Array( node.CreateChildNode( "Positions" ), data.Positions );
            XMLSerializer.WriteVector3Array( node.CreateChildNode( "Normals" ), data.Normals );
            XMLSerializer.WriteVector3Array( node.CreateChildNode( "Tangents" ), data.Tangents );
            XMLSerializer.WriteVector3Array( node.CreateChildNode( "TexCoords" ), data.TexCoords );

            node.AddChildNode( "TriangleCount", data.TriangleCount.ToString() );



            node.AddChildNode( "MaterialName", data.MaterialName );

            XMLSerializer.WriteColor( node.CreateChildNode( "Ambient" ), data.Ambient );
            XMLSerializer.WriteColor( node.CreateChildNode( "Diffuse" ), data.Diffuse );
            XMLSerializer.WriteColor( node.CreateChildNode( "Specular" ), data.Specular );

            node.AddChildNode( "Shininess", data.Shininess.ToString() );


            node.AddChildNode( "DiffuseTexture", data.DiffuseTexture );
            node.AddChildNode( "DiffuseTextureRepeatU", data.DiffuseTextureRepeatU.ToString() );
            node.AddChildNode( "DiffuseTextureRepeatV", data.DiffuseTextureRepeatV.ToString() );

            node.AddChildNode( "NormalTexture", data.NormalTexture );
            node.AddChildNode( "NormalTextureRepeatU", data.NormalTextureRepeatU.ToString() );
            node.AddChildNode( "NormalTextureRepeatV", data.NormalTextureRepeatV.ToString() );


            node.AddChildNode( "OriginalFilePath", data.OriginalFilePath );

            XMLSerializer.WriteMatrix( node.CreateChildNode( "ObjectMatrix" ), data.ObjectMatrix );

            XMLSerializer.WriteBoundingBox( node.CreateChildNode( "BoundingBox" ), data.BoundingBox );
            XMLSerializer.WriteBoundingSphere( node.CreateChildNode( "BoundingSphere" ), data.BoundingSphere );

        }
        private ModelFullData LoadEditorModelFullData( TWXmlNode node )
        {
            ModelFullData data = new ModelFullData();

            data.Positions = XMLSerializer.ReadVector3Array( node.FindChildNode( "Positions" ) );
            data.Normals = XMLSerializer.ReadVector3Array( node.FindChildNode( "Normals" ) );
            data.Tangents = XMLSerializer.ReadVector3Array( node.FindChildNode( "Tangents" ) );
            data.TexCoords = XMLSerializer.ReadVector3Array( node.FindChildNode( "TexCoords" ) );

            data.TriangleCount = int.Parse( node.ReadChildNodeValue( "TriangleCount" ) );


            data.MaterialName = node.ReadChildNodeValue( "MaterialName" );

            data.Ambient = XMLSerializer.ReadColor( node.FindChildNode( "Ambient" ) );
            data.Diffuse = XMLSerializer.ReadColor( node.FindChildNode( "Diffuse" ) );
            data.Specular = XMLSerializer.ReadColor( node.FindChildNode( "Specular" ) );


            data.Shininess = float.Parse( node.ReadChildNodeValue( "Shininess" ) );

            data.DiffuseTexture = node.ReadChildNodeValue( "DiffuseTexture" );
            data.DiffuseTextureRepeatU = float.Parse( node.ReadChildNodeValue( "DiffuseTextureRepeatU" ) );
            data.DiffuseTextureRepeatV = float.Parse( node.ReadChildNodeValue( "DiffuseTextureRepeatV" ) );

            data.NormalTexture = node.ReadChildNodeValue( "NormalTexture" );
            data.NormalTextureRepeatU = float.Parse( node.ReadChildNodeValue( "NormalTextureRepeatU" ) );
            data.NormalTextureRepeatV = float.Parse( node.ReadChildNodeValue( "NormalTextureRepeatV" ) );


            data.OriginalFilePath = node.ReadChildNodeValue( "OriginalFilePath" );

            data.ObjectMatrix = XMLSerializer.ReadMatrix( node.FindChildNode( "ObjectMatrix" ) );

            data.BoundingBox = XMLSerializer.ReadBoundingBox( node.FindChildNode( "BoundingBox" ) );
            data.BoundingSphere = XMLSerializer.ReadBoundingSphere( node.FindChildNode( "BoundingSphere" ) );

            return data;
        }


        #region ISimpleTag<TaggedObject> Members

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.InitTag( TaggedObject obj )
        {
            taggedObject = obj;
            LoadFromDisk( obj.TagManager.Database );
            //throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.ServerClient.Database.ISimpleTag<TaggedObject>.AddReference( TaggedObject obj )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
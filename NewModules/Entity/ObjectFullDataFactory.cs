using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Entity
{
    public class ObjectFullDataFactory : WorldDatabase.IDataElementFactory<ObjectFullData>
    {

        private WorldDatabase.WorldDatabase database;

        public ObjectFullDataFactory(WorldDatabase.WorldDatabase database)
        {
            this.database = database;
        }

        public static void SaveToXML(ObjectFullData data, TWXmlNode node)
        {

            node.AddChildNode("Name", data.Name);
            XMLSerializer.WriteBoundingBox(node.CreateChildNode("BoundingBox"), data.BoundingBox);
            XMLSerializer.WriteBoundingSphere(node.CreateChildNode("BoundingSphere"), data.BoundingSphere);


            TWXmlNode modelsNode = node.CreateChildNode("Models");
            modelsNode.AddAttributeInt("Count", data.Models.Count);

            for (int i = 0; i < data.Models.Count; i++)
            {
                ModelFullData model = data.Models[i];
                TWXmlNode modelNode = modelsNode.CreateChildNode("EditorModelFullData");
                SaveEditorModelFullData(model, modelNode);
            }
        }
        public static void LoadFromXML(ObjectFullData FullData, TWXmlNode node)
        {
            TWXmlNode nameNode = node.FindChildNode("Name");
            if (nameNode == null) return;


            FullData.Name = node.ReadChildNodeValue("Name");
            FullData.BoundingBox = XMLSerializer.ReadBoundingBox(node.FindChildNode("BoundingBox"));
            FullData.BoundingSphere = XMLSerializer.ReadBoundingSphere(node.FindChildNode("BoundingSphere"));

            TWXmlNode modelsNode = node.FindChildNode("Models");
            int count = modelsNode.GetAttributeInt("Count");

            TWXmlNode[] modelNodes = modelsNode.GetChildNodes();

            for (int i = 0; i < modelNodes.Length; i++)
            {
                TWXmlNode modelNode = modelNodes[i];

                ModelFullData model = LoadEditorModelFullData(modelNode);

                FullData.Models.Add(model);
            }
        }

        private static void SaveEditorModelFullData(ModelFullData data, TWXmlNode node)
        {
            XMLSerializer.WriteVector3Array(node.CreateChildNode("Positions"), data.Positions);
            XMLSerializer.WriteVector3Array(node.CreateChildNode("Normals"), data.Normals);
            XMLSerializer.WriteVector3Array(node.CreateChildNode("Tangents"), data.Tangents);
            XMLSerializer.WriteVector3Array(node.CreateChildNode("TexCoords"), data.TexCoords);

            node.AddChildNode("TriangleCount", data.TriangleCount.ToString());



            node.AddChildNode("MaterialName", data.MaterialName);

            XMLSerializer.WriteColor(node.CreateChildNode("Ambient"), data.Ambient);
            XMLSerializer.WriteColor(node.CreateChildNode("Diffuse"), data.Diffuse);
            XMLSerializer.WriteColor(node.CreateChildNode("Specular"), data.Specular);

            node.AddChildNode("Shininess", data.Shininess.ToString());


            node.AddChildNode("DiffuseTexture", data.DiffuseTexture);
            node.AddChildNode("DiffuseTextureRepeatU", data.DiffuseTextureRepeatU.ToString());
            node.AddChildNode("DiffuseTextureRepeatV", data.DiffuseTextureRepeatV.ToString());

            node.AddChildNode("NormalTexture", data.NormalTexture);
            node.AddChildNode("NormalTextureRepeatU", data.NormalTextureRepeatU.ToString());
            node.AddChildNode("NormalTextureRepeatV", data.NormalTextureRepeatV.ToString());


            node.AddChildNode("OriginalFilePath", data.OriginalFilePath);

            XMLSerializer.WriteMatrix(node.CreateChildNode("ObjectMatrix"), data.ObjectMatrix);

            XMLSerializer.WriteBoundingBox(node.CreateChildNode("BoundingBox"), data.BoundingBox);
            XMLSerializer.WriteBoundingSphere(node.CreateChildNode("BoundingSphere"), data.BoundingSphere);

        }
        private static ModelFullData LoadEditorModelFullData(TWXmlNode node)
        {
            ModelFullData data = new ModelFullData();

            data.Positions = XMLSerializer.ReadVector3Array(node.FindChildNode("Positions"));
            data.Normals = XMLSerializer.ReadVector3Array(node.FindChildNode("Normals"));
            data.Tangents = XMLSerializer.ReadVector3Array(node.FindChildNode("Tangents"));
            data.TexCoords = XMLSerializer.ReadVector3Array(node.FindChildNode("TexCoords"));

            data.TriangleCount = int.Parse(node.ReadChildNodeValue("TriangleCount"));


            data.MaterialName = node.ReadChildNodeValue("MaterialName");

            data.Ambient = XMLSerializer.ReadColor(node.FindChildNode("Ambient"));
            data.Diffuse = XMLSerializer.ReadColor(node.FindChildNode("Diffuse"));
            data.Specular = XMLSerializer.ReadColor(node.FindChildNode("Specular"));


            data.Shininess = float.Parse(node.ReadChildNodeValue("Shininess"));

            data.DiffuseTexture = node.ReadChildNodeValue("DiffuseTexture");
            data.DiffuseTextureRepeatU = float.Parse(node.ReadChildNodeValue("DiffuseTextureRepeatU"));
            data.DiffuseTextureRepeatV = float.Parse(node.ReadChildNodeValue("DiffuseTextureRepeatV"));

            data.NormalTexture = node.ReadChildNodeValue("NormalTexture");
            data.NormalTextureRepeatU = float.Parse(node.ReadChildNodeValue("NormalTextureRepeatU"));
            data.NormalTextureRepeatV = float.Parse(node.ReadChildNodeValue("NormalTextureRepeatV"));


            data.OriginalFilePath = node.ReadChildNodeValue("OriginalFilePath");

            data.ObjectMatrix = XMLSerializer.ReadMatrix(node.FindChildNode("ObjectMatrix"));

            data.BoundingBox = XMLSerializer.ReadBoundingBox(node.FindChildNode("BoundingBox"));
            data.BoundingSphere = XMLSerializer.ReadBoundingSphere(node.FindChildNode("BoundingSphere"));

            return data;
        }



        public ObjectFullData ReadFromDisk(MHGameWork.TheWizards.WorldDatabase.DataItemIdentifier item, MHGameWork.TheWizards.WorldDatabase.DataRevisionIdentifier revision)
        {
            ObjectFullData data = new ObjectFullData();

            TWXmlNode root = TWXmlNode.GetRootNodeFromFile(GetFilename(item, revision));
            if (root.Name != "ObjectFullData") throw new Exception("Invalid file format");
            if (root.ReadChildNodeValue("Factory") != GetUniqueName()) throw new Exception("Invalid factory!");


            LoadFromXML(data, root);

            return data;
        }
        public void WriteToDisk(MHGameWork.TheWizards.WorldDatabase.DataItemIdentifier item, MHGameWork.TheWizards.WorldDatabase.DataRevisionIdentifier revision, ObjectFullData dataElement)
        {
            TWXmlNode root = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "ObjectFullData");
            root.AddChildNode("Factory", GetUniqueName());

            SaveToXML(dataElement, root);

            root.Document.Save(GetFilename(item, revision));
        }



        public string GetUniqueName()
        {
            return "ObjectFullDataFactory001";
        }

        MHGameWork.TheWizards.WorldDatabase.IDataElement MHGameWork.TheWizards.WorldDatabase.IDataElementFactory.ReadFromDisk(MHGameWork.TheWizards.WorldDatabase.DataItemIdentifier item, MHGameWork.TheWizards.WorldDatabase.DataRevisionIdentifier revision)
        {
            return ReadFromDisk(item, revision);
        }
        public void WriteToDisk(MHGameWork.TheWizards.WorldDatabase.DataItemIdentifier item, MHGameWork.TheWizards.WorldDatabase.DataRevisionIdentifier revision, MHGameWork.TheWizards.WorldDatabase.IDataElement dataElement)
        {
            if (!(dataElement is ObjectFullData))
                throw new InvalidOperationException("Can only serialize ObjectFullData!");

            WriteToDisk(item, revision, dataElement as ObjectFullData);
        }


        private string GetFilename(MHGameWork.TheWizards.WorldDatabase.DataItemIdentifier item, MHGameWork.TheWizards.WorldDatabase.DataRevisionIdentifier revision)
        {
            string dir = database.GetRevisionDataElementFolder(revision) + "\\Entities";
            System.IO.Directory.CreateDirectory(dir);
            return dir + "\\ObjectFullData" + item.Id.ToString() + ".xml"; 
        }

    }
}
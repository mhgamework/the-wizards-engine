﻿using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.TileEngine
{
    public class DiskTileDataFactory
    {
        private List<TileData> tileDataList = new List<TileData>();

        private readonly IMeshFactory meshFactory;
        private readonly ITileFaceTypeFactory tileFaceTypeFactory;

        public DiskTileDataFactory(IMeshFactory meshFactory, ITileFaceTypeFactory tileFaceTypeFactory)
        {
            this.meshFactory = meshFactory;
            this.tileFaceTypeFactory = tileFaceTypeFactory;
        }

        public DirectoryInfo SaveDir
        { get; set; }

        public TileData GetTileData(Guid guid)
        {
            var ret = tileDataList.Find(o => o.Guid.Equals(guid));
            if (ret == null)
            {
                if (SaveDir == null) return null;
                var fi = new FileInfo(getTileDataFilePath(guid));
                if (!fi.Exists) return null;

                using (var fs = fi.Open(FileMode.Open))
                {
                    ret = DeserializeTileData(fs);

                }
            }
            return ret;
        }

        public void AddTileData(TileData data)
        {
            tileDataList.Add(data);
        }

        public void SerializeTileData(TileData data, Stream stream)
        {

            var node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "TileData");
            var guide = node.CreateChildNode("Guid");
            XMLSerializer.WriteGuid(guide, data.Guid);
            var mesh = node.CreateChildNode("Mesh");
            XMLSerializer.WriteGuid(mesh, data.Mesh.Guid);

            var dimensions = node.CreateChildNode("Dimensions");
            XMLSerializer.WriteVector3(dimensions, data.Dimensions);

            var facetypes = node.CreateChildNode("Faces");
            for (int i = 0; i < data.FaceTypes.Length; i++)
            {
                if (data.FaceTypes[i] == null) continue;
                var faceNode = facetypes.CreateChildNode("Face");
                var face = (TileFace)(i + 1);
                faceNode.AddAttribute("Face", face.ToString());

                XMLSerializer.WriteGuid(faceNode.CreateChildNode("Type"), data.FaceTypes[i].GetRoot().Guid);
                XMLSerializer.WriteBoolean(faceNode.CreateChildNode("Winding"), data.GetTotalWinding(face));
            }

            node.XmlDocument.Save(stream);
        }

        public TileData DeserializeTileData(Stream stream)
        {
            var node = TWXmlNode.GetRootNodeFromStream(stream);
            var data = new TileData(XMLSerializer.ReadGuid(node.FindChildNode("Guid")))
                           {
                               Mesh = meshFactory.GetMesh(XMLSerializer.ReadGuid(node.FindChildNode("Mesh"))),
                               Dimensions = XMLSerializer.ReadVector3(node.FindChildNode("Dimensions"))
                           };

            var faces = node.FindChildNode("Faces").GetChildNodes();
            for (int i = 0; i < faces.Length; i++)
            {

                var cFace = faces[i];
                var face = (TileFace)Enum.Parse(typeof(TileFace), cFace.GetAttribute("Face"));
                data.SetFaceType(face, tileFaceTypeFactory.GetTileFaceType(XMLSerializer.ReadGuid(cFace.FindChildNode("Type"))));
                data.SetLocalWinding(face, XMLSerializer.ReadBoolean(cFace.FindChildNode("Winding")));
            }

            return data;
        }


        public List<TileData> getCopyOfTileDataList()
        {
            List<TileData> ret = new List<TileData>();
            ret.AddRange(tileDataList);

            return ret;
        }

        public void SaveAllAssets()
        {

            for (int i = 0; i < tileDataList.Count; i++)
            {
                TileData cData = tileDataList[i];

                FileStream stream = File.Open(getTileDataFilePath(cData.Guid), FileMode.Create);
                SerializeTileData(cData, stream);
                stream.Close();
            }
        }

        private string getTileDataFilePath(Guid guid)
        {
            return SaveDir.FullName + "\\TileData " + guid + ".xml";
        }

    }
}

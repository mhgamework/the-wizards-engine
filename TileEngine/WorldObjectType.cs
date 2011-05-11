using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObjectType
    {
        public Guid Guid;
        private readonly TileSnapInformationBuilder builder;

        public WorldObjectType(IMesh _mesh, Guid guid, TileSnapInformationBuilder builder)
        {
            mesh = _mesh;
            BoundingBox = CalculateBoundingBoxFromMesh(mesh);
            this.Guid = guid;
            this.builder = builder;
        }



        public SnapInformation SnapInformation { get { return builder.CreateFromTile(TileData); } set { } }
        private IMesh mesh;
        public IMesh Mesh
        {
            get { return mesh; }
            set { mesh = value; BoundingBox = CalculateBoundingBoxFromMesh(mesh); }
        }

        public TileData TileData { get; set; }
               
        public BoundingBox BoundingBox;

        
        public static BoundingBox CalculateBoundingBoxFromMesh(IMesh mesh)
        {
            BoundingBox box = new BoundingBox();
            
            for (int i = 0; i < mesh.GetCoreData().Parts.Count; i++)
            {
                var points = mesh.GetCoreData().Parts[i].MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                var bTemp = BoundingBox.CreateFromPoints(points);
                
                box = box.MergeWith(bTemp);
            }
            return box;
        }

    }
}

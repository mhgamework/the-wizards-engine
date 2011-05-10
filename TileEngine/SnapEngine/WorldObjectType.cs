using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObjectType
    {
        public Guid guid;

        public WorldObjectType(IMesh _mesh, Guid guid)
        {
            mesh = _mesh;
            BoundingBox = CalculateBoundingBoxFromMesh(mesh);
            this.guid = guid;
        }

        

        public SnapInformation SnapInformation;
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

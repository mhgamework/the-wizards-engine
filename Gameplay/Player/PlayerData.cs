using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// Functionality
    /// 
    /// The Look dir for angle (0,0) is (0,0,-1) = forward. The horizontal angle is around the Y-axis and the vertical around the right axis.
    /// </summary>
    [ModelObjectChanged]
    public class PlayerData : EngineModelObject
    {

        public PlayerData()
        {
            Entity = new Engine.WorldRendering.Entity();

            Entity.Mesh = GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));
        }
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                updateEntity();
            }
        }

        private Engine.WorldRendering.Entity entity;
        public Engine.WorldRendering.Entity Entity
        {
            get { return entity; }
            set
            {
                entity = value;
                updateEntity();
            }
        }

        public float Health;

        public float LookAngleHorizontal;
        public float LookAngleVertical;

        /// <summary>
        /// The Y coordinate which is the absolute minimum for the players position
        /// </summary>
        public float GroundHeight { get; set; }

        /// <summary>
        /// Disables gravity
        /// </summary>
        public bool DisableGravity { get; set; }
        



        public string Name;


        private void updateEntity()
        {
            if (entity == null) return;
            entity.WorldMatrix = Matrix.Translation(position);
        }


        public static string BarrelObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.obj"; } }
        public static string BarrelMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.mtl"; } }

        public static RAMMesh GetBarrelMesh(OBJToRAMMeshConverter c)
        {
            var fsMat = new FileStream(BarrelMtl, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Barrel01.mtl", fsMat);

            importer.ImportObjFile(BarrelObj);

            var meshes = c.CreateMeshesFromObjects(importer);

            fsMat.Close();

            return meshes[0];
        }

    }
}

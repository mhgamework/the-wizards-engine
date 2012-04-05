using System;
using System.IO;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tiling
{
    /// <summary>
    /// This simulator will process user input and allow the player to place/manipulate tiles.
    /// </summary>
    public class TileEditorSimulator : ISimulator
    {
        private IMesh[] meshes;

        private WorldRendering.Entity ghostEntity;
        private TileRotation ghostRotation;

        public TileEditorSimulator()
        {
            meshes = new IMesh[2];
            meshes[0] = null;
            meshes[1] = GetBarrelMesh(new OBJToRAMMeshConverter(new RAMTextureFactory()));

            ghostEntity = new WorldRendering.Entity();

        }

        public static string BarrelObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\TileSet\ts001sg001.obj"; } }
        public static string BarrelMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\TileSet\ts001sg001.mtl"; } }

        public static RAMMesh GetBarrelMesh(OBJToRAMMeshConverter c)
        {
            var fsMat = new FileStream(BarrelMtl, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("ts001sg001.mtl", fsMat);

            importer.ImportObjFile(BarrelObj);

            var meshes = c.CreateMeshesFromObjects(importer);

            fsMat.Close();

            return meshes[0];
        }

        public void Simulate()
        {
            var cursorTilePosition = getCursorTilePosition();
            if (cursorTilePosition.HasValue)
            {
                ghostEntity.WorldMatrix = TiledEntity.CreateEntityWorldMatrix(ghostRotation,
                                                                              cursorTilePosition.Value);
            }

            if (TW.Game.Keyboard.IsKeyPressed(Key.D1))
            {
                setCurrentMesh(meshes[0]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D2))
            {
                setCurrentMesh(meshes[1]);
            }
            if (TW.Game.Mouse.LeftMouseJustPressed)
            {
                ghostRotation = (TileRotation)(((int)ghostRotation + 1) % 4);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.F))
            {
                if (cursorTilePosition.HasValue)
                {
                    new TiledEntity
                        {
                            Position = cursorTilePosition.Value,
                            Rotation = ghostRotation,
                            Mesh = ghostEntity.Mesh
                        };
                }

            }


        }

        private Point3? getCursorTilePosition()
        {
            var ray = TW.Model.GetSingleton<CameraInfo>().GetCenterScreenRay();

            var plane = new Plane(Vector3.UnitY, 0);

            var dist = ray.xna().Intersects(plane.xna());
            if (!dist.HasValue) return null;

            var pos = ray.Position + ray.Direction * dist.Value;

            var ret = new Point3();
            ret.X = (int)Math.Round(pos.X / TiledEntity.TileSize.X);
            ret.Y = (int)Math.Round(pos.Y / TiledEntity.TileSize.Y);
            ret.Z = (int)Math.Round(pos.Z / TiledEntity.TileSize.Z);

            return ret;
        }

        private void setCurrentMesh(IMesh mesh)
        {
            ghostEntity.Mesh = mesh;
        }
    }
}

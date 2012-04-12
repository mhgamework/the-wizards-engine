using System;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;
using System.Linq;

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

        private BoundingBox tileBounding = new BoundingBox(new Vector3(-1.5f, -2, -1.5f), new Vector3(1.5f, 2f, 1.5f));
        private TileRotation matchedGhostRotation;

        public TileEditorSimulator()
        {
            meshes = new IMesh[8];
            meshes[0] = null;
            meshes[1] = MeshFactory.Load("Core\\TileSet\\ts001sg001");
            meshes[2] = MeshFactory.Load("Core\\TileSet\\ts001icg001");
            meshes[3] = MeshFactory.Load("Core\\TileSet\\ts001ocg001");
            meshes[4] = MeshFactory.Load("Core\\TileSet\\ts001g001");

            ghostEntity = new WorldRendering.Entity();

        }
        public void Simulate()
        {
            var cursorTilePosition = getCursorTilePosition();
            if (canPlaceTile())
            {
                if (ghostEntity.Mesh != null)
                    matchedGhostRotation = findFittingRotation(ghostEntity.Mesh, cursorTilePosition.Value, ghostRotation);

                ghostEntity.Visible = true;
                ghostEntity.WorldMatrix = TiledEntity.CreateEntityWorldMatrix(matchedGhostRotation,
                                                                              cursorTilePosition.Value);
            }
            else
            {
                ghostEntity.Visible = false;
            }

            if (TW.Game.Keyboard.IsKeyPressed(Key.D1))
            {
                setCurrentMesh(meshes[0]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D2))
            {
                setCurrentMesh(meshes[1]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D3))
            {
                setCurrentMesh(meshes[2]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D4))
            {
                setCurrentMesh(meshes[3]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D5))
            {
                setCurrentMesh(meshes[4]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D6))
            {
                setCurrentMesh(meshes[5]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D7))
            {
                setCurrentMesh(meshes[6]);
            }
            if (TW.Game.Keyboard.IsKeyPressed(Key.D8))
            {
                setCurrentMesh(meshes[7]);
            }
            if (TW.Game.Mouse.RightMouseJustPressed)
            {
                ghostRotation = (TileRotation)(((int)ghostRotation + 1) % 4);
            }
            if (TW.Game.Mouse.LeftMouseJustPressed)
            {
                actionDo(cursorTilePosition);


            }


        }

        /// <summary>
        /// Returns a rotation for which the given tile fits at the given location
        /// Returns rotation when no match
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="ghostPos"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private TileRotation findFittingRotation(IMesh mesh, Point3 ghostPos, TileRotation rotation)
        {

            for (int i = 0; i < 4; i++)
            {
                if (checkTileFits(mesh, ghostPos, rotation)) return rotation;
                rotation.Rotate(TileRotation.Rotation90);
            }

            return rotation;



        }

        private bool checkTileFits(IMesh mesh, Point3 tilePos, TileRotation rotation)
        {
            var dirs = new[] { new Point3(1, 0, 0), new Point3(-1, 0, 0), new Point3(0, 0, 1), new Point3(0, 0, -1) };
            foreach (var dir in dirs)
            {
                var tile = TW.Model.GetSingleton<TileModel>().GetTileAt(tilePos + dir);
                var face = Enum.GetValues(typeof(TileFace)).Cast<TileFace>().First(f => f.Normal() == dir.ToVector3());

                var bounding1 = TileBoundary.CreateFromMesh(mesh, tileBounding, face);
                var bounding2 = TileBoundary.CreateFromMesh(tile.Mesh, tileBounding, face.GetOpposing());

                if (!bounding1.Matches(bounding2, new TileBoundaryWinding { Rotation = TileRotation.Rotation0, Mirror = true }))
                    return false;


            }
            return true;
        }


        private void actionDo(Point3? cursorTilePosition)
        {
            if (ghostEntity.Mesh == null)
            {
                // Remove tile
                if (!cursorTilePosition.HasValue) return;
                var tile = TW.Model.GetSingleton<TileModel>().GetTileAt(cursorTilePosition.Value);
                if (tile != null)
                    tile.Delete();
            }
            else
            {
                // Place new tile
                if (!canPlaceTile()) return;
                new TiledEntity
                    {
                        Position = cursorTilePosition.Value,
                        Rotation = matchedGhostRotation,
                        Mesh = ghostEntity.Mesh
                    };
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
            //ret.X = (int)Math.Round(pos.X / TiledEntity.TileSize.X);
            //ret.Y = (int)Math.Round(pos.Y / TiledEntity.TileSize.Y);
            //ret.Z = (int)Math.Round(pos.Z / TiledEntity.TileSize.Z);
            ret.X = (int)Math.Floor(pos.X / TiledEntity.TileSize.X);
            ret.Y = 0;
            ret.Z = (int)Math.Floor(pos.Z / TiledEntity.TileSize.Z);

            return ret;
        }

        private bool canPlaceTile()
        {
            var pos = getCursorTilePosition();
            if (!pos.HasValue) return false;
            return canPlaceTileAt(pos.Value);
        }
        private bool canPlaceTileAt(Point3 p)
        {
            return TW.Model.GetSingleton<TileModel>().GetTileAt(p) == null;
        }

        private void setCurrentMesh(IMesh mesh)
        {
            ghostEntity.Mesh = mesh;
        }
    }
}

using System;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tiling;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Simulators
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
                ghostEntity.Visible = true;
                ghostEntity.WorldMatrix = TiledEntity.CreateEntityWorldMatrix(ghostRotation,
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
                        Rotation = ghostRotation,
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

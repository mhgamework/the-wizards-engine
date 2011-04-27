using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObjectPlaceTool : IXNAObject
    {
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (!enabled)
                {
                    ghost.WorldMatrix = new Matrix();
                    return;
                }

                objectsPlacedSinceEnabled = 0;
                if (PlaceType == null)
                    throw new InvalidOperationException("No PlaceType has been set");
                if (ghost != null)
                    ghost.WorldMatrix = new Matrix();

                ghost = renderer.AddMesh(PlaceType.Mesh);
            }
        }

        public WorldObjectType PlaceType { get; set; }

        private IXNAGame game;
        World world;
        private SimpleMeshRenderer renderer;
        private WorldObjectFactory factory;




        private SimpleMeshRenderElement ghost;
        private bool isGhostActive;


        private Snapper snapper = new Snapper();
        private TileSnapInformationBuilder builder;

        private List<ISnappableWorldTarget> snapTargetList;
        private List<Transformation> transformations = new List<Transformation>();
        private int objectsPlacedSinceEnabled;

        public WorldObjectPlaceTool(IXNAGame _game, World _world, SimpleMeshRenderer _renderer, TileSnapInformationBuilder _builder)
        {
            game = _game;
            world = _world;
            renderer = _renderer;
            factory = new WorldObjectFactory(world);
            snapTargetList = world.SnapTargetList;
            snapper.addSnapper(new SnapperPointPoint());
            builder = _builder;
        }


        public void PlaceWorldObjectAtPosition(WorldObjectType type, Vector3 position)
        {
            WorldObject worldObject = factory.CreateNewWorldObject(game, type, renderer);
            worldObject.Position = position;
        }



        #region IXNAObject Members

        public void Initialize(IXNAGame game)
        {
        }

        public void Render(IXNAGame game)
        {
            if (!Enabled) return;

        }

        public void Update(IXNAGame game)
        {
            if (!Enabled) return;
            if (!game.Mouse.CursorEnabled) return;

            updateGhostPosition();
            if (game.Mouse.LeftMouseJustPressed)
                placeNewObjectAtGhost();


        }

        private void placeNewObjectAtGhost()
        {
            var obj = factory.CreateNewWorldObject(game, PlaceType, renderer);
            Vector3 scale, translation;
            Quaternion rotation;
            ghost.WorldMatrix.Decompose(out scale, out rotation, out translation);
            obj.Position = translation;
            obj.Rotation = rotation;
            objectsPlacedSinceEnabled++;
        }

        private void updateGhostPosition()
        {
            Transformation transformation;
            if (canSnapGhost(out transformation))
            {
                ghost.WorldMatrix = transformation.CreateMatrix();
            }
            else
            {
                // Place ghost at groundplane
                var raycastPosition = raycastGroundPlaneCursor();
                ghost.WorldMatrix = Matrix.CreateTranslation(raycastPosition);
            }
        }

       
        private bool canSnapGhost(out Transformation transformation)
        {
            transformations = snapper.SnapTo(builder.CreateFromTile(PlaceType.TileData), snapTargetList);

            if (transformations.Count > 0)
                transformation = transformations[0];
            else
                transformation = new Transformation();

            return transformations.Count > 0;
        }

        private Vector3 raycastGroundPlaneCursor()
        {
            Ray ray = game.GetWereldViewRay(new Vector2(game.Mouse.CursorPosition.X, game.Mouse.CursorPosition.Y));
            Vector3 pos = world.Raycast(ray);
            return pos;
        }



        #endregion

        public int ObjectsPlacedSinceEnabled()
        {
            return objectsPlacedSinceEnabled;
        }
    }
}

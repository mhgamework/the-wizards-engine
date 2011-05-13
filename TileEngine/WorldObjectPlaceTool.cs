﻿using System;
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
                if (enabled == value) return;


                enabled = value;

                updateGhost();
                if (!enabled) return;

                objectsPlacedSinceEnabled = 0;


            }
        }

        private void updateGhost()
        {
            if (ghost != null)
                ghost.Delete();
            ghost = null;
            if (enabled == false) return;
            if (PlaceType != null)
                ghost = renderer.AddMesh(PlaceType.Mesh);
        }

        private WorldObjectType placeType;
        public WorldObjectType PlaceType
        {
            get { return placeType; }
            set
            {
                if (placeType == value) return;
                placeType = value;
                updateGhost();
            }
        }

        public World World
        {
            get { return world; }
            set { world = value; }
        }

        private IXNAGame game;
        private World world;
        private SimpleMeshRenderer renderer;


        private WorldTileSnapper worldTileSnapper;

        private SimpleMeshRenderElement ghost;
        private bool isGhostActive;


        private TileSnapInformationBuilder builder;
        private readonly IMeshFactory meshFactory;
        private readonly ITileFaceTypeFactory tileFaceTypeFactory;

        private List<Transformation> transformations = new List<Transformation>();
        private int objectsPlacedSinceEnabled;

        public WorldObjectPlaceTool(IXNAGame _game, World _world, SimpleMeshRenderer _renderer, TileSnapInformationBuilder _builder, IMeshFactory meshFactory, ITileFaceTypeFactory tileFaceTypeFactory)
        {
            game = _game;
            World = _world;
            renderer = _renderer;
            builder = _builder;
            this.meshFactory = meshFactory;
            this.tileFaceTypeFactory = tileFaceTypeFactory;
            worldTileSnapper = new WorldTileSnapper(builder);
        }


        public void PlaceWorldObjectAtPosition(WorldObjectType type, Vector3 position)
        {
            WorldObject worldObject = World.CreateNewWorldObject(game, type, renderer);
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
            if (PlaceType == null) return;
            if (!game.Mouse.CursorEnabled) return;

            updateGhostPosition();
            if (game.Mouse.LeftMouseJustPressed)
                placeNewObjectAtGhost();


        }

        private void placeNewObjectAtGhost()
        {
            var obj = World.CreateNewWorldObject(game, PlaceType, renderer);
            Vector3 scale, translation;
            Quaternion rotation;
            (Matrix.Invert(PlaceType.TileData.MeshOffset) * ghost.WorldMatrix).Decompose(out scale, out rotation, out translation);
            obj.Position = translation;
            obj.Rotation = rotation;
            objectsPlacedSinceEnabled++;
        }

        private void updateGhostPosition()
        {
            var raycastPosition = raycastGroundPlaneCursor();

            Transformation transformation = worldTileSnapper.CalculateSnap(PlaceType.TileData,
                                                                           new Transformation(raycastPosition),
                                                                           World.WorldObjectList);

            ghost.WorldMatrix = transformation.CreateMatrix();
        }




        private Vector3 raycastGroundPlaneCursor()
        {
            Ray ray = game.GetWereldViewRay(new Vector2(game.Mouse.CursorPosition.X, game.Mouse.CursorPosition.Y));
            Vector3 pos = World.Raycast(ray);
            return pos;
        }



        #endregion

        public int ObjectsPlacedSinceEnabled()
        {
            return objectsPlacedSinceEnabled;
        }
    }
}

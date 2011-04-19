using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.Scene.Editor
{
    public class SceneEditor : IXNAObject
    {

        public IMesh PlaceModeMesh;
        private EditorCamera editorCamera;
        public EditorCamera EditorCamera
        {
            get { return editorCamera; }
            private set { editorCamera = value; }
        }

        private Entity selectedEntity;
        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set { selectedEntity = value; }
        }

        public void EnableSelectMod()
        {
            activeMode = Mode.Select;
        }
        public void EnablePlaceEntityMode()
        {
            if (PlaceModeMesh == null)
                throw new InvalidOperationException("PlaceModeMesh must be set!");

            activeMode = Mode.Place;
        }


        public void SetEditingScene(Scene scene)
        {
            this.scene = scene;
        }


        public SceneEditor()
        {
            EditorCamera = new EditorCamera();

        }


        private Scene scene;

        private Mode activeMode;
        private IXNAGame game;


        public void Initialize(IXNAGame _game)
        {
            game = _game;
            EditorCamera.Enabled = true;
            game.AddXNAObject(EditorCamera);
            game.SetCamera(EditorCamera);

            var grid = new EditorGrid();
            game.AddXNAObject(grid);

        }

        public void Render(IXNAGame _game)
        {
            if (SelectedEntity != null)
            {
                game.LineManager3D.AddBox(SelectedEntity.BoundingBox, Color.Black);
            }
        }

        public void Update(IXNAGame _game)
        {
            EditorCamera.UpdateCameraMoveModeDefaultControls();

            if (EditorCamera.ActiveMoveMode == EditorCamera.MoveMode.None)
                updateModes();
        }

        private void updateModes()
        {
            if (activeMode == Mode.Place)
            {
                if (game.Mouse.LeftMouseJustPressed)
                {
                    tryPlaceEntity();
                }
            }
            else if (activeMode == Mode.Select)
            {
                if (game.Mouse.LeftMouseJustPressed)
                {
                    trySelectEntity();
                }
            }
        }

        private void trySelectEntity()
        {
            var ent = scene.RaycastScene(game.GetWereldViewRay(game.Mouse.CursorPositionVector));

            SelectedEntity = ent;
        }


        private void tryPlaceEntity()
        {
            var ray = game.GetWereldViewRay(game.Mouse.CursorPositionVector);

            var groundPlane = new Plane(Vector3.Up, 0);

            var dist = ray.Intersects(groundPlane);
            if (!dist.HasValue) return;

            placeEntityAt(ray.Position + ray.Direction * dist.Value);



        }

        private void placeEntityAt(Vector3 position)
        {
            var ent = new Entity(scene);
            ent.Visible = true;
            ent.Solid = true;
            ent.Mesh = PlaceModeMesh;
            ent.Transformation = new Transformation(Vector3.One, Quaternion.Identity, position);

            scene.AddEntity(ent);

        }


        public void ChangeMode(Mode newMode)
        {
            if (this.activeMode == newMode) return;



            this.activeMode = newMode;
        }

        public enum Mode
        {
            None = 0,
            Select,
            Place
        }
    }
}

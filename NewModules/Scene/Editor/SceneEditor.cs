using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.Scene.Editor
{
    public class SceneEditor : IXNAObject
    {

        public IMesh PlaceModeMesh;
        private EditorCamera editorCamera;

        public void EnablePlaceEntityMode()
        {
            activeMode = Mode.Place;
        }


        public void SetEditingScene(Scene scene)
        {
            this.scene = scene;
        }


        public SceneEditor()
        {
        }


        private Scene scene;

        private Mode activeMode;
        private IXNAGame game;


        public void Initialize(IXNAGame _game)
        {
            game = _game;
            editorCamera = new EditorCamera();
            editorCamera.Enabled = true;
            game.AddXNAObject(editorCamera);
            game.SetCamera(editorCamera);

        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
            if (activeMode == Mode.Place)
            {
                if (game.Mouse.LeftMouseJustPressed)
                {
                    TryPlaceEntity();
                }
            }
        }

        private void TryPlaceEntity()
        {
            var ray = game.GetWereldViewRay(game.Mouse.CursorPositionVector);

            var groundPlane = new Plane(Vector3.Up, 0);

            var dist = ray.Intersects(groundPlane);
            if (!dist.HasValue) return;

            placeEntityAt(ray.Position + ray.Direction * dist.Value);



        }

        private void placeEntityAt(Vector3 position)
        {
            var ent = new Entity();
            ent.Visible = true;
            ent.Solid = true;
            ent.Mesh = PlaceModeMesh;
            ent.Transformation = new Transformation(Vector3.One, Quaternion.Identity, position);

            scene.AddEntity(ent);
        }


        public enum Mode
        {
            None = 0,
            Place
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldSceneLoader
    {
        private readonly World world;
        private readonly Scene.Scene scene;

        public WorldSceneLoader(World world, Scene.Scene scene)
        {
            this.world = world;
            this.scene = scene;
        }

        public void LoadIntoScene()
        {
            foreach (var obj in world.WorldObjectList)
            {
                var ent = scene.CreateEntity();
                ent.Mesh = obj.ObjectType.TileData.Mesh;
                ent.Transformation = new Graphics.Transformation(Vector3.One, obj.Rotation, obj.Position);
                ent.Solid = true;
            }
        }
    }
}

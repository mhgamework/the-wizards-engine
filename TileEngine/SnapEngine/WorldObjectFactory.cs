using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObjectFactory
    {
        private World world;

        public WorldObjectFactory(World _world)
        {
            world = _world;
        }

        public WorldObject CreateNewWorldObject(IXNAGame game, WorldObjectType objectType, SimpleMeshRenderer renderer)
        {
            var worldObject = new WorldObject(game, objectType, renderer);
            addWorldObject(worldObject);
            return worldObject;
        }

        private void addWorldObject(WorldObject worldObject)
        {
            world.WorldObjectList.Add(worldObject);
        }

        public WorldObject CloneWorldObject(WorldObject worldObject)
        {
            throw new NotImplementedException();
            //WorldObject clone = worldObject.Clone();
            //addWorldObject(clone);
            //return clone;
        }



    }
}

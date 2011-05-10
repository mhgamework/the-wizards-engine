using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObjectFactory
    {
        private World world;
        private readonly IMeshFactory meshFactory;
        private readonly ITileFaceTypeFactory tileFaceTypeFactory;

        public WorldObjectFactory(World _world, IMeshFactory meshFactory, ITileFaceTypeFactory tileFaceTypeFactory)
        {
            world = _world;
            this.meshFactory = meshFactory;
            this.tileFaceTypeFactory = tileFaceTypeFactory;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class World
    {
        
        public List<WorldObject> WorldObjectList = new List<WorldObject>();

        public WorldObject Raycast(Ray ray, List<WorldObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var obj = list[i];

                var inverseWorld = Matrix.Invert(obj.WorldMatrix);

                var start = ray.Position;
                var end = start + ray.Direction;

                start = Vector3.Transform(start, inverseWorld);
                end = Vector3.Transform(end, inverseWorld);

                var localRay = new Ray(start, Vector3.Normalize(end - start));

                var currentBB = list[i].ObjectType.BoundingBox;
                if (currentBB.Intersects(localRay).HasValue)
                {
                    return list[i];
                }
            }
            return null;
        }

        public Vector3 Raycast(Ray ray)
        {
            Plane ground = new Plane(new Vector3(0, 1, 0), 0);
            if(ray.Intersects(ground).HasValue)
            {
                Vector3 raydir = ray.Direction * ray.Intersects(ground).Value;
                Vector3 pos = ray.Position + raydir;
                return pos;
            }
            return new Vector3 (0,0,0);
        }

        public void DeleteWorldObject(WorldObject activeWorldObject)
        {            
            activeWorldObject.IsDeleted = true;
            WorldObjectList.Remove(activeWorldObject);
        }

        public void AddWorldObject(WorldObject worldObject)
        {
            WorldObjectList.Add(worldObject);
        }

        public WorldObject CreateNewWorldObject(IXNAGame game, WorldObjectType objectType, SimpleMeshRenderer renderer)
        {
            var worldObject = new WorldObject(game, objectType, renderer);
            AddWorldObject(worldObject);
            return worldObject;
        }

    }
}

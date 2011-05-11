using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldTileSnapper
    {
        private readonly TileSnapInformationBuilder builder;

        public WorldTileSnapper(TileSnapInformationBuilder builder)
        {
            this.builder = builder;
            snapper = new Snapper();
            snapper.AddSnapper(new SnapperPointPoint());
        }

        private Transformation currentTransformation;
        private Snapper snapper;

        public Transformation CalculateSnap(TileData data, Transformation currentTransformation, List<WorldObject> snaptargetList)
        {

            this.currentTransformation = currentTransformation;

            var temp = new List<ISnappableWorldTarget>();
            temp.AddRange(snaptargetList.Select(o => (ISnappableWorldTarget) o));
            

            var transformations = snapper.SnapTo(builder.CreateFromTile(data), temp);
            transformations.Sort(compareTransformations);

            var meBB = new BoundingBox(data.Dimensions * 0.95f * 0.5f, data.Dimensions * 0.95f * 0.5f);

            Transformation transformation;
            for (int i = 0; i < transformations.Count; i++)
            {
                transformation = transformations[i];
                var intersects = false;
                for (int j = 0; j < snaptargetList.Count; j++)
                {
                    var obj = snaptargetList[j];
                    //if (obj == selectedWorldObject) continue;

                    var objBB = new BoundingBox(-obj.ObjectType.TileData.Dimensions * 0.5f, obj.ObjectType.TileData.Dimensions * 0.5f);

                    if (objBB.Transform(obj.WorldMatrix)
                        .Contains(
                        meBB.Transform(transformation.CreateMatrix())) == ContainmentType.Disjoint)
                        continue;

                    intersects = true;
                    break;
                }

                if (intersects)
                    continue;

                var maxDist = 10;
                var distSq = (transformation.Translation - currentTransformation.Translation).LengthSquared();
                if (distSq > maxDist * maxDist)
                {
                    return currentTransformation;
                }
                return transformation;


            }

            return currentTransformation;
        }

        private int compareTransformations(Transformation a, Transformation b)
        {
            var valueA = calculateTransformationQuality(a);
            var valueB = calculateTransformationQuality(b);
            return (int)(valueA - valueB);
        }

        private float calculateTransformationQuality(Transformation a)
        {
            float ret = (a.Translation - currentTransformation.Translation).LengthSquared() * 100;

            var vectorA = Vector3.Transform(Vector3.UnitX, currentTransformation.Rotation);
            var vectorB = Vector3.Transform(Vector3.UnitX, a.Rotation);

            ret += Vector3.Dot(vectorA, vectorB) * 10;

            return ret;
        }
    
        
    }
}

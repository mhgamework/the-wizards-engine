using System;
using System.Collections.Generic;
using DirectX11;
using SlimDX;
using System.Linq;
namespace MHGameWork.TheWizards.RTSTestCase1.Shaping
{
    /// <summary>
    /// Telling the possible construction that can be made (partially) out of the given set of blocks.
    /// </summary>
    public class ShapeDetector
    {
        public OrientedShape FindConstruction(Dictionary<Point3, IShapeElement> blocks)
        {
            Matrix matrix;
            IShape retShape;
            
            var key = GetBestUniqueShapeKey(blocks, out matrix);
            return !keys.TryGetValue(key, out retShape) ? new OrientedShape() : new OrientedShape() { Rotation = matrix, Shape = retShape };
        
        }
        public List<OrientedShape> FindPossibilities(Dictionary<Point3, IShapeElement> blocks)
        {
            throw new NotImplementedException();
        }



        private Dictionary<IShape, Dictionary<Point3, IShapeElement>> constructions = new Dictionary<IShape, Dictionary<Point3, IShapeElement>>();
        private Dictionary<long, IShape> keys = new Dictionary<long, IShape>();
        //private Dictionary<Point3, IShapeElement> rotatePositions
        public void AddConstruction(IShape shape, Dictionary<Point3, IShapeElement> blocks)
        {   
            constructions.Add(shape,blocks);
            Matrix rot;
            keys.Add(GetBestUniqueShapeKey(blocks, out rot),shape);
        }



        private long GetBestUniqueShapeKey(Dictionary<Point3, IShapeElement> blocks, out Matrix rotation)
        {
            var normTransBlocks = normalizeTranslation(blocks);
            var rotations = getRotations(normTransBlocks);
            var bestShape = rotations.OrderBy(o => GetUniqueShapeKey(o.blocks)).FirstOrDefault();
            rotation = bestShape.rotation;
            return GetUniqueShapeKey(bestShape.blocks);
        }
        private long GetUniqueShapeKey(Dictionary<Point3, IShapeElement> blocks)
        {
            Func<long, KeyValuePair<Point3, IShapeElement>, long> calculateHash;
            calculateHash = (current, pair) => current + pair.Key.GetHashCode() * pair.Value.GetHashCode();
            return blocks.Aggregate<KeyValuePair<Point3, IShapeElement>, long>(0, calculateHash);
        }

        private List<RotatedBlocks> getRotations(Dictionary<Point3, IShapeElement> blocks)
        {
            var rotations = new List<RotatedBlocks>();
          var currentBlock = rotateAndAddBlocks(new RotatedBlocks(){blocks = blocks,rotation =  Matrix.RotationX(0)}, rotations,Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock,rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationZ(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationY(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            currentBlock = rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            rotateAndAddBlocks(currentBlock, rotations, Matrix.RotationX(MathHelper.PiOver2));
            return rotations;
        }

        private RotatedBlocks rotateAndAddBlocks(RotatedBlocks roBlocks, List<RotatedBlocks> rotations, Matrix rotation)
        {
            var addBlocks = new Dictionary<Point3, IShapeElement>();
            foreach (var pair in roBlocks.blocks)
            {
                var point = Vector3.TransformCoordinate(pair.Key.ToVector3(), rotation).ToPoint3Rounded();
                addBlocks.Add(point, pair.Value);
            }
            addBlocks = normalizeTranslation(addBlocks);
            var resultRotBlocks = new RotatedBlocks()
                                      {rotation = Matrix.Multiply(rotation, roBlocks.rotation), blocks = addBlocks};
            rotations.Add(resultRotBlocks);
            return resultRotBlocks;

        }

        private Dictionary<Point3, IShapeElement> normalizeTranslation(Dictionary<Point3, IShapeElement> blocks)
        {
            var minimum = new Point3(int.MaxValue, int.MaxValue, int.MaxValue);
            foreach (var point in blocks.Keys)
            {
                minimum.X = Math.Min(point.X, minimum.X);
                minimum.Y = Math.Min(point.Y, minimum.Y);
                minimum.Z = Math.Min(point.Z, minimum.Z);
            }
            return blocks.ToDictionary(shapeElement => (shapeElement.Key.ToVector3() - minimum.ToVector3()).ToPoint3Rounded(), shapeElement => shapeElement.Value);
        }

        public struct OrientedShape
        {
            public Matrix Rotation;
            public IShape Shape;
        }

        struct RotatedBlocks
        {
            public Matrix rotation; //Dit is de rotatie ten opzichte van het origineel
            public Dictionary<Point3, IShapeElement> blocks;
        }
    }
}
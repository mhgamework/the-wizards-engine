using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Shaping
{
    public interface IBlockHasher
    {
        long GetHashOf(Dictionary<Point3, IShapeElement> blocks);

    }
    
    //Gives a hash for a bunch of blocks, does not depend on rotation or translation of the blocks.
    public interface IBlockMovementHasher : IBlockHasher
    {
    }

    
    public class MovementNormalHasher : IBlockMovementHasher
    {
        private IBlockHasher translationHasher;
        private IBlocksRotationGenerator rotationGenerator;
        public MovementNormalHasher(IBlockTranslationHasher translationHasher, IBlocksRotationGenerator rotationGenerator)
        {
            this.translationHasher = translationHasher;
            this.rotationGenerator = rotationGenerator;
        }

        public long GetHashOf(Dictionary<Point3, IShapeElement> blocks)
        {
            return rotationGenerator.GetRotations(blocks).Select(rotation => translationHasher.GetHashOf(rotation.Item1)).Concat(new[] {long.MaxValue}).Min();
        }
    }
    
    
    public interface IBlockTranslationHasher : IBlockHasher
    {
    }

    public class TranslationNormalHasher:IBlockTranslationHasher
    {
        private IBlockHasher hasher;
        private IBlockTranslationNormalizer normalizer;

        public TranslationNormalHasher(IBlockBasicHasher hasher, IBlockTranslationNormalizer normalizer)
        {
            this.hasher = hasher;
            this.normalizer = normalizer;
        }

        public long GetHashOf(Dictionary<Point3, IShapeElement> blocks)
        {
          return hasher.GetHashOf(normalizer.CreateTranslationNormalisation(blocks));
        }
    }


    public interface IBlockBasicHasher : IBlockHasher
    {
    }
    
    public class SimpleBlockHasher:IBlockBasicHasher
    {
        public long GetHashOf(Dictionary<Point3, IShapeElement> blocks)
        {
            Func<long, KeyValuePair<Point3, IShapeElement>, long> calculateHash = (current, pair) => current + pair.Key.GetHashCode() * pair.Value.GetHashCode();
            return blocks.Aggregate(0, calculateHash);
        }
    }

    public interface IBlockTranslationNormalizer
    {
        Dictionary<Point3, IShapeElement> CreateTranslationNormalisation(Dictionary<Point3, IShapeElement> blocks);
    }

    public class BlockTranslationNormalizer : IBlockTranslationNormalizer
    {
        public Dictionary<Point3, IShapeElement> CreateTranslationNormalisation(Dictionary<Point3, IShapeElement> blocks)
        {
            var minimum = FindMinimumBlockPosition(blocks);
            return blocks.ToDictionary(shapeElement => (shapeElement.Key.ToVector3() - minimum.ToVector3()).ToPoint3Rounded(), shapeElement => shapeElement.Value);
        }
        private Point3 FindMinimumBlockPosition(Dictionary<Point3, IShapeElement> blocks)
        {
            var minimum = new Point3(int.MaxValue, int.MaxValue, int.MaxValue);
            foreach (var point in blocks.Keys)
            {
                minimum.X = Math.Min(point.X, minimum.X);
                minimum.Y = Math.Min(point.Y, minimum.Y);
                minimum.Z = Math.Min(point.Z, minimum.Z);
            }
            return minimum;
        }
    }
   
    public interface IBlocksRotationGenerator
    {
        IEnumerable<Tuple<Dictionary<Point3, IShapeElement>,Matrix>> GetRotations(Dictionary<Point3, IShapeElement> blocks);
    }

    public class BlocksRotationGenerator : IBlocksRotationGenerator
    {
        private Dictionary<Point3, IShapeElement> currentBlocks;
        private Matrix rotation;
        public IEnumerable<Tuple<Dictionary<Point3, IShapeElement>,Matrix>> GetRotations(Dictionary<Point3, IShapeElement> blocks)
        {
            currentBlocks = blocks;
            rotation = Matrix.RotationX(0);
            yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(blocks,Matrix.RotationX(0));
            foreach (var shapeElement in getTripleRotation(Matrix.RotationX(MathHelper.PiOver2)))
                yield return shapeElement;
            doRotationOnBlocks(Matrix.RotationZ(MathHelper.PiOver2));
            yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(currentBlocks, rotation);
            foreach (var shapeElement in getTripleRotation(Matrix.RotationY(MathHelper.PiOver2)))
                yield return shapeElement;
            doRotationOnBlocks(Matrix.RotationX(MathHelper.PiOver2));
            yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(currentBlocks, rotation);

            foreach (var shapeElement in getTripleRotation(Matrix.RotationZ(MathHelper.PiOver2)))
                yield return shapeElement;
            doRotationOnBlocks(Matrix.RotationX(MathHelper.PiOver2));
            yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(currentBlocks, rotation);

            foreach (var shapeElement in getTripleRotation(Matrix.RotationY(MathHelper.PiOver2)))
                yield return shapeElement;
            doRotationOnBlocks(Matrix.RotationX(MathHelper.PiOver2));
            yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(currentBlocks, rotation);

            foreach (var shapeElement in getTripleRotation(Matrix.RotationZ(MathHelper.PiOver2)))
                yield return shapeElement;
            doRotationOnBlocks(Matrix.RotationY(MathHelper.PiOver2));
            yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(currentBlocks, rotation);

            foreach (var shapeElement in getTripleRotation(Matrix.RotationX(MathHelper.PiOver2)))
                yield return shapeElement;
        }

        private IEnumerable<Tuple<Dictionary<Point3, IShapeElement>,Matrix>> getTripleRotation(Matrix rotationMatrix)
        {
            for (int i = 0; i < 3; i++)
            {
                var retBlocks = doRotationOnBlocks(rotationMatrix);
                yield return new Tuple<Dictionary<Point3, IShapeElement>, Matrix>(retBlocks, rotation);
                
            }
        }


        private Dictionary<Point3, IShapeElement> doRotationOnBlocks(Matrix rotationMatrix)
        {
            var retDictionairy = currentBlocks.ToDictionary(currentBlock => Vector3.TransformCoordinate(currentBlock.Key.ToVector3(), rotationMatrix).ToPoint3Rounded(), currentBlock => currentBlock.Value);
            currentBlocks = retDictionairy;
            rotation = rotation*rotationMatrix;
            return currentBlocks;
        }
    }
}
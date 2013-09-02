using System.Collections.Generic;
using DirectX11;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Shaping
{
    public interface IShapeDetector
    {
        void AddBlueprint(IShape shape, Dictionary<Point3, IShapeElement> blocks);
        Dictionary<Point3, IShapeElement> GetBlocksOfShape(OrientedShape shape);
        IEnumerable<OrientedShape> GetConstruction(Dictionary<Point3, IShapeElement> blocks);
    }

    public class SimpleShapeDetector : IShapeDetector
    {
        private Dictionary<IShape, Dictionary<Point3, IShapeElement>> bluePrints = new Dictionary<IShape, Dictionary<Point3, IShapeElement>>();
        private BlockBuilder blockBuilder;
        private ShapeBuilder shapeBuilder;
        public SimpleShapeDetector(BlockBuilder blockBuilder, ShapeBuilder shapeBuilder)
        {
            this.blockBuilder = blockBuilder;
            this.shapeBuilder = shapeBuilder;
        }

        public void AddBlueprint(IShape shape, Dictionary<Point3, IShapeElement> blocks)
        {
            bluePrints.Add(shape, blocks);
        }

        public Dictionary<Point3, IShapeElement> GetBlocksOfShape(OrientedShape shape)
        {
            return blockBuilder.GetBlocksFromShape(shape, bluePrints);
        }

        public IEnumerable<OrientedShape> GetConstruction(Dictionary<Point3, IShapeElement> blocks)
        {
            return shapeBuilder.GetShapesFromBlocks(blocks, bluePrints);
        }
    }

    public class ShapeBuilder
    {
        //todo Make this hasher have a memory to speed up things.
        private IBlockMovementHasher movementHasher;
        private IBlockTranslationHasher translationHasher;
        private IBlocksRotationGenerator rotationGenerator;
        private IBlockGroupTranslationCalculator blockGroupTranslationCalculator;
        public ShapeBuilder(IBlocksRotationGenerator rotationGenerator, IBlockMovementHasher movementHasher, IBlockTranslationHasher translationHasher, IBlockGroupTranslationCalculator blockGroupTranslationCalculator)
        {
            this.rotationGenerator = rotationGenerator;
            this.movementHasher = movementHasher;
            this.translationHasher = translationHasher;
            this.blockGroupTranslationCalculator = blockGroupTranslationCalculator;
        }

        public IEnumerable<OrientedShape> GetShapesFromBlocks(Dictionary<Point3, IShapeElement> blocks, Dictionary<IShape, Dictionary<Point3, IShapeElement>> bluePrints)
        {
            var myHash = movementHasher.GetHashOf(blocks);
            var recognizedBluePrint = bluePrints.FirstOrDefault(o => movementHasher.GetHashOf(o.Value).Equals(myHash));
            var recognizedShape = recognizedBluePrint.Key;
            if (recognizedShape == null)
                yield break;
            var goodRotations = rotationGenerator.GetRotations(recognizedBluePrint.Value).Where(
                o => translationHasher.GetHashOf(o.Item1).Equals(translationHasher.GetHashOf(blocks)));
            foreach (var goodRotation in goodRotations)
            {
                yield return new OrientedShape { Shape = recognizedShape, Rotation = goodRotation.Item2, Translation = blockGroupTranslationCalculator.CalculateDifference(blocks, goodRotation.Item1) };
            }
        }
    }

    public interface IBlockGroupTranslationCalculator
    {
        Point3 CalculateDifference(Dictionary<Point3, IShapeElement> blocks,
                                                   Dictionary<Point3, IShapeElement> blocks2);

    }

    public class BlockGroupTranslationCalculator : IBlockGroupTranslationCalculator
    {
        public Point3 CalculateDifference(Dictionary<Point3, IShapeElement> blocks, Dictionary<Point3, IShapeElement> blocks2)
        {
            var totalP = new Point3();
            totalP = blocks.Aggregate(totalP, (current, shapeElement) => current + shapeElement.Key);
            totalP = blocks2.Aggregate(totalP, (current, pair) => current - pair.Key);
            var translation = totalP.ToVector3() / blocks.Count();
            return translation.ToPoint3Rounded();
        }
    }

    public class BlockBuilder
    {
        public Dictionary<Point3, IShapeElement> GetBlocksFromShape(OrientedShape shape, Dictionary<IShape, Dictionary<Point3, IShapeElement>> bluePrints)
        {
            var goodBlocks = bluePrints.FirstOrDefault(o => o.Key.Equals(shape.Shape)).Value;
            //This should be a clean clone + transformation.
            return goodBlocks == null ? null : goodBlocks.ToDictionary(o => (Vector3.TransformCoordinate(o.Key.ToVector3(), shape.Rotation) + shape.Translation).ToPoint3Rounded(), o => o.Value);
        }
    }

    public class ShapeDetectorBuilder
    {
        //reuse for ioc config file
        public IShapeDetector BuildShapeDetector()
        {
            var blockTranslationCalculator = new BlockGroupTranslationCalculator();
            var rotationGenerator = new BlocksRotationGenerator();
            var blockTranslationNorm = new BlockTranslationNormalizer();
            var blockBasicHasher = new SimpleBlockHasher();
            var translationHasher = new TranslationNormalHasher(blockBasicHasher, blockTranslationNorm);
            var movementHasher = new MovementNormalHasher(translationHasher, rotationGenerator);
            var shapeBuilder = new ShapeBuilder(rotationGenerator, movementHasher, translationHasher,
                                                blockTranslationCalculator);
            var blockBuilder = new BlockBuilder();
            return new SimpleShapeDetector(blockBuilder, shapeBuilder);
        }
    }

    public class OrientedShape
    {
        public Vector3 Translation;
        public Matrix Rotation;
        public IShape Shape;
    }
}

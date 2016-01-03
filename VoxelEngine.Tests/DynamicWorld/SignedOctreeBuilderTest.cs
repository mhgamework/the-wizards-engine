using System;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests
{
    public class SignedOctreeBuilderTest
    {
        private SignedOctreeBuilder builder;
        private float smallCubeSize = 8;

        [SetUp]
        public void Setup()
        {
            builder = new SignedOctreeBuilder();
        }

        [Test]
        public void TestSignsToOctree_Signs_SmallCube()
        {
            var tree = CreateOctreeSmallCube( smallCubeSize );

            Assert.AreEqual(8, tree.Signs.Count(s => s == false));
            Assert.NotNull(tree.Children);
            foreach (var c in tree.Children)
                Assert.AreEqual(7, c.Signs.Count(s => s == false));
        }

        public SignedOctreeNode CreateOctreeSmallCube( float gridSize )
        {
            var signs = new Array3D<bool>( new global::DirectX11.Point3( 3, 3, 3 ) );
            signs[ new global::DirectX11.Point3( 1, 1, 1 ) ] = true;

            var tree = builder.GenerateCompactedTreeFromSigns( signs, gridSize );
            return tree;
        }

        [Test]
        public void TestCompact_AllFalse()
        {
            var signs = new Array3D<bool>(new global::DirectX11.Point3(3, 3, 3));

            var tree = builder.GenerateCompactedTreeFromSigns(signs, smallCubeSize);

            Assert.AreEqual(8, tree.Signs.Count(s => s == false));
            Assert.Null(tree.Children);
        }

        [Test]
        public void TestCompact_AllTrue()
        {
            var signs = new Array3D<bool>(new global::DirectX11.Point3(3, 3, 3));
            signs.ForEach((b, p) => signs[p] = true);
            var tree = builder.GenerateCompactedTreeFromSigns(signs, smallCubeSize);

            Assert.AreEqual(8, tree.Signs.Count(s => s == true));
            Assert.Null(tree.Children);
        }

        [Test]
        public void TestCorrectRootsize()
        {
            var signs = new Array3D<bool>(new global::DirectX11.Point3(3, 3, 3));
            var tree = builder.GenerateCompactedTreeFromSigns(signs, smallCubeSize);

            Assert.AreEqual(smallCubeSize, tree.Size);
        }

        public void TestCompactComplexSigns_Sin()
        {
            var signs = new Array3D<bool>(new global::DirectX11.Point3(3, 3, 3));
            signs.ForEach((b, p) => signs[p] = Math.Sin(p.X) + Math.Sin(p.Z) < 0);
            var tree = builder.GenerateCompactedTreeFromSigns(signs, smallCubeSize);

            var helper = new ClipMapsOctree<SignedOctreeNode>();

            signs.ForEach( ( actualSign, worldPos ) =>
            {
                //TODO: verify this check
                var signFound = false;
                helper.VisitDepthFirst( tree, node =>
                {
                    var relative = worldPos - node.LowerLeft;

                    var isCorner = false;
                    for ( int i = 0; i < 3; i++ )
                        if (relative[i] == 0 || relative[i] == node.Size )
                            isCorner = true;

                    if ( isCorner )
                    {
                        var offset = relative/( node.Size );
                        var signIndex = SignedOctreeNode.SignOffsets.IndexOf( offset );
                        
                        Assert.AreEqual( actualSign, signIndex );
                        signFound = true;

                    }

                    if ( node.Children == null )
                    {
                        //This is leaf, so either the corners have been verified in previous step, or this is an all full or all empty cube
                        Assert.IsTrue( node.Signs.All( sign => sign == actualSign ) );
                        signFound = true;
                    }
                    

                } );
                Assert.IsTrue( signFound );
            } );



            //helper.VisitDepthFirst( tree, node =>
            //{
            //    for ( int i = 0; i < 8; i++ )
            //    {
            //        var offset = SignedOctreeNode.SignOffsets[ i ];
            //        var sign = node.Signs[ i ];
            //        var worldOffset = node.LowerLeft +  offset*node.Size;
            //        Assert.AreEqual(signs[worldOffset],sign);
            //    }
            //} );
        }
    }

}
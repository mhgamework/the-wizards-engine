using System;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Features.Testing;
using NUnit.Framework;
using MHGameWork.TheWizards.GodGame._Engine;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures
{
    [EngineTest]
    [TestFixture]
    public class Array3DTest
    {
        [Test]
        public void TestCacheTrashingByteArray()
        {
            var size = 100;
            var arr = new Array3D<byte[]>(new Point3(size, size, size));
            arr.ForEach((v, p) => arr[p] = new byte[1024]);


            byte add = 2;

            PerformanceHelper.Measure(() =>
                {
                    for (int x = 0; x < size; x++)
                        for (int y = 0; y < size; y++)
                            for (int z = 0; z < size; z++)
                            {
                                var value = arr.GetFast(x, y, z);
                                for (int i = 0; i < value.Length; i++)
                                {
                                    add += value[i];
                                }
                            }
                }).PrettyPrint().With(s => Console.WriteLine("xyz: " + s));

            PerformanceHelper.Measure(() =>
            {
                for (int z = 0; z < size; z++)
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                        {
                            var value = arr.GetFast(x, y, z);
                            for (int i = 0; i < value.Length; i++)
                            {
                                add += value[i];
                            }
                        }
            }).PrettyPrint().With(s => Console.WriteLine("zyx: " + s));

        }
        [Test]
        public void TestCacheTrashing()
        {
            var size = 100*2;
            var arr = new Array3D<int>(new Point3(size, size, size));
            arr.ForEach((v, p) => arr[p] = p.X + p.Y + p.Z);


            int add = 2;

            PerformanceHelper.Measure(() =>
            {
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                        for (int z = 0; z < size; z++)
                        {
                            var value = arr.GetFast(x, y, z);
                            add = (add + value) % 1654654;
                        }
            }).PrettyPrint().With(s => Console.WriteLine("xyz: " + s));

            PerformanceHelper.Measure(() =>
            {
                for (int z = 0; z < size; z++)
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                        {
                            var value = arr.GetFast(x, y, z);
                            add = (add + value) % 1654654;
                        }
            }).PrettyPrint().With(s => Console.WriteLine("zyx: " + s));

        }

        [Test]
        public void TestAccessPerformance()
        {
            var size = 100;
            var times = 10000;


            var arr = new float[size, size, size];

            var multiDim = PerformanceHelper.Measure(() =>
                {
                    for (int j = 0; j < times; j++)
                        for (int i = 0; i < size - 2; i++)
                        {
                            var val = arr[i, i + 1, i + 2];
                            var val2 = arr[i + 1, i + 2, i];
                        }
                });
            Console.WriteLine("Multidim: " + multiDim.PrettyPrint());


            var arr2 = new float[size * size * size];

            var singleDim = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                    for (int i = 0; i < size - 2; i++)
                    {
                        var val = arr2[(i + (i + 1) * size) * size + i + 2];
                        var val2 = arr2[(i + (i + 2) * size) * size + i + 1];
                    }
            });
            Console.WriteLine("SingleDim: " + singleDim.PrettyPrint());


            Assert.Less(singleDim, multiDim);

            var arr3d = new Array3D<float>(new Point3(size, size, size));
            var arr3DSpeed = PerformanceHelper.Measure(() =>
                {
                    for (int j = 0; j < times; j++)
                        for (int i = 0; i < size - 2; i++)
                        {
                            var val = arr3d[new Point3(i, i + 1, i + 2)];
                            var val2 = arr3d[new Point3(-i, -i + 1, -i + 2)];
                        }
                });
            Console.WriteLine("arr3d[]: " + arr3DSpeed.PrettyPrint());

            var arr3DFastSpeed = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                    for (int i = 0; i < size - 2; i++)
                    {
                        var val = arr3d.GetFast(i, i + 1, i + 2);
                        var val2 = arr3d.GetFast(i, i + 2, i + 1);
                    }
            });
            Console.WriteLine("arr3d.GetFast:" + arr3DFastSpeed.PrettyPrint());

            var arr3DTiled = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                    for (int i = 0; i < size - 2; i++)
                    {
                        var val = arr3d.GetTiled(new Point3(i, i + 1, i + 2));
                        var val2 = arr3d.GetTiled(new Point3(i, i + 2, i + 1));
                    }
            });
            Console.WriteLine("arr3d.GetTiled:" + arr3DTiled.PrettyPrint());

            var arr3DTiledFast = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                    for (int i = 0; i < size - 2; i++)
                    {
                        var val = arr3d.GetTiledFast(i, i + 1, i + 2);
                        var val2 = arr3d.GetTiledFast(i, i + 2, i + 1);
                    }
            });
            Console.WriteLine("arr3d.GetTiledFast:" + arr3DTiledFast.PrettyPrint());

            var arr3DTiledCacheSize = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                    for (int i = 0; i < size - 2; i++)
                    {
                        var a1 = TWMath.nfmod(i, size);
                        var a2 = TWMath.nfmod(i + 1, size);
                        var a3 = TWMath.nfmod(i + 2, size);
                        var a4 = TWMath.nfmod(i, size);
                        var a5 = TWMath.nfmod(i + 2, size);
                        var a6 = TWMath.nfmod(i + 1, size);
                        var val = arr3d.GetFast(a1, a2, a3);
                        var val2 = arr3d.GetFast(a4, a5, a6);
                    }
            });
            Console.WriteLine("arr3d.GetTiledCacheSize:" + arr3DTiledCacheSize.PrettyPrint());


            var internalArr = arr3d.GetInternalField<float[, ,]>("arr");
            var methodGet = PerformanceHelper.Measure(() =>
            {
                for (int j = 0; j < times; j++)
                    for (int i = 0; i < size - 2; i++)
                    {
                        var val = Val(internalArr, i, i + 1, i + 2);
                        var val2 = Val(internalArr, i, i + 2, i + 1);
                    }
            });
            Console.WriteLine("DirectMethod: " + methodGet.PrettyPrint());

            Assert.Less(arr3DFastSpeed, arr3DSpeed);
        }

        private float Val(float[, ,] arr, int x, int y, int z)
        {
            return arr[x, y, z];
        }
    }
}
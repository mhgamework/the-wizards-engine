using System;
using Castle.DynamicProxy;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using NSubstitute;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class VoxelChangeListenerTest
    {
        [Test]
        public void TestAdjacentChange()
        {
            var s = new VoxelChangeListener();

            var world = new Internal.Model.World((w, p) => new GameVoxel(w, p, new ProxyGenerator()));
            world.Initialize(10, 10);

            var target = world.GetVoxel(new Point2(5, 5));

            var count = 0;

            s.RegisterAdjacentChange(target, _ => count++);

            world.GetVoxel(new Point2(4, 5)).Data.Type = new GameVoxelType("Type1");
            world.GetVoxel(new Point2(5, 4)).Data.Type = new GameVoxelType("Type3");
            world.GetVoxel(new Point2(5, 7)).Data.Type = new GameVoxelType("Type2");

            s.ProcessChanges(world);
            world.ClearChangedFlags();
            Assert.AreEqual(2, count);

            s.UnRegisterAdjacentChange(target);

            world.GetVoxel(new Point2(4, 5)).Data.Type = new GameVoxelType("Type1");
            world.GetVoxel(new Point2(5, 4)).Data.Type = new GameVoxelType("Type3");
            world.GetVoxel(new Point2(5, 7)).Data.Type = new GameVoxelType("Type2");

            s.ProcessChanges(world);
            world.ClearChangedFlags();
            Assert.AreEqual(2, count);



        }

        [Test]
        public void TestRegisterAnyChange()
        {
            var s = new VoxelChangeListener();

            var world = new Internal.Model.World((w, p) => new GameVoxel(w, p, new ProxyGenerator()));
            world.Initialize(10, 10);
            var count = 0;

            var d = s.ChangedVoxels.Subscribe(_ => count++);

            world.GetVoxel(new Point2(4, 5)).Data.Type = new GameVoxelType("Type1");
            world.GetVoxel(new Point2(5, 4)).Data.Type = new GameVoxelType("Type3");
            world.GetVoxel(new Point2(5, 7)).Data.Type = new GameVoxelType("Type2");

            s.ProcessChanges(world);
            world.ClearChangedFlags();
            Assert.AreEqual(3, count);

            d.Dispose();

            world.GetVoxel(new Point2(4, 5)).Data.Type = new GameVoxelType("Type1");
            world.GetVoxel(new Point2(5, 4)).Data.Type = new GameVoxelType("Type3");
            world.GetVoxel(new Point2(5, 7)).Data.Type = new GameVoxelType("Type2");

            s.ProcessChanges(world);
            world.ClearChangedFlags();
            Assert.AreEqual(3, count);



        }


    }
}
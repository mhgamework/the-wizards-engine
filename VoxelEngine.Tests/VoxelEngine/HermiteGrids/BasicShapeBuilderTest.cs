﻿using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{
    public class BasicShapeBuilderTest :EngineTestFixture
    {
        private DualContouringTestEnvironment env;
        private BasicShapeBuilder builder = new BasicShapeBuilder();
        [SetUp]
        public void Setup()
        {
            env = new DualContouringTestEnvironment();
            env.AddToEngine(EngineFactory.CreateEngine());
        }

        [Test]
        public void TestUnitCube()
        {
            env.Grid = builder.CreateCube(1);
        }
        [Test]
        public void Test4Cube()
        {
            env.Grid = builder.CreateCube(4);
        }

        [Test]
        public void TestUnitSphere()
        {
            env.Grid = builder.CreateSphere(1);

        }
        [Test]
        public void Test4Sphere()
        {
            env.Grid = builder.CreateSphere(4);

        }

    }
}
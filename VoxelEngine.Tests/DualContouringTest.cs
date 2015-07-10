using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DualContouring._Test;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using Matrix = SlimDX.Matrix;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.DualContouring
{
    [TestFixture]
    [EngineTest]
    public class DualContouringTest
    {
        private float gridWorldSize;
        private int subdivision;
        private float cellSize;
        private LineManager3DLines lines;
        private DualContouringTestEnvironment environment;

        [SetUp]
        public void Setup()
        {
            environment = new DualContouringTestEnvironment();
            environment.AddToEngine(EngineFactory.CreateEngine());

            gridWorldSize = 10f;
            subdivision = 20;
            cellSize = gridWorldSize / subdivision;
            lines = new LineManager3DLines(TW.Graphics.Device);
        }


        [Test]
        public void TestIntersectableGeometryCube()
        {
            environment.Grid = createCubeGrid();
        }

        [Test]
        public void TestIntersectableGeometrySphere()
        {
            environment.Grid = createSphereGrid();
        }

        public HermiteDataGrid createSphereGrid()
        {
            return HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Scaling(new Vector3(4)) * Matrix.Translation(5, 5, 5),
                                                             new IntersectableSphere());
        }
        public HermiteDataGrid createCubeGrid()
        {
            return HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Scaling(new Vector3(4)) * Matrix.Translation(5, 5, 5),
                                                             new IntersectableCube());
        }
    }
}
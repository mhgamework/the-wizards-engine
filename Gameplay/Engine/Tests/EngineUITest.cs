using System;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Simulators;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;
using NUnit.Framework;
using TreeGenerator.NoiseGenerater;
using TreeGenerator.TerrrainGeneration;

namespace MHGameWork.TheWizards.Engine.Tests
{
    [TestFixture]
    [EngineTest]
    public class EngineUITest
    {
        [Test]
        public void TestShowError()
        {
            var engine = EngineFactory.CreateEngine();

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    throw new NotImplementedException();
                }));

        }


    }
}
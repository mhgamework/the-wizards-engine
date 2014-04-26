﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class JumpPadTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestJumpPadFlight()
        {
            var level = new Level();
            var player = level.LocalPlayer;

            AddonsTest.AddPlaySimulators(level,player,engine);

            player.Position = new Vector3(0, 3, 0);

            var i = level.CreateNewIsland(new Vector3(0, 0, 0));
            var jumpPad01 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad01);

            i = level.CreateNewIsland(new Vector3(0, 50, 150));
            var jumpPad02 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad02);

            i = level.CreateNewIsland(new Vector3(10, 50, 140));
            var jumpPad03 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad03);

            i = level.CreateNewIsland(new Vector3(20, 0, 0));
            var jumpPad04 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad04);

            jumpPad01.TargetJumpPad = jumpPad02;
            jumpPad02.TargetJumpPad = jumpPad03;
            jumpPad03.TargetJumpPad = jumpPad04;
            jumpPad04.TargetJumpPad = jumpPad04;

        }

        [Test]
        public void TestJumpPadTrajectory()
        {
            var level = new Level();

            var padPos = new Vector3(150, 3, -50);

            var isle01 = level.CreateNewIsland(new Vector3(0, 0, 0));
            var pad = new JumpPad(level, isle01.Node.CreateChild());

            var isle02 = level.CreateNewIsland(padPos);
            var target = new JumpPad(level, isle02.Node.CreateChild());

            pad.TargetJumpPad = target;

            pad.CalculateTrajectorySettings();
            var endPos = pad.GetPosAtTime(float.MaxValue);

            Assert.True(Vector3.Distance(endPos, target.GetLandingCoordinates()) < 0.01f);
        }

        [Test]
        public void TestJumpPadSelection()
        {
            var level = new Level();
            var player = level.LocalPlayer;

            AddonsTest.AddPlaySimulators(level, player, engine);

            player.Position = new Vector3(0, 3, 0);

            var i = level.CreateNewIsland(new Vector3(2, 0, 2));
            var jumpPad01 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad01);

            i = level.CreateNewIsland(new Vector3(250, 0, 0));
            var jumpPad02 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad02);

            i = level.CreateNewIsland(new Vector3(10, 0, 10));
            var jumpPad03 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad03);

            i = level.CreateNewIsland(new Vector3(50, 0, 100));
            var jumpPad04 = new JumpPad(level, i.Node.CreateChild());
            i.AddAddon(jumpPad04);

            jumpPad01.TargetJumpPad = jumpPad02;
            jumpPad02.TargetJumpPad = jumpPad03;
            jumpPad03.TargetJumpPad = jumpPad04;
            jumpPad04.TargetJumpPad = jumpPad04;
        }

        [Test]
        public void TestJumpPadInWorld()
        {
            var level = new Level();
            var player = level.LocalPlayer;
            addPlaySimulators(level, player);
            var gen = new WorldGenerationService(level, new Random(0),null);

            gen.Generate();

            var allPads = new List<JumpPad>();

            var isle = level.Islands.ElementAt(0);
            var jumpPad01 = new JumpPad(level, isle.Node.CreateChild());
            isle.AddAddon(jumpPad01);
            var firstJumpPad = jumpPad01;
            allPads.Add(firstJumpPad);
            var nbJumpPads = (int)Math.Floor(level.Islands.Count() * 0.1f);
            var rnd = new Random(0);

            for (int j = 1; j < nbJumpPads; j++)
            {
                var index = rnd.Next(0, level.Islands.Count());

                isle = level.Islands.ElementAt(index);
                var jumpPad02 = new JumpPad(level, isle.Node.CreateChild());
                isle.AddAddon(jumpPad02);
                allPads.Add(jumpPad02);
            }

            #region padTargetting
            foreach (var pad in allPads)
            {
                Vector3 padPos;
                Vector3 s;
                Quaternion r;
                pad.Node.Absolute.Decompose(out s, out r, out padPos);

                for (int i = allPads.IndexOf(pad) + 1; i < allPads.Count; i++)
                {
                    Vector3 tPadPos;
                    allPads[i].Node.Absolute.Decompose(out s, out r, out tPadPos);
                    if (Vector3.Distance(padPos, tPadPos) <= pad.MaxJumpDistance)
                    {
                        pad.TargetJumpPad = allPads[i];
                        break;
                    }
                }

                if (pad.TargetJumpPad != null)
                    continue;

                for (int i = 0; i < allPads.IndexOf(pad); i++)
                {
                    Vector3 tPadPos;
                    allPads[i].Node.Absolute.Decompose(out s, out r, out tPadPos);
                    if (Vector3.Distance(padPos, tPadPos) <= pad.MaxJumpDistance)
                    {
                        pad.TargetJumpPad = allPads[i];
                        break;
                    }
                }
            }
            #endregion padTargetting

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }

        private void addPlaySimulators(Level level, ScatteredPlayer player)
        {
            AddonsTest.AddPlaySimulators(level, player, engine);
        }

    }
}

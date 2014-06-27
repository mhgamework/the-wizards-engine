using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.Model;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    [EngineTest]
    public class GodPowersTest
    {
        [Test]
        public void TestShapeLand()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var world = new World(10, 10);


            var playerInputHandler = new PlayerInputHandler(world);
            var playerInputSimulator = new PlayerInputSimulator(playerInputHandler, world);
            engine.AddSimulator(playerInputSimulator);
            engine.AddSimulator(new BasicSimulator(() =>
                {
                    var target = playerInputSimulator.GetTargetedVoxel();
                    world.ForEach((v, p) =>
                        {
                            var col = Color.Black.dx();
                            if (v == target)
                                col = Color.Red; 
                            if (v.Type == GameVoxelType.Land)
                                col = Color.Green;

                            TW.Graphics.LineManager3D.AddBox(v.GetBoundingBox(), col);
                            TW.Graphics.LineManager3D.AddLine(v.GetBoundingBox().Minimum, v.GetBoundingBox().Maximum, col);
                        });

                }));
            engine.AddSimulator(new WorldRenderingSimulator());

        }
    }

    public class PlayerInputSimulator : ISimulator
    {
        private readonly PlayerInputHandler handler;
        private World world;

        public PlayerInputSimulator(PlayerInputHandler handler, World world)
        {
            this.handler = handler;
            this.world = world;
        }


        public void Simulate()
        {
            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                handler.OnLeftClick(GetTargetedVoxel());
            if (TW.Graphics.Mouse.RightMouseJustPressed)
                handler.OnRightClick(GetTargetedVoxel());
        }

        public GameVoxel GetTargetedVoxel()
        {
            var groundPos = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!groundPos.HasValue) return null;
            return world.GetVoxelAtGroundPos(groundPos.Value);
        }

    }

    public class PlayerInputHandler
    {
        private readonly World world;

        public PlayerInputHandler(World world)
        {
            this.world = world;
        }

        public void OnLeftClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Air);
        }
        public void OnRightClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Land);
        }
    }
    public class GameVoxelType
    {
        public static GameVoxelType Air;
        public static GameVoxelType Land;

        static GameVoxelType()
        {
            Air = new GameVoxelType("Air");
            Land = new GameVoxelType("Land");
        }

        public string Name { get; private set; }

        public GameVoxelType(string name)
        {
            Name = name;
        }

    }

}
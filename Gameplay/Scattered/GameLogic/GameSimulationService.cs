using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.GameLogic
{
    /// <summary>
    /// Performs the update step of the gameplay phase
    /// </summary>
    public class GameSimulationService : ISimulator
    {
        private Level level;

        private EnemySpawningService enemySpawningService;
        private readonly PlayerInteractionService interactionService;
        private readonly ClusterPhysicsService clusterPhysicsService;

        public GameSimulationService(Level level, EnemySpawningService enemySpawningService,PlayerInteractionService interactionService, ClusterPhysicsService clusterPhysicsService)
        {
            this.level = level;
            this.enemySpawningService = enemySpawningService;
            this.interactionService = interactionService;
            this.clusterPhysicsService = clusterPhysicsService;
        }

        public void Simulate()
        {
            simulatePlayer();
            simulateGameObjects();
            clusterPhysicsService.UpdateClusterMovement();
        }

        private void simulateGameObjects()
        {
            level.SimulateBehaviours();
            enemySpawningService.Simulate();
        }

        private void simulatePlayer()
        {
            interactionService.SimulateInteraction();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Q))
                level.LocalPlayer.AttemptDropResource();

            level.LocalPlayer.AttemptHeal();

            if (!TW.Graphics.Mouse.CursorEnabled && TW.Graphics.Mouse.LeftMouseJustPressed)
            {
                level.LocalPlayer.Shoot();
            }

            level.LocalPlayer.MovementDisabled =
                level.Islands.SelectMany(i => i.Addons.OfType<JumpPad>()).Any(j => j.IsPerformingJump);
        }
    }
}
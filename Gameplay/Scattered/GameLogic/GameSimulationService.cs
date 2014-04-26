using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Core.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX.DirectInput;
using System.Linq;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.Core
{
    /// <summary>
    /// Performs the update step of the gameplay phase
    /// </summary>
    public class GameSimulationService : ISimulator
    {
        private Level level;

        private EnemySpawningService enemySpawningService;

        public GameSimulationService(Level level, EnemySpawningService enemySpawningService)
        {
            this.level = level;
            this.enemySpawningService = enemySpawningService;
        }

        public void Simulate()
        {
            simulatePlayer();
            simulateGameObjects();
        }

        private void simulateGameObjects()
        {
            level.SimulateBehaviours();
            enemySpawningService.Simulate();
        }

        private void simulatePlayer()
        {
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
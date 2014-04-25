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
    public class GameplaySimulator : ISimulator
    {
        private Level level;

        public GameplaySimulator(Level level)
        {
            this.level = level;
        }

        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Q))
                level.LocalPlayer.AttemptDropResource();

            level.LocalPlayer.AttemptHeal();

            if (!TW.Graphics.Mouse.CursorEnabled && TW.Graphics.Mouse.LeftMouseJustPressed)
            {
                level.LocalPlayer.Shoot();
            }

            level.Islands.SelectMany(i => i.Addons.OfType<Resource>()).ToArray().ForEach(k => k.AttemptMerge());

            level.LocalPlayer.MovementDisabled = level.Islands.SelectMany(i => i.Addons.OfType<JumpPad>()).Any(j => j.IsPerformingJump);

            var bulletCopy = level.Bullets.ToList();
            foreach (var bullet in bulletCopy)
            {
                bullet.Update();
            }

            level.SimulateBehaviours();


        }
    }
}
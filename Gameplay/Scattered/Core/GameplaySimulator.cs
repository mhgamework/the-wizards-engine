using MHGameWork.TheWizards.Engine;
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

            level.Islands.SelectMany(i => i.Addons.OfType<Resource>()).ToArray().ForEach(k => k.AttemptMerge());
        }
    }
}
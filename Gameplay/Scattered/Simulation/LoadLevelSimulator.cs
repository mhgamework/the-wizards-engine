using System;
using System.IO;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    /// <summary>
    /// Allows loading and saving of the current level
    /// </summary>
    public class LoadLevelSimulator : ISimulator
    {
        private readonly Level level;

        public LoadLevelSimulator(Level level)
        {
            this.level = level;

            try
            {
                loadLevel();
            }
            catch (Exception ex)
            {
             Console.WriteLine(ex);   
            }
        }

        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.O)) saveLevel();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.I)) loadLevel();
        }

        private void saveLevel()
        {
            var s = new LevelSerializer();
            s.Serialize(level,new FileInfo(TWDir.GameData.CreateSubdirectory("Scattered") + "\\Level.txt"));
        }
        private void loadLevel()
        {
            var s = new LevelSerializer();
            s.Deserialize(level, new FileInfo(TWDir.GameData.CreateSubdirectory("Scattered") + "\\Level.txt"));
        }

    }
}
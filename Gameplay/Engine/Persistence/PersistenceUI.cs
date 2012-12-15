using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.Persistence
{
    /// <summary>
    /// Part of the EngineUI
    /// </summary>
    public class PersistenceUI
    {
        private List<SaveEntry> saves;
        private TextMenu<Action> menu;

        private string saveDirectory;

        public PersistenceUI()
        {
            menu = new TextMenu<Action>();

            saves = new List<SaveEntry>();


            saveDirectory = TWDir.GameData + "\\Saves";

            Directory.CreateDirectory(saveDirectory);
            updateSavesList();

        }

        public void CreateNewSave()
        {
            var name = InputBox.ShowInputBox("Name?", "Save world");
            TW.Data.GetSingleton<Datastore>().SaveToFile(new FileInfo(saveDirectory + "\\" + name + ".xml"));

            updateSavesList();
        }

        private void updateSavesList()
        {
            saves.Clear();
            menu.Items.Clear();
            foreach (var filename in Directory.EnumerateFiles(saveDirectory))
            {
                var saveEntry = new SaveEntry { Name = Path.GetFileName(filename), File = new FileInfo(filename) };
                saves.Add(saveEntry);
                menu.Items.Add(new TextMenuItem<Action> { Label = saveEntry.Name, Data = () => saveEntry.LoadSave() });
            }
            menu.Items.Add(new TextMenuItem<Action> { Label = "[New Save]", Data = () => CreateNewSave() });
        }

        public TextMenu<Action> CreateMenu()
        {
            return menu;
        }

        private class SaveEntry
        {
            public string Name;
            public FileInfo File;

            public void LoadSave()
            {
                TW.Data.GetSingleton<Datastore>().LoadFromFile(File);
            }



        }
    }
}

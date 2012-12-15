using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.Persistence
{
    /// <summary>
    /// Responsible for simulating the
    /// </summary>
    public class PersistenceInterfaceSimulator : ISimulator
    {
        private Textarea area;
        private List<SaveEntry> saves;
        private TextMenu<Action> menu;

        private string saveDirectory;

        public PersistenceInterfaceSimulator()
        {
            area = new Textarea();
            area.Position = new Vector2(0, 0);
            area.Size = new Vector2(200, 600);

            menu = new TextMenu<Action>();

            saves = new List<SaveEntry>();


            saveDirectory = TWDir.GameData + "\\Saves";

            Directory.CreateDirectory(saveDirectory);
            updateSavesList();

        }
        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.DownArrow))
                menu.MoveDown();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.UpArrow))
                menu.MoveUp();




            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Return))
                if (menu.SelectedItem != null) menu.SelectedItem();

            area.Text = menu.generateText();

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

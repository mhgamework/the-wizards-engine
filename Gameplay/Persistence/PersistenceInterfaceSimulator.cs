using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Serialization;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for simulating the
    /// </summary>
    public class PersistenceInterfaceSimulator : ISimulator
    {
        private Textarea area;
        private List<SaveEntry> saves;
        private Menu<Action> menu;

        private string saveDirectory;

        public PersistenceInterfaceSimulator()
        {
            area = new Textarea();
            area.Position = new Vector2(0, 0);
            area.Size = new Vector2(200, 600);

            menu = new Menu<Action>();

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
                menu.Items.Add(new MenuItem<Action> { Label = saveEntry.Name, Data = () => saveEntry.LoadSave() });
            }
            menu.Items.Add(new MenuItem<Action> { Label = "[New Save]", Data = () => CreateNewSave() });
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

        private class Menu<T> where T : class
        {
            public List<MenuItem<T>> Items = new List<MenuItem<T>>();
            private int selected;


            public void MoveDown()
            {
                selected++;
                normalizeSelected();
            }

            private void normalizeSelected()
            {
                selected = (int)MathHelper.Clamp(selected, 0, Items.Count);
            }

            public void MoveUp()
            {
                selected--;
                normalizeSelected();
            }

            public T SelectedItem
            {
                get
                {
                    if (selected < 0)
                        return null;
                    if (selected >= Items.Count)
                        return null;
                    return Items[selected].Data;
                }
            }


            public string generateText()
            {
                normalizeSelected();
                var ret = "\n\n\n";
                string prefix = "      ";
                string selectedPrefixArrow = "-> ";
                string notSelectedPrefix = "   ";
                for (int i = 0; i < Items.Count; i++)
                {
                    var save = Items[i];
                    var selectedPrefix = notSelectedPrefix;
                    if (selected == i)
                        selectedPrefix = selectedPrefixArrow;

                    ret += prefix + selectedPrefix + save.Label;
                    ret += "\n";
                }
                return ret;
            }

        }

        private class MenuItem<T>
        {
            public String Label;
            public T Data;
        }
    }
}

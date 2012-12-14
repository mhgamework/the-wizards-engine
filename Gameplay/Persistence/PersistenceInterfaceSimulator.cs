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
        private int selected;
        private List<SaveEntry> saves;
        private ModelSerializer modelSerializer;
        private string saveDirectory;

        public PersistenceInterfaceSimulator()
        {
            area = new Textarea();
            area.Position = new Vector2(0, 0);
            area.Size = new Vector2(200, 600);

            saves = new List<SaveEntry>();

          


            var stringSerializer = StringSerializer.Create();
            stringSerializer.AddConditional(new FilebasedAssetSerializer());
            modelSerializer = new Persistence.ModelSerializer(stringSerializer);
            saveDirectory = TWDir.GameData + "\\Saves";

            Directory.CreateDirectory(saveDirectory);
            updateSavesList();

        }
        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.DownArrow))
                selected++;
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.UpArrow))
                selected--;
            selected = (int)MathHelper.Clamp(selected, 0, getSaves().Count());

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Return))
                if (selected == getSaves().Count())
                    save();
                else
                    loadSave();


            area.Text = generateText();

        }

        private void save()
        {
            var name = InputBox.ShowInputBox("Name?", "Save world");
            TW.Data.GetSingleton<Datastore>().SaveToFile(new FileInfo(saveDirectory + "\\" + name + ".xml"));

            updateSavesList();
        }

        private void updateSavesList()
        {
            saves.Clear();
            foreach (var filename in  Directory.EnumerateFiles(saveDirectory))
            {
                saves.Add(new SaveEntry {Name = Path.GetFileName(filename), File = new FileInfo(filename)});
            }
        }

        private void loadSave()
        {
            var save = getSaves()[selected];

           TW.Data.GetSingleton<Datastore>().LoadFromFile(save.File);

        }

        private string generateText()
        {
            var ret = "\n\n\n";
            string prefix = "      ";
            string selectedPrefixArrow = "-> ";
            string notSelectedPrefix = "   ";
            foreach (var save in getSaves())
            {
                var selectedPrefix = notSelectedPrefix;
                if (save == getSelected())
                {
                    selectedPrefix = selectedPrefixArrow;
                }
                ret += prefix + selectedPrefix + save.Name;
                ret += "\n";
            }
            ret += "\n";
            var pre = prefix + notSelectedPrefix;
            if (selected == getSaves().Count)
                pre = prefix + selectedPrefixArrow;
            ret += pre + "[New Save]";
            return ret;
        }

        private SaveEntry getSelected()
        {
            if (selected < 0)
                return null;
            if ( selected >= getSaves().Count)
                return null;
            return getSaves()[selected];
        }

        private List<SaveEntry> getSaves()
        {
            return saves;
        }

        private class SaveEntry
        {
            public string Name;
            public FileInfo File;
        }
    }
}

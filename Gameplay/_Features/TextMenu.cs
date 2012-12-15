using System.Collections.Generic;
using DirectX11;

namespace MHGameWork.TheWizards.Persistence
{
    public class TextMenu<T> where T : class
    {
        public List<TextMenuItem<T>> Items = new List<TextMenuItem<T>>();
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
}
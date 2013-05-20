using System;
using System.Collections;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting
{
    /// <summary>
    /// Contains the configuration for the current editor menu.
    /// </summary>
    public class EditorMenuConfiguration
    {
        public EditorMenuConfiguration()
        {
            Items = new List<Item>();
        }

        public void CreateItem(string rivers, Action enableRivers)
        {
            Items.Add(new Item{Name = rivers,Command = enableRivers});
        }

        public List<Item> Items { get; private set; }

        public class Item
        {
            public string Name;
            public Action Command;
        }
    }
}
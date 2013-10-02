using System;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Allows binding a script (specified on construction) to a world object
    /// </summary>
    public class ScriptToolItem : IHotbarItem
    {
        private readonly Type scriptType;

        public ScriptToolItem(Type scriptType)
        {
            if (!typeof(IWorldScript).IsAssignableFrom(scriptType)) throw new InvalidOperationException();
            this.scriptType = scriptType;
        }

        public string Name { get { return scriptType.Name; } }
        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
        }
    }
}
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Allows binding a script (specified on construction) to a world object
    /// </summary>
    public class ScriptToolItem : IHotbarItem
    {
        private readonly IWorldScript script;

        public ScriptToolItem(IWorldScript script)
        {
            this.script = script;
        }

        public string Name { get; private set; }
        public void OnSelected()
        {
            throw new System.NotImplementedException();
        }

        public void OnDeselected()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
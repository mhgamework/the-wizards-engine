namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    public interface IHotbarItem
    {
        string Name { get; }

        void OnSelected();
        void OnDeselected();

        /// <summary>
        /// Only called when selected
        /// </summary>
        void Update();

    }
}
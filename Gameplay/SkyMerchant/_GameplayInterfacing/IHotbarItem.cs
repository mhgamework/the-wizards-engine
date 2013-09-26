namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Represents an item on the hotbar, which the user can select and deselect.
    /// </summary>
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
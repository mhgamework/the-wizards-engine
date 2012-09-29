namespace MHGameWork.TheWizards.Data
{
    public enum ModelChange
    {
        /// <summary>
        /// Note: the meaning of the none-state has been changed from an illegal state, to a state where nothing changed
        ///        and is now a valid state
        /// </summary>
        None = 0,
        Added = 1,
        Modified = 2,
        Removed = 3

    }
}
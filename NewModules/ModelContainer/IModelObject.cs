namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// Should have an empty constructor, if automatic synchronization is to be used.
    /// </summary>
    public interface IModelObject
    {
        ModelContainer Container { get; }
        /// <summary>
        /// This is called by the ModelContainer
        /// </summary>
        /// <param name="container"></param>
        void Initialize(ModelContainer container);
    }
}

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// Basic implementation of the functionality that a modelobject is required to implement
    /// </summary>
    [ModelObjectChanged]
    public class BaseModelObject : IModelObject
    {

        public BaseModelObject()
        {
            TW.Model.AddObject(this);
        }

        public ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer container)
        {
            Container = container;
        }
    }
}


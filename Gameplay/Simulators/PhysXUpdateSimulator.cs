using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// This simulator pushes the changes from PhysX into the model
    /// </summary>
    public class PhysXUpdateSimulator : ISimulator
    {
        public void Simulate()
        {
            int length;
            ModelContainer.ModelContainer.ObjectChange[] array;
            TW.Model.GetObjectChanges(out array, out length);

            for (int i = 0; i < length; i++)
            {
                var change = array[i];
                if (change.ModelObject is Sphere)
                {
                    ((Sphere) change.ModelObject).ProcessPhysXChanges();
                }
            }
        }
    }
}

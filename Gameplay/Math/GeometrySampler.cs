namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Random sampler with geometric distributions
    /// </summary>
    public class GeometrySampler
    {
        private readonly Seeder s;

        public GeometrySampler(Seeder s)
         {
             this.s = s;
         }
    }
}
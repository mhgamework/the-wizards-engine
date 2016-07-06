namespace MHGameWork.TheWizards.DualContouring.QEFs
{
    public interface IQefCalculator
    {
        Vector3 CalculateMinimizer(Vector3[] normals, Vector3[] posses, int numIntersections, Vector3 preferredPosition);
    }
}
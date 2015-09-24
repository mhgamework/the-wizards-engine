namespace MHGameWork.TheWizards
{
    public struct Matrix_Adapter
    {
        public static global::MHGameWork.TheWizards.Matrix_Adapter operator *(global::MHGameWork.TheWizards.Matrix_Adapter a, global::MHGameWork.TheWizards.Matrix_Adapter b)
        {
            throw new System.InvalidOperationException();
        }

        public static global::MHGameWork.TheWizards.Matrix_Adapter Scaling(System.Single x, System.Single y, System.Single z)
        {
            throw new System.InvalidOperationException();
        }

        public static global::MHGameWork.TheWizards.Matrix_Adapter Scaling(global::MHGameWork.TheWizards.Vector3_Adapter v)
        {
            throw new System.InvalidOperationException();
        }

        public static global::MHGameWork.TheWizards.Matrix_Adapter Invert(global::MHGameWork.TheWizards.Matrix_Adapter world)
        {
            throw new System.InvalidOperationException();
        }
    }
}
namespace MHGameWork.TheWizards.MathExtra
{
    public interface Function
    {
        /**
         * Evaluates a function at a given point
         * 
         * @param x
         *            the point, at which the function has to be evaluated
         * @return the function value at <code>x</code>
         */
        double valueAt( double x );
    }
}

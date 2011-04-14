using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.MathExtra
{
    public interface Polynomial : Function
    {
        /**
         * Returns the coefficients array of the polynomial
         * 
         * @return the coefficients array of the polynomial, where
         *         <code>result[n]</code> is the coefficient before X^n
         */
        double[] toArray();

        /**
         * Calculates the first derivative of the polynomial
         * 
         * @return the derivation
         */
        Polynomial diff();

        /**
         * Calculates the remainder of the polynomial division of <code>this</code>
         * by <code>other</code>
         * 
         * @param other
         *            the divisor (must not be constant)
         * @return the remainder of the polynomial division
         */
        Polynomial mod( Polynomial other );

        /**
         * Multiplies the polynomial by a real scalar
         * 
         * @param scalar
         *            the scalar to multiply the polynomial by
         * @return the multiplied polynomial
         */
        Polynomial multiply( double scalar );

        /**
         * Determines the degree of the polynomial
         * 
         * @return the degree of the polynomial, where the degree of the zero
         *         polynomial is defined as -1
         */
        int degree();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.MathExtra
{
    public class Polynomial4 : Polynomial
    {
        private double x0, x1, x2, x3, x4;

       
        /// <summary>
        /// Construct a polynomial with given coefficients
        /// </summary>
        /// <param name="x0">coefficient before 1</param>
        /// <param name="x1">coefficient before X</param>
        /// <param name="x2">coefficient before X*X</param>
        /// <param name="x3">coefficient before X*X*X</param>
        /// <param name="x4">coefficient before X*X*X*X</param>
        public Polynomial4( double x0, double x1, double x2, double x3, double x4 )
        {
            this.x0 = x0;
            this.x1 = x1;
            this.x2 = x2;
            this.x3 = x3;
            this.x4 = x4;
        }

        /**
         * Construct a polynomial with given coefficients
         * 
         * @param poly
         *            coefficient array, where
         *            <code>poly[n]</cody> is the coefficient before X^n for
         *            n = 0, 1, 2, 3 or 4.
         *            The array length may be less than 5, but not greater than 5.
         */
        public Polynomial4( double[] poly )
        {
            //WARNING: assert poly.length <= 5;

            if ( poly.Length > 0 )
            {
                x0 = poly[ 0 ];
            }
            if ( poly.Length > 1 )
            {
                x1 = poly[ 1 ];
            }
            if ( poly.Length > 2 )
            {
                x2 = poly[ 2 ];
            }
            if ( poly.Length > 3 )
            {
                x3 = poly[ 3 ];
            }
            if ( poly.Length > 4 )
            {
                x4 = poly[ 4 ];
            }
        }

        public double[] toArray()
        {
            return new double[] { x0, x1, x2, x3, x4 };
        }


        public override string ToString()
        {
            return x4 + " * X^4 + " + x3 + " * X^3 + " + x2 + " * X^2 + " + x1
                + " * X + " + x0;
        }

        public double valueAt( double x )
        {
            // evaluation using the Horner scheme
            double value = x4;
            value = x3 + value * x;
            value = x2 + value * x;
            value = x1 + value * x;
            value = x0 + value * x;
            return value;
        }

        // Actually a degree4-result is sufficient (and we want to make our users
        // benefit from this fact, so we define a Polynomial4-diff-method)...
        public Polynomial4 diff4()
        {
            return new Polynomial4( x1, 2 * x2, 3 * x3, 4 * x4, 0 );
        }

        // ...but to satisfy the interface we need also a method, which returns a
        // general polynomial.
        public Polynomial diff()
        {
            return diff4();
        }

        public Polynomial multiply( double scalar )
        {
            return new Polynomial4( scalar * x0, scalar * x1, scalar * x2, scalar
                    * x3, scalar * x4 );
        }

        public int degree()
        {
            if ( x4 != 0 )
            {
                return 4;
            }
            else if ( x3 != 0 )
            {
                return 3;
            }
            else if ( x2 != 0 )
            {
                return 2;
            }
            else if ( x1 != 0 )
            {
                return 1;
            }
            else if ( x0 != 0 )
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private double[] reduce( double[] a, int degA, double[] b, int degB )
        {
            int degDiff = degA - degB;

            double[] result = new double[ degA ];
            for ( int i = degA - 1; i >= degDiff; i-- )
            {
                result[ i ] = a[ i ] - b[ i - degDiff ] / b[ degB ] * a[ degA ];
            }

            for ( int i = 0; i < degDiff; i++ )
            {
                result[ i ] = a[ i ];
            }
            return result;
        }

        private double[] mod( double[] a, int degA, double[] b, int degB )
        {
            if ( degB < 1 )
            { // the illegal case
                throw new ArgumentException(
                        "Cannot divide by constant polynomials" );
            }
            else if ( degA < degB )
            { // the basic case
                return a;
            }
            else
            { // the recursion case
                // reduce a by b
                double[] result = reduce( a, degA, b, degB );

                // calculate the degree of the result
                int newDeg = degA - 1;
                while ( newDeg >= 0 && result[ newDeg ] == 0 )
                {
                    newDeg--;
                }

                // do recursion
                return mod( result, newDeg, b, degB );
            }
        }

        // Actually a degree4-result is sufficient (and we want to make our users
        // benefit from this fact, so we define a Polynomial4-mod-method)...
        public Polynomial4 mod( Polynomial4 other )
        {
            return new Polynomial4( mod( toArray(), degree(), other.toArray(), other
                    .degree() ) );
        }

        // ...but to satisfy the interface we need also a method, which returns a
        // general polynomial.
        public Polynomial mod( Polynomial other )
        {
            return new Polynomial4( mod( toArray(), degree(), other.toArray(), other
                    .degree() ) );
        }
    }
}

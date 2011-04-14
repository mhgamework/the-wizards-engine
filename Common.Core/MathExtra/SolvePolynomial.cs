using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.MathExtra
{
    public class Solve
    {

        private const double FLOATING_POINT_PRECISION = 0;

       
        /// <summary>
        /// Search zeroes of a polynomial function by executing a bisection algorithm
        /// using Sturm's theorem
        /// </summary>
        /// <param name="poly">the function, whose zeroes are searched</param>
        /// <param name="num">the number of the wanted zero; counting starts from<code>lower</code></param>
        /// <param name="lower">lower bound of the interval, in which the zero is searched</param>
        /// <param name="upper">upper bound of the interval, in which the zero is searched</param>
        /// <param name="precision">tolerance in comparing function values</param>
        /// <param name="iterations">maximum number of iterations (the more iterations, the more
        /// precise the result); the algorithm stops before that maximum
        /// number, when it reaches sufficient precision (machine
        /// precision)</param>
        /// <returns>The zero</returns>
        public static double solve( Polynomial poly, int num, double lower,
                double upper, double precision, int iterations )
        {
            return bisection( calculateSturm( poly ), num, lower, upper, precision,
                    iterations );
        }
        /// <summary>
        /// Search zeroes of a polynomial function by executing a bisection algorithm
        /// using Sturm's theorem
        /// </summary>
        /// <param name="poly">the function, whose zeroes are searched</param>
        /// <param name="num">the number of the wanted zero; counting starts from<code>lower</code></param>
        /// <param name="lower">lower bound of the interval, in which the zero is searched</param>
        /// <param name="upper">upper bound of the interval, in which the zero is searched</param>
        /// <param name="iterations">maximum number of iterations (the more iterations, the more
        /// precise the result); the algorithm stops before that maximum
        /// number, when it reaches sufficient precision (machine
        /// precision)</param>
        /// <returns>The zero</returns>
        public static double solve( Polynomial poly, int num, double lower,
                double upper, int iterations )
        {
            return bisection( calculateSturm( poly ), num, lower, upper,
                    FLOATING_POINT_PRECISION, iterations );
        }

        /**
         * Sturm's "w" function for counting zeroes
         * 
         * @param sturm
         *            the Sturm chain as array
         * @param x
         *            where to evaluate the "w" function
         * @param precision
         *            tolerance in comparing function values
         * @return the result of the "w" function defined by Sturm
         */
        private static int w( Polynomial[] sturm, double x, double precision )
        {
            int signChanges = 0;
            int lastNonZero = 0;
            // run through the array
            for ( int i = 1; i < sturm.Length; i++ )
            {
                if ( Math.Abs( sturm[ i ].valueAt( x ) ) > precision )
                {
                    // compare the sign to the last non-zero sign
                    if ( sturm[ lastNonZero ].valueAt( x ) * sturm[ i ].valueAt( x ) < 0 )
                    {
                        // sign change found: count up
                        signChanges++;
                    }
                    lastNonZero = i;
                }
            }
            return signChanges;
        }

        /**
         * Search zeroes of a polynomial function by executing a bisection algorithm
         * using Sturm's theorem
         * 
         * @param sturm
         *            the Sturm chain of the function
         * @param num
         *            the number of the wanted zero; counting starts from
         *            <code>lower</code>
         * @param lower
         *            lower bound of the interval, in which the zero is searched
         * @param upper
         *            upper bound of the interval, in which the zero is searched
         * @param precision
         *            tolerance in comparing function values
         * @param iterations
         *            maximum number of iterations (the more iterations, the more
         *            precise the result); the algorithm stops before that maximum
         *            number, when it reaches sufficient precision (machine
         *            precision)
         * @return the zero
         */
        private static double bisection( Polynomial[] sturm, int num, double lower,
                double upper, double precision, int iterations )
        {
            // define the point where to start counting the zeroes
            double t = lower;

            // do the maximum number or iterations (if necessary)
            for ( int i = 0; i < iterations; i++ )
            {
                // determine the middle of the interval
                double c = ( upper + lower ) / 2;

                // Check, if we have already reached machine precision
                if ( upper <= lower || c <= lower || c >= upper )
                {
                    return lower;
                }

                // Left or right interval?
                // Are there less than "num" zeroes between t and c?
                if ( w( sturm, t, precision ) - w( sturm, c, precision ) < num )
                {
                    // right
                    lower = c;
                }
                else
                {
                    // left
                    upper = c;
                }
            }
            // the wanted zero lies in the intervall [lower, upper],
            // so the middle of this interval might be a good guess
            return ( upper + lower ) / 2;
        }

        /**
         * Calculates the Sturm chain to a given polynomial
         * 
         * @param function
         *            the polynomial function
         * @return the Sturm chain of <code>function</code> as array
         */
        public static Polynomial[] calculateSturm( Polynomial function )
        {
            LinkedList<Polynomial> sturm = new LinkedList<Polynomial>();

            // add the original function and its derivation
            sturm.AddFirst( function );
            sturm.AddFirst( function.diff() );

            // iteratively perform polynomial divison
            while ( sturm.First.Value.degree() > 0 )
            {
                sturm.AddFirst( sturm.First.Next.Value.mod( sturm.First.Value ).multiply( -1 ) );
            }

            // convert the list to an array for efficiency purposes
            Polynomial[] result = new Polynomial4[ sturm.Count ];
            int i = 0;
            foreach ( Polynomial poly in sturm )
            {
                result[ i ] = poly;
                i++;
            }
            return result;
        }
        //public static Polynomial[] calculateSturm( Polynomial function )
        //{
        //    List<Polynomial> sturm = new LinkedList<Polynomial>();

        //    // add the original function and its derivation
        //    sturm.add( 0, function );
        //    sturm.add( 0, function.diff() );

        //    // iteratively perform polynomial divison
        //    while ( sturm.get( 0 ).degree() > 0 )
        //    {
        //        sturm.add( 0, sturm.get( 1 ).mod( sturm.get( 0 ) ).multiply( -1 ) );
        //    }

        //    // convert the list to an array for efficiency purposes
        //    Polynomial[] result = new Polynomial4[ sturm.size() ];
        //    int i = 0;
        //    foreach ( Polynomial poly in sturm )
        //    {
        //        result[ i ] = poly;
        //        i++;
        //    }
        //    return result;
        //}



        public static void TestSolvePolynomial()
        {
            // x^2 - x = 0
            // Solutions: -1 and 1
            testPoly2( new Polynomial4( new double[] { -1, 0, 1 } ) );

            // x^2 - 2x + 1 = 0
            // Solution: 1
            // As we call the function for 2 solutions, but our polynomial has got
            // only one, the second solution will be nonsense
            testPoly2( new Polynomial4( new double[] { 1, -2, 1 } ) );

            // x^3 - 10x^2 + 31x - 30
            // Solutions: 2, 3 and 5
            testPoly3( new Polynomial4( new double[] { -30, 31, -10, 1 } ) );

            // x^4 - 10x^3 + 31x^2 - 30x
            // Solutions: 0, 2, 3 and 5
            testPoly4( new Polynomial4( new double[] { 0, -30, 31, -10, 1 } ) );

            // x^4 - 13x^3 + 61x^2 - 123x + 90
            // Solutions: 2, 3 and 5
            testPoly3( new Polynomial4( new double[] { 90, -123, 61, -13, 1 } ) );
        }

        private static void testPoly2( Polynomial poly )
        {
            double solution1 = Solve.solve( poly, 1, -10, +10, 200000 );
            double solution2 = Solve.solve( poly, 2, -10, +10, 200000 );
            Console.WriteLine( "Solutions of " + poly + " = 0:\n" );
            Console.WriteLine( "   " + solution1 + " and " + solution2 );
            Console.WriteLine( "\n" );
        }

        private static void testPoly3( Polynomial poly )
        {
            double solution1 = Solve.solve( poly, 1, -10, +10, 200000 );
            double solution2 = Solve.solve( poly, 2, -10, +10, 200000 );
            double solution3 = Solve.solve( poly, 3, -10, +10, 200000 );
            Console.WriteLine( "Solutions of " + poly + " = 0:\n" );
            Console.WriteLine( "   " + solution1 + " and " + solution2 + " and "
                    + solution3 );
            Console.WriteLine( "\n" );
        }

        private static void testPoly4( Polynomial poly )
        {
            double solution1 = Solve.solve( poly, 1, -10, +10, 200000 );
            double solution2 = Solve.solve( poly, 2, -10, +10, 200000 );
            double solution3 = Solve.solve( poly, 3, -10, +10, 200000 );
            double solution4 = Solve.solve( poly, 4, -10, +10, 200000 );
            Console.WriteLine( "Solutions of " + poly + " = 0:\n" );
            Console.WriteLine( "   " + solution1 + " and " + solution2 + " and "
                    + solution3 + " and " + solution4 );
            Console.WriteLine( "\n" );
        }
    }

}

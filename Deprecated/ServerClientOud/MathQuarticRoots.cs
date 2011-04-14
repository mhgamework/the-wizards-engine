using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public struct MathQuarticRoots
    {
        public float Root1;
        public float Root2;
        public float Root3;
        public float Root4;

        public static MathQuarticRoots SolveQuartic( float a, float b, float c, float d )
        {
            float a0, a1, a2, a3;
            float T1, T2, T3, T4, T5;
            float R1, R2, R3, R4, R5;
            float r1, r2, r3, r4;
            a3 = a;
            a2 = b;
            a1 = c;
            a0 = d;

            T1 = -a3 / 4;
            T2 = ( a2 * a2 ) - 3 * a3 * a1 + 12 * a0;
            T3 = ( 2 * a2 * a2 * a2 - 9 * a3 * a2 * a1 + 27 * a1 * a1 + 27 * a3 * a3 * a0 - 72 * a2 * a0 ) / 2;
            T4 = ( -a3 * -a3 * -a3 + 4 * a3 * a2 - 8 * a1 ) / 32;
            T5 = ( 3 * a3 * a3 - 8 * a2 ) / 48;

            R1 = sqrt( T3 * T3 - T2 * T2 * T2 );
            R2 = cubic_root( T3 + R1 );
            R3 = ( 1 / 12 ) * ( T2 / R2 + R2 );
            R4 = sqrt( T5 + R3 );
            R5 = 2 * T5 - R3;
            R6 = T4 / R4;

            r1 = T1 - R4 - sqrt( R5 - R6 );
            r2 = T1 - R4 + sqrt( R5 - R6 );
            r3 = T1 + R4 - sqrt( R5 + R6 );
            r4 = T1 + R4 + sqrt( R5 + R6 );




            /*function v = cubic_root(u)
            if imag(u) == 0 & real(u) < 0
             v = -(abs(u)^(1/3));
            else
             v = u^(1/3);
            end*/

        }
    }
}

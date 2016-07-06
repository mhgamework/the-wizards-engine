using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;

namespace MHGameWork.TheWizards.DualContouring.QEFs
{
    public class IterativeQefCalculator : IQefCalculator
    {

        /// <summary>
        ///     WARNING MIGHT NOT BE THREAD SAFE
        /// </summary>
        /// <param name="normals"></param>
        /// <param name="posses"></param>
        /// <param name="preferredPosition"></param>
        /// <returns></returns>
        public Vector3 CalculateMinimizer(Vector3[] normals, Vector3[] posses, int numIntersections,Vector3 preferredPosition)
        {

            var curr = preferredPosition;
            var err = 0f;

            for (int j = 0; j < 1000; j++)
            {
                err = 0;
                //Debug.Log("Start");
                var meanOnplane = new Vector3();

                for (int i = 0; i < numIntersections; i++)
                {
                    // Project on plane
                    var planePos = posses[i];
                    var planeN = normals[i];
                    var relCurr = curr - planePos;
                    var offsetAlongNormal = Vector3.Dot(relCurr, planeN);
                    var onPlane = curr + offsetAlongNormal * -planeN;
                    err += (onPlane - curr).LengthSquared();
                    meanOnplane += onPlane;
                }
                if (err < 0.0001f) break;

                //Debug.Log(err);
                meanOnplane /= numIntersections;
                curr += (meanOnplane - curr) * 0.7f; // Found this number on some blogpost
                //curr = new Vector3(0,0,0);

            }
            if (err >= 0.001f)
            {
                int a = 5;
            }


            return curr;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Raycast
{
    /// <summary>
    /// Alternative method
    /// </summary>
    /// <typeparam name="T">The type of the objects being raycasted</typeparam>
    public interface IRaycastable<T> where T : class
    {
        RaycastResult<T> Raycast( Ray ray );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the objects being raycasted</typeparam>
    /// <typeparam name="U">The type of the raycast result returned</typeparam>
    public interface IRaycastable<T, U>
        where T : class
        where U : RaycastResult<T>
    {
        U Raycast( Ray ray );
    }

}

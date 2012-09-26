using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Raycast
{
    //[Obsolete("Better don't use unless your very sure of the raycasting algorithm")]
    /// <summary>
    /// NOTE: This looks like it is useless
    /// </summary>
    public interface IRaycastResult : IComparable<IRaycastResult>
    {
        float Distance { get;}
        bool IsHit { get;}
        bool IsCloser( IRaycastResult other );

    }
}

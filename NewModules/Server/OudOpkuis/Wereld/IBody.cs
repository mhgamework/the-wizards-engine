using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;

namespace MHGameWork.TheWizards.Server.Wereld
{

    public interface IBody : IHolderElement
    {
        Vector3 Positie
        { get; set;}
        Vector3 Scale
        { get; set;}
        Matrix Rotatie
        { get; set;}
        Quaternion RotatieQuat
        { get; set; }

        NxActor Actor
        { get; set;}
    }

}

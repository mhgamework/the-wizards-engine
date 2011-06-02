using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    /// <summary>
    /// This actually implements two snappers: A to B and B to A
    /// </summary>
    public interface ISnapObjectSnapper
    {
        Type SnapObjectTypeA { get; }
        Type SnapObjectTypeB { get; }

        void SnapAToB(object A, object B, Transformation transformationB, List<Transformation> outTransformations);
        void SnapBToA(object A, object B, Transformation transformationA, List<Transformation> outTransformations);


    }
}

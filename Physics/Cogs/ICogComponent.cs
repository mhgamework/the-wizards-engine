using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Cogs
{
    public interface ICogComponent
    {
        Actor Actor { get; set; }

        void PlaceAtHit(RaycastHit hit);
        void IncreaseSize();
        void DecreaseSize();

        bool ContainsShape(Shape shape);

        void DisposeActors();
    }

}

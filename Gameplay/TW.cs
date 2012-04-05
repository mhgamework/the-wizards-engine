using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards
{
    public static class TW
    {
        public static DX11Game Game { get; set; }
        public static ModelContainer.ModelContainer Model { get; set; }
        public static PhysicsEngine PhysX { get; set; }
        public static StillDesign.PhysX.Scene Scene { get; set; }
    }
}

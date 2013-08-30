using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Pro-ness terrain in cube shape: supports elevated and non-elevated state :)
    /// </summary>
    [ModelObjectChanged]
    public class TerrainCube : EngineModelObject, IPhysical
    {
        public TerrainCube()
        {
            Physical = new Physical();
        }

        public bool Elevated { get; set; }

        public Physical Physical { get; set; }

        public static int GetCellSize() { return 20; }

        public void UpdatePhysical()
        {
            var pos = Physical.GetPosition();

            var height = Elevated ? 20 : 0;
            height = 20;
            Physical.Visible = Elevated;

            Physical.ObjectMatrix = Matrix.Translation(0, -GetCellSize() / 2, 0);

            Physical.WorldMatrix = Physical.WorldMatrix * Matrix.Translation(-pos) *
                                   Matrix.Translation(pos.TakeXZ().ToXZ(height));

            Physical.Mesh = UtilityMeshes.CreateMeshWithTexture(GetCellSize() / 2, TW.Assets.LoadTexture("RTS\\plank-diffuse-seamless.jpg"));

        }



    }
}

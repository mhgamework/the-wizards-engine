using System;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// TODO: rename InfestationType
    /// </summary>
    public class InfestationVoxelType : GameVoxelType
    {
        public InfestationVoxelType()
            : base("InfestationVoxelType")
        {
            ColoredBaseMesh = false;
            datavalueMeshes[0] = createColoredMesh(Color.FromArgb(5, 5, 5));
            datavalueMeshes[1] = createColoredMesh(Color.FromArgb(5, 5, 5));
            datavalueMeshes[2] = createColoredMesh(Color.FromArgb(20, 20, 20));
            datavalueMeshes[3] = createColoredMesh(Color.FromArgb(25, 25, 25));
            datavalueMeshes[4] = createColoredMesh(Color.FromArgb(50, 50, 50));

        }
        public override void Tick(IVoxelHandle handle)
        {


            if (handle.Seeder.NextFloat(0, 100) > 1) return;
            randomizeColor(handle);
            var possible = handle.Get8Connected().Where(isInfecteable).ToArray();
            if (possible.Length == 0) return;
            var i = handle.Seeder.NextInt(0, possible.Length - 1);

            var target = possible[i];
            if (target.Data.MagicLevel > 10) return;
            target.Data.MagicLevel--;
            if (target.Data.MagicLevel < 0)
            {
                target.Data.MagicLevel = 0;
                InfestVoxel(target);
            }
        }

        private static void randomizeColor(IVoxelHandle handle)
        {
            if (handle.Seeder.NextFloat(0, 100) > 1) return;
            var c = handle.Seeder.NextInt(1, 4);
            if (handle.Data.DataValue == c) return;
            handle.Data.DataValue = handle.Seeder.NextInt(1, 4);
            handle.GetInternalVoxel().World.NotifyVoxelChanged(handle.GetInternalVoxel()); // Cheat!

        }

        public void InfestVoxel(IVoxelHandle target)
        {
            if (target.Type == Monument) return;
            var data = new InfestationData()
                {
                    OriginalType = target.Type,
                    OriginalDataValue = target.Data.DataValue
                };
            target.ChangeType(Infestation);
            target.Data.Infestation = data;
        }

        private bool isInfecteable(IVoxelHandle arg)
        {
            if (arg == null) return false;
            if (arg.Type == null) return false;
            if (arg.Type is InfestationVoxelType) return false;
            if (arg.Type == Air) return false;
            return true;
        }

        public void CureInfestation(IVoxelHandle v)
        {
            if (v.Type != Infestation) throw new InvalidOperationException();

            var data = v.Data.Infestation;

            v.ChangeType(data.OriginalType ?? Land);
            v.Data.DataValue = data.OriginalDataValue;

            v.Data.MagicLevel = 10;

        }


        public class InfestationData
        {
            public IGameVoxelType OriginalType;

            public int OriginalDataValue;

            public static InfestationData Emtpy
            {
                get { return new InfestationData(); }
            }
        }
    }
}
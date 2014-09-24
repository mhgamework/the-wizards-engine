using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.UI;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Responsible for collecting inputs, and applying collected inputs
    /// (time decoupling of calling inputs from executing them )
    /// </summary>
    public class UserInputService
    {
        private List<NumberInput> pendingMonumentInputs = new List<NumberInput>();

        public void SendMonumentRangeInput(IVoxelHandle monumentVoxel, int value)
        {
            pendingMonumentInputs.Add(new NumberInput(monumentVoxel.GetInternalVoxel().Coord, value));
        }

        public Inputs BuildInputs()
        {
            var ret =  new Inputs();
            ret.MonumentRangeInputs = pendingMonumentInputs.ToArray();
            return ret;
        }

        public void ApplyInputs(Model.World world, Inputs inputs)
        {
            foreach (var m in inputs.MonumentRangeInputs)
            {
                var v = world.GetVoxel(m.Coord);
                if (!(v.Type is MonumentType))
                    continue; // Ignore input because it arrived to late or is invalid
                ((MonumentType)v.Type).ApplyMonumentRangeInput(v, m.Value);
            }
        }

        [Serializable]
        public class Inputs
        {
            public NumberInput[] MonumentRangeInputs;
        }
        [Serializable]
        public struct NumberInput
        {
            public Point2 Coord;
            public int Value;

            public NumberInput(Point2 coord, int value)
            {
                Coord = coord;
                Value = value;
            }
        }
    }


}
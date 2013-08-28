﻿using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Windsor;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class GenerationSourcePart : EngineModelObject, IPhysical
    {
        #region Injection
        public GenerationPart GenerationPart { get; set; }
        public Physical Physical { get; set; }
      
        #endregion

        public IMesh EmptyMesh { get; set; }
        public IMesh FullMesh { get; set; }

        public void UpdatePhysical()
        {
        }

        public GenerationSourcePart()
        {
        }

        public void SimulateGeneration()
        {
            GenerationPart.SimulateResourceGeneration();

            Physical.Mesh = GenerationPart.HasResource ? FullMesh : EmptyMesh;


        }
    }
}
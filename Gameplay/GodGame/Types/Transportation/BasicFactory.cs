using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Generic;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Services;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation
{
    public class BasicFactory : ITickable
    {
        public float ProcessingTime = 5;

        public ItemType[] Input;
        public ItemType[] Output;

        public bool IsWorking;

        public float WorkCompleteAt;

        /// <summary>
        /// 0..1
        /// </summary>
        public float Efficiency;

        public float EfficiencySpeedMultiplier = 5; // Speed multiplier at max efficiency
        public float EfficiencyWorkOperationsToMax = 10; // Number of complete work operations to reach max efficiency. (could change this to a time metric instead)
        public float EfficiencyTimeToZero = 5; // Time from max to zero efficiency when idle

        private readonly IVoxel handle;

        public BasicFactory(IVoxel handle)
        {
            this.handle = handle;
        }

        public void Tick()
        {
            if (IsWorking)
            {
                if (!doWork()) return;
                Efficiency += 1f / EfficiencyWorkOperationsToMax;
                if (Efficiency > 1) Efficiency = 1;
            }
            else if (tryStartWork()) return;
            else
            {
                Efficiency -= TW.Graphics.Elapsed * EfficiencyTimeToZero;
                if (Efficiency < 0) Efficiency = 0;
            }


        }

        private bool tryStartWork()
        {
            if (!TransportationService.CanTakeItemsFromNearbyWarehouses(handle, Input)) return false;
            TransportationService.TakeItemsFromNearbyWarehouses(handle, Input);
            IsWorking = true;
            WorkCompleteAt = MathHelper.Lerp(ProcessingTime, ProcessingTime * EfficiencySpeedMultiplier, Efficiency) + TW.Graphics.TotalRunTime;
            return true;
        }

        private bool doWork()
        {
            if (TW.Graphics.TotalRunTime < WorkCompleteAt) return false;
            if (!TransportationService.CanStoreItemsInNearbyWarehouses(handle, Output)) return false;
            TransportationService.MoveItemsIntoNearbyWarehouse(handle, Output);
            IsWorking = false;
            return true;
        }
    }
}
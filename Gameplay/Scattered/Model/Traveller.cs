using System;
using System.Collections.Generic;
using SlimDX;
using System.Linq;
using MHGameWork.TheWizards.SkyMerchant._Engine;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Represents something moving from island to island
    /// </summary>
    public class Traveller
    {
        public Traveller()
        {
            DetermineDestinationAction = () => null;
            Inventory = new Inventory();
            OnReachIsland = a => { };
            Type = new TravellerType() {Name = "Unknown"};
        }
        public BridgePosition BridgePosition { get; set; }
        public TravellerType Type { get; set; }

        public Island Destination { get { return PlannedPath.With(i => i.Last()); } }

        public Func<Island> DetermineDestinationAction { get; set; }

        public Inventory Inventory { get; private set; }

        public Island[] PlannedPath { get; set; }

        public bool IsAtIsland(Island island)
        {
            //TODO: use Island property
            if (BridgePosition.Percentage < 0.001f && BridgePosition.Start == island) return true;
            if (BridgePosition.Percentage > 0.999f && BridgePosition.End == island) return true;
            return false;
        }

        /// <summary>
        /// Returns null when between islands, otherwise the island it is on
        /// </summary>
        public Island Island
        {
            get
            {
                if (BridgePosition.Percentage < 0.001f) return BridgePosition.Start;
                if (BridgePosition.Percentage > 0.999f) return BridgePosition.End;
                return null;
            }
        }

        /// <summary>
        /// Called every frame when this traveller is at an island and not on a bridge
        /// </summary>
        public Action<Island> OnReachIsland { get; set; }

        /// <summary>
        /// The in-game way to cleanly destroy an traveller at any frame
        /// </summary>
        public void Destroy()
        {
            PlannedPath = null;
        }
    }

    public struct BridgePosition
    {
        public Island Start;
        public Island End;
        public float Percentage;

        public BridgePosition(Island start, Island end, float percentage)
        {
            Start = start;
            End = end;
            Percentage = percentage;
        }

        public Vector3 CalculateActualPositon()
        {
            return Vector3.Lerp(Start.Position, End.Position, Percentage);
        }
    }
}
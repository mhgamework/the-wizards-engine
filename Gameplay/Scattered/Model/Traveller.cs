using System;
using SlimDX;

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
        }
        public BridgePosition BridgePosition { get; set; }

        public Island Destination { get; set; }

        public Func<Island> DetermineDestinationAction { get; set; }
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
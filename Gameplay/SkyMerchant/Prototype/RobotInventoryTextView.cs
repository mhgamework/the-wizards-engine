using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    /// <summary>
    /// Responsible for displaying the robot's inventory contents onscreen
    /// </summary>
    public class RobotInventoryTextView
    {
        private readonly RobotPlayerPart robot;


        public RobotInventoryTextView(RobotPlayerPart robot)
        {
            this.robot = robot;
        }

        public string GenerateText()
        {
            var types = robot.Items.Select(i => i.Type).Distinct();
            var counts = types.ToDictionary(t => t, t => robot.Items.Count(i => i.Type == t));

            var text = "";
            foreach (var t in types)
            {
                text += t.Name + ": " + counts[t] + "\n";
            }
            return text;
        }
    }
}
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

        private Textarea area = new Textarea();

        public RobotInventoryTextView(RobotPlayerPart robot)
        {
            this.robot = robot;
            area.Position = new Vector2(5, 5);
            area.Size = new Vector2(100, 100);
        }

        public void Update()
        {
            var types = robot.Items.Select(i => i.Type).Distinct();
            var counts = types.ToDictionary(t => t, t => robot.Items.Count(i => i.Type == t));

            var text = "";
            foreach (var t in types)
            {
                text += t.Name + ": " + counts[t] + "\n";
            }

            area.Text = text;
        }

    }
}
﻿using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class PrototypeUserInterface
    {
        private RobotInventoryTextView inventory;
        private Rendering2DComponentsFactory rendering;
        private RobotPlayerPart robot;


        private Textarea textarea;
        
        public PrototypeUserInterface(RobotInventoryTextView inventory, Rendering2DComponentsFactory rendering, LocalPlayer player)
        {
            this.inventory = inventory;
            this.rendering = rendering;
            robot = player.RobotPlayerPart;

            textarea = rendering.CreateTextArea();

            textarea.Position = new Vector2(5,5);
            textarea.Size = new Vector2(300,300);
        }

        public void Update()
        {
            textarea.Text = "";
            textarea.Text += "Robot inventory: " + "\n";
            textarea.Text += inventory.GenerateText() + "\n\n";

            textarea.Text += "Robot Health: " + robot.Health.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTSTestCase1.Inputting
{
    [ModelObjectChanged]
    public class InputFactory : EngineModelObject
    {
        private Dictionary<Key, UserButtonEvent> buttonsKey = new Dictionary<Key, UserButtonEvent>();
        private Dictionary<string, UserButtonEvent> buttonsName = new Dictionary<string, UserButtonEvent>();

        private UserButtonEvent leftMouseButton;

        public InputFactory()
        {
            addButton("moveForward", Key.W);
            addButton("moveBackward", Key.S);
            addButton("moveStrafeLeft", Key.A);
            addButton("moveStrafeRight", Key.D);
            addButton("moveJump", Key.Space);
            addButton("use", Key.F);
            setLeftMouse("attack");
        }

        private void addButton(string use, Key p1)
        {
            var b = new UserButtonEvent();

            buttonsKey.Add(p1, b);
            buttonsName.Add(use, b);


        }

        private void setLeftMouse(string attack)
        {
            leftMouseButton = new UserButtonEvent();
            buttonsName.Add(attack, leftMouseButton);
        }

        public UserButtonEvent GetButton(string identifier)
        {
            if (!buttonsName.ContainsKey(identifier))
                throw new InvalidOperationException("This kind of button is not yet predefined!");

            return buttonsName[identifier];
        }

        public void Update()
        {
            foreach (var pair in buttonsKey)
            {
                pair.Value.SetState(TW.Graphics.Keyboard.IsKeyDown(pair.Key));
            }
            if (leftMouseButton != null)
                leftMouseButton.SetState(TW.Graphics.Mouse.LeftMousePressed);
        }
    }
}

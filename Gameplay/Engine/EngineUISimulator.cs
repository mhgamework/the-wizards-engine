using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.VSIntegration;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Simulates the engine ui (this is pure al logic since it combines all the features that make up the engine)
    /// </summary>
    public class EngineUISimulator : ISimulator
    {
        private Textarea area;
        private Stack<TextMenu<Action>> menuStack = new Stack<TextMenu<Action>>();
        private TextMenu<Action> mainMenu;


        private PersistenceUI persistence = new PersistenceUI();
        public EngineUISimulator()
        {
            area = new Textarea();
            area.Position = new Vector2(0, 0);
            area.Size = new Vector2(200, 600);

            mainMenu = createMainMenu();
        }
        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Backspace))
            {
                if (menuStack.Count == 0)
                    menuStack.Push(mainMenu);
                else
                    menuStack.Pop();
            }

            if (menuStack.Count == 0)
            {
                area.Text = "";
                return;
            }
            var currentMenu = menuStack.Peek();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.DownArrow))
                currentMenu.MoveDown();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.UpArrow))
                currentMenu.MoveUp();




            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Return))
                if (currentMenu.SelectedItem != null) currentMenu.SelectedItem();

            area.Text = currentMenu.generateText();

        }


        private TextMenu<Action> createMainMenu()
        {
            var ret = new TextMenu<Action>();
            ret.AddItem("Debug", delegate { new VSDebugAttacher().AttachToVisualStudio(); });
            ret.AddItem("Load/Save", showPersistence);
            ret.AddItem("Exit", delegate { TW.Graphics.Exit(); });

            return ret;

        }

        private void showPersistence()
        {
            menuStack.Push(persistence.CreateMenu());
        }
    }
}

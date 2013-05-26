using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.VSIntegration;
using SlimDX;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Simulates the engine ui (this is pure AL logic since it combines all the features that make up the engine)
    /// </summary>
    public class EngineUISimulator : ISimulator
    {

        private Stack<TextMenu<Action>> menuStack = new Stack<TextMenu<Action>>();
        private TextMenu<Action> mainMenu;


        private PersistenceUI persistence = new PersistenceUI();

        private Data data = TW.Data.GetSingleton<Data>();

        private Textarea area;

        public EngineUISimulator()
        {
            area = data.Area;



            mainMenu = createMainMenu();

            if (!data.ConsoleCreated)
                new EngineUIConsole();
            data.ConsoleCreated = true;
        }


        public void Simulate()
        {
            if (data.Area == null)
            {
                area = new Textarea();
                area.Position = new Vector2(0, 0);
                area.Size = new Vector2(200, 600);
                data.Area = area;

            }
            updateErrorArea();

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

        private void updateErrorArea()
        {
            if (data.ErrorArea == null)
            {
                data.ErrorArea = new Textarea()
                    {
                        Position = new Vector2(10, 600 - 10 - 60),
                        Size = new Vector2(800 - 10 * 2, 60),
                        BackgroundColor = new Color4(1, 1, 0, 0)
                    };
            }

            data.ErrorArea.Visible = TW.Debug.LastException != null;
            if (TW.Debug.LastException == null) return;
            data.ErrorArea.Text = TW.Debug.LastException.Message;
            if (!string.IsNullOrEmpty(TW.Debug.LastExceptionExtra))
                data.ErrorArea.Text += "\n" + TW.Debug.LastExceptionExtra;

            data.ErrorArea.Text += "\n" + new string(TW.Debug.LastException.StackTrace.TakeWhile(c => c != '\n').ToArray());

            // Hack to stop serializer from failing!
            data.ErrorArea.Text += "\n";


            if (TW.Graphics.Keyboard.IsKeyPressed(Key.L))
            {
                var attacher = new VSDebugAttacher();
                attacher.SelectExceptionLine(TW.Debug.LastException);
            }

        }


        private TextMenu<Action> createMainMenu()
        {
            var ret = new TextMenu<Action>();
            ret.AddItem("Debug", delegate { new VSDebugAttacher().AttachToVisualStudio(); });
            ret.AddItem("Load/Save", showPersistence);
            ret.AddItem("Clean", cleanData);
            ret.AddItem("Reset", resetData);
            ret.AddItem("Select test [Press F5]", delegate { });
            ret.AddItem("Exit DONT PRESS", OnExit);

            return ret;

        }

        private static void OnExit()
        {

            DI.Get<Persistence.EngineStatePersistor>().SaveEngineState();
            TW.Graphics.Exit();
        }

        private void cleanData()
        {
            var test = TW.Data.GetSingleton<TestingData>();
            resetData();
            TW.Data.Objects.Add(test);
        }

        private void resetData()
        {
            // Clear all objects
            TW.Data.Objects.Clear();
            TW.Debug.NeedsReload = true;
        }

        private void showPersistence()
        {
            menuStack.Push(persistence.CreateMenu());
        }

        [ModelObjectChanged]
        public class Data : EngineModelObject
        {
            public bool ConsoleCreated { get; set; }
            public Textarea Area { get; set; }
            public Textarea ErrorArea { get; set; }
        }
    }
}

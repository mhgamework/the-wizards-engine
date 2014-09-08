using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class DeveloperConsoleUI
    {
        private readonly ICommandProvider provider;
        private const int visibleLogLines = 10;
        private Textarea textarea = new Textarea();

        private DirectInputTextBox textBox = new DirectInputTextBox();
        private Queue<string> lines = new QuickGraph.Collections.Queue<string>();

        public string Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public DeveloperConsoleUI(ICommandProvider provider)
        {
            this.provider = provider;
            for (int i = 0; i < visibleLogLines; i++)
            {
                WriteLine("");
            }
            Hide();

            textarea.BackgroundColor = new Color4(0.5f, 0.5f, 0.5f, 0.5f);
            textarea.Size = new Vector2(TW.Graphics.Form.Form.ClientSize.Width, 200);
        }
        public void Show()
        {
            textarea.Visible = true;
        }
        public void Hide()
        {
            textarea.Visible = false;

            textBox.Text = "";
        }

        public void Update()
        {
            if (!textarea.Visible) return;

            textBox.ProcessUserInput(TW.Graphics.Keyboard);

            updateTextarea();

            tryExecuteCommand();

        }

        private void tryExecuteCommand()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.Return)) return;
            if (Text == "") return;

            var output = provider.Execute(Text);

            output.Split('\n').ForEach(WriteLine);

            Text = "";
        }

        private void updateTextarea()
        {
            textarea.Text = "";
            textarea.Text += lines.Reverse().Take(visibleLogLines).Reverse().Aggregate((acc, el) => acc + "\n" + el) + "\n";

            textarea.Text += ">>> " + textBox.Text;
        }


        public void WriteLine(string line)
        {
            lines.Enqueue(line);
            if (lines.Count > 1000)
                lines.Dequeue();
        }
    }
}
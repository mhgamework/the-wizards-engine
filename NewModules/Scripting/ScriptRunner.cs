using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.Scripting
{
    /// <summary>
    /// This is a basic test implementation of a scriptrunner
    /// </summary>
    public class ScriptRunner : IXNAObject
    {
        private List<IStateScript> stateScripts = new List<IStateScript>();
        private IXNAGame game;

        public ScriptRunner(IXNAGame game)
        {
            this.game = game;
            game.AddXNAObject(this);

        }

        public void RunScript(IStateScript script)
        {
            stateScripts.Add(script);

            SetScriptLayerScope();
            script.Init();
        }

        public void Initialize(IXNAGame _game)
        {
        }

        public void Render(IXNAGame _game)
        {
            SetScriptLayerScope();

            for (int i = 0; i < stateScripts.Count  ; i++)
            {
                var s = stateScripts[i];
                s.Draw();
            }
        }

        public void Update(IXNAGame _game)
        {
            SetScriptLayerScope();

            for (int i = 0; i < stateScripts.Count; i++)
            {
                var s = stateScripts[i];
                s.Update();
            }
        }

        private void SetScriptLayerScope()
        {
            ScriptLayer.Game = game;
        }
    }
}

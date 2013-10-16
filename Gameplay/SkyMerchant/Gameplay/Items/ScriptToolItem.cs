using System;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay.Items
{
    /// <summary>
    /// Allows binding a script (specified on construction) to a world object
    /// </summary>
    public class ScriptToolItem : IHotbarItem
    {
        private readonly IScriptType scriptType;
        private readonly ILocalPlayer player;

        public ScriptToolItem(IScriptType scriptType , ILocalPlayer player)
        {
            this.scriptType = scriptType;
            this.player = player;
        }

        public string Name { get { return scriptType.Name; } }
        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
            if (TW.Graphics.Mouse.LeftMouseJustPressed)tryAttachScript();
        }

        private void tryAttachScript()
        {
            if (!player.TargetedObjects.Any()) return;
            var obj = player.TargetedObjects.First();

            throw new NotImplementedException();
            //obj.Scripts.Add(scriptType.CreateInstance());

        }
    }
}
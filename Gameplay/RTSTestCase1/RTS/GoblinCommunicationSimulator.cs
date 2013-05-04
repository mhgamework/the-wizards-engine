﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTS.Commands;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS
{
    [PersistanceScope]
    public class GoblinCommunicationSimulator : ISimulator
    {
        private Goblin goblin = null;
        private IGoblinCommand command = null;

        private PlayerRTS player = TW.Data.GetSingleton<PlayerRTS>();

        private Data data = TW.Data.GetSingleton<Data>();

        public GoblinCommunicationSimulator()
        {
            if (data.Textarea == null)
                data.Textarea = new Textarea();

            data.Textarea.Position = new Vector2(30, 30);
            data.Textarea.Size = new Vector2(400, 400);
        }

        public void Simulate()
        {
            processFKey();

            if (goblin != null)
                if (Vector3.Distance(goblin.Position, TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx()) > 8)
                    deselectGoblin();
            //TODO: optimize, this changes the text every frame
            if (goblin == null)
                clearTalkScreen();
            else
                updateTalkScreen();
        }

        private void processFKey()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.F)) return;

            if (goblin == null)
                trySelectGoblin();
            else
                deselectGoblin();

        }

        private void deselectGoblin()
        {
            var cmd = goblin.get<GoblinCommandState>().CurrentCommand as TalkingCommand;
            goblin.get<GoblinCommandState>().CurrentCommand = command;
            goblin = null;
        }

        private void updateTalkScreen()
        {
            data.Textarea.Visible = true;
            data.Textarea.Text = String.Format("Hey! I'm currently {0}!", command.Description);
            data.Textarea.Text += "\n\n";//Hier zou hij altijd moeten komen
            data.Textarea.Text += "1. Idle\n";
            data.Textarea.Text += "2. Follow\n";
            data.Textarea.Text += "3. Fetch items of the type I am holding to my position\n";
            data.Textarea.Text += "4. Fetch items of the type I am holding from my position\n";
            checkKeyIdle();
            checkKeyFollow();
            checkKeyAdvancedFetchTo();
            checkKeyAdvancedFetchFrom();
        }



        private void checkKeyIdle()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.D1)) return;
            command = new GoblinIdleCommand();
        }

        private void checkKeyFollow()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.D2)) return;
            command = new GoblinFollowCommand();
        }
        /*private void checkKeyFetch()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.D3)) return;
            if (player.Holding == null) return;
            var target = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            target.Y = 0.3f;
            command = new GoblinFetchCommand()
                {
                    ResourceType = player.Holding.Type,
                    TargetPosition = target
                };
        }*/

        private Goblin lastFetchTo;
        private Vector3 fetchToPosition;
        private Goblin lastFetchFrom;
        private Vector3 fetchFromPosition;

        private void checkKeyAdvancedFetchTo()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.D3)) return;
            if (player.Holding == null) return;
            var target = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            target.Y = 0.3f;
            if (lastFetchFrom == goblin)
            {    command = new GoblinAdvancedFetchCommand()
                              {
                                  ResourceType = player.Holding.Type,
                                  TargetPosition = target,
                                  SourcePosition = fetchFromPosition
                              };
                return;
            }
            fetchToPosition = target;
            lastFetchTo = goblin;
        }
        private void checkKeyAdvancedFetchFrom()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.D4)) return;
            if (player.Holding == null) return;
            var target = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            target.Y = 0.3f;

            if (lastFetchTo == goblin)
            {
                command = new GoblinAdvancedFetchCommand()
                {
                    ResourceType = player.Holding.Type,
                    TargetPosition = fetchToPosition,
                    SourcePosition = target
                };
                return;
            }
            fetchToPosition = target;
            lastFetchFrom = goblin;
        }

        private void clearTalkScreen()
        {
            data.Textarea.Visible = false;
        }

        private void trySelectGoblin()
        {
            var obj = TW.Data.GetSingleton<Engine.WorldRendering.World>()
                        .Raycast(TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay(), e => e.Tag is Goblin);

            if (!obj.IsHit) return;
            if (obj.Distance > 5) return;
            if (obj.Object == null) return;

            var ent = obj.Object;
            Goblin found = listGoblinsWithEntity(ent).FirstOrDefault();
            if (found == null) return;

            command = found.get<GoblinCommandState>().CurrentCommand ?? new GoblinIdleCommand();
            found.get<GoblinCommandState>().CurrentCommand = new TalkingCommand();
            goblin = found;
        }

        private IEnumerable<Goblin> listGoblinsWithEntity(object e)
        {
            return TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().Where(o => o.get<Engine.WorldRendering.Entity>() == e);
        }

        [ModelObjectChanged]
        private class Data : EngineModelObject
        {
            public Textarea Textarea { get; set; }
        }
    }
}

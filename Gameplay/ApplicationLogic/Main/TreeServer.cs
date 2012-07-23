using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TreeGenerator.EngineSynchronisation;
using TreeGenerator.TreeEngine;

namespace MHGameWork.TheWizards.Main
{
    public class TreeServer
    {
        private Seeder seeder;
        private ServerTreeSyncer serverTreeSyncer;

        public void SetUp(IServerPacketManager packetManager)
        {
            seeder = new Seeder(123);


            serverTreeSyncer = new ServerTreeSyncer(packetManager);
        }

        public void Update(XNAGame game)
        {
            if (game.Keyboard.IsKeyPressed(Keys.A))
            {
                throw new NotImplementedException();
                /*serverTreeSyncer.AddTree(new EngineTree(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(10, 0, 10)),
                                                        0, 123, 456));*/
            }
            if (game.Keyboard.IsKeyPressed(Keys.F))
            {
                throw new NotImplementedException();
                Vector3 pos = game.SpectaterCamera.CameraPosition -
                              game.SpectaterCamera.CameraPosition.Y * Vector3.UnitY;
                /*serverTreeSyncer.AddTree(new EngineTree(pos,
                                                        0, 123, 456));*/
            }
            serverTreeSyncer.Update();
        }
    }
}

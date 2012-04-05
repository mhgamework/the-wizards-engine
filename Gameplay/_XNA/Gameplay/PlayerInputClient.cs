using System;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public class PlayerInputClient
    {
        private PlayerInputPacket currentPacket;
        private IntervalCaller sendInterval;
        private IClientPacketTransporter<PlayerInputPacket> inputTransporter;

        public const float UpdateRate = 1 / 20f;

        public float HorizontalLookAngle { get; set; }

        public PlayerInputClient(IClientPacketManager pm)
        {
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache + "\\PlayerInputClient" + (new Random()).Next(0, 10000) + "dll");
            var fact = gen.GetFactory<PlayerInputPacket>();
            gen.BuildFactoriesAssembly();

            inputTransporter = pm.CreatePacketTransporter("PlayerInputClientInput", fact, Networking.PacketFlags.UDP);

            sendInterval = new IntervalCaller(sendUpdatePacket, UpdateRate);
        }

        public void Update(IXNAGame game)
        {

            if (game.Keyboard.IsKeyDown(Keys.Z))
            {
                currentPacket.Input |= PlayerInputPacket.PlayerInput.Forward;
            }
            if (game.Keyboard.IsKeyDown(Keys.S))
            {
                currentPacket.Input |= PlayerInputPacket.PlayerInput.Backward;
            }
            if (game.Keyboard.IsKeyDown(Keys.Q))
            {
                currentPacket.Input |= PlayerInputPacket.PlayerInput.Left;
            }
            if (game.Keyboard.IsKeyDown(Keys.D))
            {
                currentPacket.Input |= PlayerInputPacket.PlayerInput.Right;
            }
            if (game.Keyboard.IsKeyPressed(Keys.Space))
            {
                currentPacket.Input |= PlayerInputPacket.PlayerInput.Jump;
            }

            sendInterval.Update(game.Elapsed);
        }

        private void sendUpdatePacket()
        {
            currentPacket.HorizontalLookAngle = HorizontalLookAngle;


            inputTransporter.Send(currentPacket);
            currentPacket.Input = PlayerInputPacket.PlayerInput.None;
        }
    }
}

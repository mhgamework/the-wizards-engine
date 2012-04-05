using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public class PlayerInputServer
    {
        private readonly PlayerController controller;
        private IClientPacketTransporter<PlayerInputPacket> inputTransporter;

        private PlayerInputPacket lastPacket;

        public PlayerInputServer(IClientPacketTransporter<PlayerInputPacket> inputTransporter, PlayerController controller)
        {
            this.controller = controller;
            this.inputTransporter = inputTransporter;
        }

        public void Update(float elapsed)
        {
            while (inputTransporter.PacketAvailable)
                lastPacket = inputTransporter.Receive();

            controller.HorizontalAngle = lastPacket.HorizontalLookAngle;
            
            

            if ((lastPacket.Input & PlayerInputPacket.PlayerInput.Forward) == PlayerInputPacket.PlayerInput.Forward)
                controller.DoMoveForward(elapsed);

            if ((lastPacket.Input & PlayerInputPacket.PlayerInput.Backward) == PlayerInputPacket.PlayerInput.Backward)
                controller.DoMoveBackwards(elapsed);

            if ((lastPacket.Input & PlayerInputPacket.PlayerInput.Left) == PlayerInputPacket.PlayerInput.Left)
                controller.DoStrafeLeft(elapsed);

            if ((lastPacket.Input & PlayerInputPacket.PlayerInput.Right) == PlayerInputPacket.PlayerInput.Right)
                controller.DoStrafeRight(elapsed);

            if ((lastPacket.Input & PlayerInputPacket.PlayerInput.Jump) == PlayerInputPacket.PlayerInput.Jump)
                controller.DoJump();
        }


        public static IServerPacketTransporter<PlayerInputPacket> CreateTransporter(IServerPacketManager pm)
        {
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache + "\\PlayerInputServer.dll");
            var fact = gen.GetFactory<PlayerInputPacket>();
            gen.BuildFactoriesAssembly();

            return pm.CreatePacketTransporter("PlayerInputClientInput", fact, Networking.PacketFlags.UDP);
        }
    }
}

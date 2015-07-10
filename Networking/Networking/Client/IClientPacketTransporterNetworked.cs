using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MHGameWork.TheWizards.Networking.Client
{
    public partial class ClientPacketManagerNetworked
    {
        public interface IClientPacketTransporterNetworked
        {
            string UniqueName
            {
                get;
            }

            /*PacketFlags Flags
            {
                get;
            }*/

            int NetworkID
            {
                get;
            }
            /*INetworkPacketFactory Factory
            {
                get;

            }
            */


            void SetNetworkID( int id );

            /*int WaitingReceiversCount
            {
                get;
                set;
            }*/

            /// <summary>
            /// Internal use only!
            /// Should be thread safe!
            /// </summary>
            void QueueReceivedPacket( BinaryReader br );
        }
    }
}

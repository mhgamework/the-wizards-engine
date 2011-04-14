using System.Net;
using System.Net.Sockets;
using System.Threading;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.Network;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.Game3DPlay.Core;
namespace MHGameWork.TheWizards.Common
{
    public class Communication : SpelObject
    {

        public Communication( SpelObject nParent )
            : base( nParent )
        {

            this.setUDPConn( new UDPConnection() );

            UDPConn.NetworkErrorAsync += mUDPConn_NetworkErrorAsync;
            UDPConn.PacketRecievedAsync += mUDPConn_PacketRecievedAsync;
        }

        public const short UDPPort = 5012;
        public const int ListenerPort = 5014;

        private UDPConnection mUDPConn;
        public UDPConnection UDPConn
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.mUDPConn; }
        }
        private void setUDPConn( UDPConnection value )
        {
            this.mUDPConn = value;
        }

        public enum Protocol
        {
            TCP = 1,
            UDP = 2
        }


        public enum ClientCommands : byte
        {
            Ping = 1,
            OnSuccessfulLogin = 2,
            OnUDPConnectionLinked = 3,
            EntityUpdate = 4,
            UpdateWorld = 5,
            UpdateTime = 6,
            SetGameClientData = 7,
            SetGameFilesList = 8,
            SetGameIniFileID = 9,
            //SignaleerVerandering 

            //Verandering 

            //DeltaSnapshot 

            //Time 








            //Reply's 
            PingReply = 100,
            GetGameFilesListCompleted = 101,
            GetTerrainsListCompleted = 102,
            GetCoreFilesListCompleted = 103,

            GetGameFileDataCompleted = 104

            //LoginReply 
            //LinkUDPConnectionReply 

            //LaadWereldReplyEntity 
            //LaadWereldReplyCompleted 

            //GetEntityReply 


            //GetModelReply 





            //ADMINGetGameFilesListReply 
            //ADMINAddGameFileReply 

            //ADMINGetModelListReply 
            //ADMINUpdateModelReply 











        }

        public enum ServerCommands : byte
        {
            Ping = 1,
            Login = 2,
            LinkUDPConnection = 3,
            GetGameFilesList = 4,
            GetTerrainsList = 5,
            GetCoreFilesList = 6,

            ApplyPlayerInput = 7,

            GetGameFileData = 8,


            TempShootShuriken = 9,

            SetBlockHeightMapData = 10,
            GetGameIniFileID = 11,

            //Login 
            //LinkUDPConnection 

            //LaadWereld 

            //GetEntity 

            //PlayerCommand 

            //UpdateAngles 



            //GetModel 






            //'ADMIN 
            //ADMINGetGameFilesList 
            //ADMINAddGameFile 

            //ADMINGetModelList 
            //ADMINUpdateModel 

            //ADMINPutStaticEntity 

















            //Reply's 
            PingReply = 100

        }


        // '' ''Private mScheduler As New SchedulerElement(Me) 
        // '' ''Public ReadOnly Property Scheduler() As SchedulerElement 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mScheduler 
        // '' '' End Get 
        // '' ''End Property 
        // '' ''Private Sub setScheduler(ByVal value As SchedulerElement) 
        // '' '' Me.mScheduler = value 
        // '' ''End Sub 





        public override void Dispose()
        {
            base.Dispose();
            this.UDPConn.Dispose();

        }



        protected virtual void OnUDPPacketRecieved( object sender, BaseConnection.PacketRecievedEventArgs e )
        {

        }


        // '' ''Private Sub mUDPConn_PacketRecieved(ByVal sender As Object, ByVal e As UDPConnection.PacketRecievedEventArgs) Handles mUDPConn.PacketRecieved 
        // '' '' Me.Scheduler.ScheduleAction(AddressOf Me.OnUDPPacketRecieved, New PacketRecievedEventArgs(e.Dgram, e.EP), 0) 
        // '' ''End Sub 

        private void mUDPConn_NetworkErrorAsync( object sender, Networking.BaseConnection.NetworkErrorEventArgs e )
        {

        }

        private void mUDPConn_PacketRecievedAsync( object sender, Networking.BaseConnection.PacketRecievedEventArgs e )
        {
            HoofdObj.Invoker.Invoke( OnUDPPacketRecieved, sender, e );
        }
    }
}
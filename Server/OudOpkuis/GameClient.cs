using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Server
{
    public class GameClient
    {
        private Network.ProxyClient proxy;
        private String temporaryLoginKey;
        private bool loggedIn;

        ServerMainNew main;

        private Entities.Player playerEntity;

        private Common.PlayerInputData lastPlayerInput;
        Vector3 cameraAngles;

        private string username;
        private Common.GameClientData gameClientData;


        public GameClient( Network.ProxyClient nProxy, ServerMainNew nMain )
        {
            proxy = nProxy;
            main = nMain;

            cameraAngles = new Vector3( 0, 0, 0 );
        }



        /* Public ReadOnly Property Proxy() As ProxyClient
             <System.Diagnostics.DebuggerStepThrough()> Get
                 Return Me.mProxy
             End Get
         End Property
         Private Sub setProxy(ByVal value As ProxyClient)
             Me.mProxy = value
         End Sub



         Public ReadOnly Property TemporaryLoginKey() As String
             <System.Diagnostics.DebuggerStepThrough()> Get
                 Return Me.mTemporaryLoginKey
             End Get
         End Property
         Public Sub setTemporaryLoginKey(ByVal value As String)
             Me.mTemporaryLoginKey = value
         End Sub*/

        public string GenerateLoginKey()
        {
            DateTime Now = DateTime.Now;
            Random rand = new Random( Now.Millisecond );

            return String.Format( "{0}-{1}-{2}-{3}-{4}-{5}-{6}", Now.Year, Now.Millisecond, Now.Minute, Now.Second, Now.Day, Now.Month, rand.Next( int.MinValue, int.MaxValue ) );
        }
        public string GetTemporaryLoginKey()
        {
            if ( !HasTemporaryLoginKey ) throw new Exception( "There is no login key" );
            return temporaryLoginKey;
        }
        public void ClearTemporaryLoginKey()
        {
            temporaryLoginKey = "";
        }


        public bool TryLogin( string nUsername, byte[] nPassword )
        {
            //Check database
            if ( true )
            {
                //Login succeeded
                username = nUsername;
                temporaryLoginKey = GenerateLoginKey();

                return true;
            }
            else
            {
                //Login failed
            }


        }

        public void OnLoginSuccesfull()
        {
            loggedIn = true;

            //proxy.SetGameFilesList( main.GameFileManager );


            LoadGameClientData( username );

            //Debug only:
            playerEntity = new Entities.Player( main );
            Wereld.ServerEntityHolder entH = Entities.Player.CreatePlayerEntity( playerEntity );
            main.Wereld.AddEntity( entH );
            gameClientData.PlayerEntityID = entH.ID;

            Proxy.ProxyGameClient.SetGameClientData( gameClientData );


        }

        public void LoadGameClientData( string username )
        {
            gameClientData = new MHGameWork.TheWizards.Common.GameClientData();


        }



        public void ApplyPlayerInput( Common.PlayerInputData input )
        {
            lastPlayerInput = input;
        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            if ( lastPlayerInput == null ) return;
            //Game3DPlay.Core.Elements.ProcessEventArgs nE = ServerMainNew.instance.ProcessEventArgs;





            Vector3 totalDisp = new Vector3();  //Total Displacement
            Vector3 localMov = new Vector3();  //Local Movement
            Vector3 worldMov = new Vector3();   //World Movement


            /*With Me.LastPlayerCommand
                If .Vooruit Then
                    LocalMov += New Vector3(0, 0, 1)
                End If
                If .Achteruit Then
                    LocalMov += New Vector3(0, 0, -1)
                End If
                If .StrafeLinks Then
                    LocalMov += New Vector3(-1, 0, 0)
                End If
                If .StrafeRechts Then
                    LocalMov += New Vector3(1, 0, 0)
                End If
                If .Jump Then
                    If Me.JumpTime = 0 Then Me.JumpTime = 1000
                Else
                    Me.JumpTime = 0
                End If
                If .Crouch Then
                    Me.LinkedPlayerEntity.Controller.Height = 0.2
                Else
                    Me.LinkedPlayerEntity.Controller.Height = 2
    '    WorldMov += New Vector3(0, -1, 0)
                End If

            End With*/
            //LocalDisp.Normalize()

            //if ( nE.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Up ) )
            //{
            //    localMov += new Vector3( 0, 0, 1 );
            //}
            //if ( nE.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Down ) )
            //{
            //    localMov += new Vector3( 0, 0, -1 );
            //}
            //if ( nE.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Left ) )
            //{
            //    cameraAngles.Y += 1 * e.Elapsed;
            //}
            //if ( nE.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Right ) )
            //{
            //    cameraAngles.Y -= 1 * e.Elapsed;
            //}

            if ( lastPlayerInput.MoveForward )
            { localMov += new Vector3( 0, 0, 1 ); }
            if ( lastPlayerInput.MoveBackwards )
            { localMov += new Vector3( 0, 0, -1 ); }
            if ( lastPlayerInput.StrafeLeft )
            { localMov += new Vector3( 1, 0, 0 ); }
            if ( lastPlayerInput.StrafeRight )
            { localMov += new Vector3( -1, 0, 0 ); }

            //worldMov += Vector3.Transform( localMov, Matrix.CreateFromYawPitchRoll( cameraAngles.Y, -cameraAngles.X, cameraAngles.Z ) );
            worldMov += Vector3.Transform( localMov, lastPlayerInput.Orientation );
            if ( worldMov.Length() != 0 ) worldMov.Normalize();

            /*If Me.LastPlayerCommand.Run Then
                WorldMov *= 5
            End If*/

            //if ( jumpTime > 0 )
            //{
            //    jumpTime -= e.Elapsed * 1000f;
            //    worldMov += new Vector3( 0, 1, 0 );
            //}


            totalDisp += worldMov;//CType(Me.HoofdObj, BaseMain).ClientSpeed
            totalDisp *= 10;
            //if ( playerEntity.body.Actor.getGlobalPosition().Y > -500 )
                totalDisp += new Vector3( 0, -9.81f, 0 );
            //else
            //   totalDisp = totalDisp;

            //if ( jumpTime <= 0 )
            //{
            //    jumpTime = 0;
            //    totalDisp += new Vector3( 0, -9.81f, 0 ); //CType(Me.HoofdObj, BaseMain).ClientGravity
            //}



            if ( playerEntity.body.Actor.getGlobalPosition().Y - 1.5f + totalDisp.Y < -500 )
            { totalDisp.Y = -( playerEntity.body.Actor.getGlobalPosition().Y - 1.5f - (-500) ); }






            uint collisionFlags;

            totalDisp *= 10f;

            playerEntity.controller.move( totalDisp * e.Elapsed, uint.MaxValue, 0.001f, out collisionFlags, 1.0f );

            //playerEntity.body.Actor.GlobalOrientation = Matrix.CreateFromYawPitchRoll( cameraAngles.Y, -cameraAngles.X, cameraAngles.Z );


            Vector3 forward = Vector3.Transform( new Vector3( 0, 0, 1 ), lastPlayerInput.Orientation );

            //forward = new Vector3( 1, 1, 1 );

            forward.Y = 0;
            forward.Normalize();

            float angle = (float)Math.Acos( forward.Z );
            if ( forward.X < 0 ) angle = -angle;

            playerEntity.body.Rotatie = Matrix.CreateRotationY( angle );









        }


        public bool HasTemporaryLoginKey { get { return temporaryLoginKey != ""; } }
        public Network.ProxyClient Proxy { get { return proxy; } }
        public bool LoggedIn { get { return loggedIn; } set { loggedIn = value; } }
        public Common.GameClientData GameClientData
        { get { return gameClientData; } }

        public Entities.Player PlayerEntity
        { get { return playerEntity; } }


        public Common.PlayerInputData LastPlayerInput
        { get { return lastPlayerInput; } }
    }
}
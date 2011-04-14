using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;
using ServerWereld = MHGameWork.TheWizards.Server.Wereld;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
    [Obsolete("This class has been reimplemented as ServerSyncedActor")]
    public class Body : IBody
    {
        private Vector3 positie;
        private Vector3 scale;
        private Matrix rotatie;

        private NxActor actor;

        public ClientEntityHolder clientEntHolder;

        public Common.Network.UpdateEntityPacket[] snapshots;
        public int firstElementTick;
        public int firstElementIndex;

        public const int SnapshotBufferLength = 100;
        public const int RenderDelay = 100;

        public Body()
        {
            positie = Vector3.Zero;
            scale = Vector3.One;
            rotatie = Matrix.Identity;

            snapshots = new MHGameWork.TheWizards.Common.Network.UpdateEntityPacket[ SnapshotBufferLength ];
            ClearBuffer();
        }


        public void AddEntityUpdate( int nTick, Common.Network.UpdateEntityPacket p )
        {
            /*if ( nTick < firstElementTick || nTick >= firstElementTick + snapshots.Length )
            {
                // nTick out of buffer range? should we reposition the firstElementTick or just dispose
                // the update?

                //Dispose for now
                return;
            }*/

            int temp = firstElementTick;

            /*firstElementTick = temp;
            firstElementIndex = TickToIndex( firstElementTick );*/

            if ( nTick == firstElementTick + snapshots.Length )
            {
                //Move the buffer right by one.
                firstElementTick++;
                firstElementIndex++;
                if ( firstElementIndex == snapshots.Length ) firstElementIndex = 0;

                //Put the new packet in the buffer. The packet at the old firstElementIndex is now replaced by p
                snapshots[ TickToIndex( nTick ) ] = p;

            }
            else if ( nTick > firstElementTick + snapshots.Length )
            {
                MoveBufferStartTickRight( nTick - ( firstElementTick + snapshots.Length - 1 ) );

                snapshots[ TickToIndex( nTick ) ] = p;
            }
            else if ( nTick < firstElementTick )
            {
                //Dispose the packet, out of date (do nothing)


            }
            else
            {
                //Packet is in range
                snapshots[ TickToIndex( nTick ) ] = p;
            }


            if ( firstElementIndex != TickToIndex( firstElementTick ) ) throw new Exception();



        }

        public void MoveBufferStartTickRight( int places )
        {
            if ( places > snapshots.Length ) places = snapshots.Length;

            int start = firstElementTick;
            int index = firstElementIndex;
            int end = firstElementTick + places;

            for ( int tick = start; tick < end; tick++ )
            {
                snapshots[ index ] = Common.Network.UpdateEntityPacket.Empty;
                index++;

                if ( index == snapshots.Length ) index = 0;
            }

            firstElementIndex = index;
            firstElementTick += places;


        }

        public void ClearBuffer()
        {
            for ( int i = 0; i < snapshots.Length; i++ )
            {
                snapshots[ i ] = Common.Network.UpdateEntityPacket.Empty;
            }
        }

        public int TickToIndex( int nTick )
        {
            return nTick % snapshots.Length;
        }


        void IHolderElement.SetEntityHolder( ClientEntityHolder nEntH )
        {
            clientEntHolder = nEntH;
        }

        void IHolderElement.Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            int time = e.Time - RenderDelay;
            if ( time < 0 ) return;
            int lastTick = (int)Math.Floor( time / ( e.HoofdObj.TickRate * 1000 ) );
            CalculateInterpolatedData( time, lastTick, e );
            //CalculateNonInterpolatedData( time, lastTick, e );



        }

        public void CalculateNonInterpolatedData( int nTime, int lastTick, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            // TickToIndex: dit gebruikt % en zou het serieus kunnen vertragen.


            Common.Network.UpdateEntityPacket startPacket = Common.Network.UpdateEntityPacket.Empty;
            Common.Network.UpdateEntityPacket endPacket = Common.Network.UpdateEntityPacket.Empty;

            int startTick = -1;
            //int endTick = -1;

            //Find start snapshot.

            int iTick = lastTick;
            int index;

            while ( iTick > firstElementTick )
            {
                index = TickToIndex( iTick );
                if ( !snapshots[ index ].IsEmpty() )
                {
                    startPacket = snapshots[ index ];
                    startTick = iTick;
                    break;
                }

                iTick--;
            }


           Positie =  startPacket.Positie;

           RotatieQuat = startPacket.RotatieQuat;




        }

        public void CalculateInterpolatedData( int nTime, int lastTick, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            // TickToIndex: dit gebruikt % en zou het serieus kunnen vertragen.


            Common.Network.UpdateEntityPacket startPacket = Common.Network.UpdateEntityPacket.Empty;
            Common.Network.UpdateEntityPacket endPacket = Common.Network.UpdateEntityPacket.Empty;

            int startTick = -1;
            int endTick = -1;

            //Find start snapshot.

            int iTick = lastTick;
            int index;

            while ( iTick > firstElementTick )
            {
                index = TickToIndex( iTick );
                if ( !snapshots[ index ].IsEmpty() )
                {
                    startPacket = snapshots[ index ];
                    startTick = iTick;
                    break;
                }

                iTick--;
            }



            //Find end snapshot.

            iTick = lastTick + 1;

            while ( iTick < firstElementTick + snapshots.Length )
            {
                index = TickToIndex( iTick );
                if ( !snapshots[ index ].IsEmpty() )
                {
                    endPacket = snapshots[ index ];
                    endTick = iTick;
                    break;
                }

                iTick++;
            }

            if ( startPacket.IsEmpty() ) return;
            if ( endPacket.IsEmpty() ) return;

            int startTime = (int)Math.Floor( startTick * ( e.HoofdObj.TickRate * 1000 ) );
            int endTime = (int)Math.Floor( endTick * ( e.HoofdObj.TickRate * 1000 ) );

            int interval = endTime - startTime;

            //nTime -= startTime;
            //nTime /= interval;
            float amount = (float)( nTime - startTime ) / (float)interval;

            Positie = Vector3.Lerp( startPacket.Positie, endPacket.Positie, amount );

            RotatieQuat = Quaternion.Lerp( startPacket.RotatieQuat, endPacket.RotatieQuat, amount );




        }

        void IHolderElement.Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

            /*positie = Actor.getGlobalPosition();

            OnBodyChanged();*/
        }



        public virtual void OnBodyChanged()
        {
            if ( clientEntHolder != null && clientEntHolder.Node != null )
            {
                ServerClientMainOud.instance.Wereld.Tree.OrdenEntity( clientEntHolder );
            }

        }






        public virtual Vector3 Positie
        {
            get { return positie; }
            set
            {
                positie = value;
                if ( actor != null ) actor.GlobalPosition = positie;
                OnBodyChanged();
            }
        }
        public virtual Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                //TODO
                OnBodyChanged();
            }
        }
        public virtual Matrix Rotatie
        {
            get { return rotatie; }
            set
            {
                rotatie = value;
                if ( actor != null ) actor.GlobalOrientation = rotatie;
                OnBodyChanged();
            }
        }

        public NxActor Actor
        {
            get { return actor; }
            set
            {
                actor = value;
            }
        }

        public Quaternion RotatieQuat
        {
            get
            {
                return Quaternion.CreateFromRotationMatrix( rotatie );
            }
            set
            {
                Rotatie = Matrix.CreateFromQuaternion( value );
            }
        }





    }
}

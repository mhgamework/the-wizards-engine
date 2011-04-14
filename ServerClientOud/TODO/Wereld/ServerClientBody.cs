using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;
using ServerWereld = MHGameWork.TheWizards.Server.Wereld;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
    public class ServerClientBody : ServerClient.Wereld.IBody, Server.Wereld.IBody
    {
        public ServerClient.Wereld.Body clientBody;
        public ServerWereld.Body serverBody;


        public ServerClientBody()
        {
            clientBody = new Body();
            serverBody = new MHGameWork.TheWizards.Server.Wereld.Body();
        }


        //#region IClientHolderElement Members

        //void IHolderElement.SetEntityHolder(ClientEntityHolder nEntH)
        //{
        //    clientEntHolder = nEntH;
        //}

        //void IHolderElement.Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
        //{

        //}

        //void IHolderElement.Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
        //{

        //    /*positie = Actor.getGlobalPosition();

        //    OnBodyChanged();*/
        //}

        //#endregion

        //#region IServerHolderElement Members

        //void MHGameWork.TheWizards.Server.Wereld.IHolderElement.SetEntityHolder(MHGameWork.TheWizards.Server.Wereld.ServerEntityHolder nEntH)
        //{
        //    serverEntHolder = nEntH;
        //}

        //void MHGameWork.TheWizards.Server.Wereld.IHolderElement.Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
        //{

        //}

        //void MHGameWork.TheWizards.Server.Wereld.IHolderElement.Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
        //{
        //    if (actor != null)
        //    {
        //        positie = Actor.getGlobalPosition();
        //        rotatie = actor.getGlobalOrientation();

        //        OnBodyChanged();
        //    }
        //}

        //#endregion




        //public void OnBodyChanged()
        //{
        //    if (clientEntHolder != null && clientEntHolder.Node != null)
        //    {
        //        ServerClientMain.instance.Wereld.Tree.OrdenEntity(clientEntHolder);
        //    }
        //    if (serverEntHolder != null && serverEntHolder.Node != null)
        //    {
        //        ServerClientMain.instance.ServerMain.Wereld.Tree.OrdenEntity(serverEntHolder);
        //    }



        //}






        public Vector3 Positie
        {
            get { return serverBody.Positie; }
            set
            {
                clientBody.Positie = value;
                serverBody.Positie = value;
            }
        }
        public Vector3 Scale
        {
            get { return serverBody.Scale; }
            set
            {
                clientBody.Scale = value;
                serverBody.Scale = value;
            }
        }
        public Matrix Rotatie
        {
            get { return serverBody.Rotatie; }
            set
            {
                clientBody.Rotatie = value;
                serverBody.Rotatie = value;
            }
        }
        public Quaternion RotatieQuat
        { get { throw new Exception( "TODO" ); } set { throw new Exception( "TODO" ); } }


        #region ClientIHolderElement Members

        void IHolderElement.SetEntityHolder( ClientEntityHolder nEntH )
        {
            ( (IHolderElement)clientBody ).SetEntityHolder( nEntH );
        }

        void IHolderElement.Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            ( (IHolderElement)clientBody ).Process( e );
        }

        void IHolderElement.Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            ( (IHolderElement)clientBody ).Tick( e );
        }

        #endregion

        #region ServerIHolderElement Members

        void MHGameWork.TheWizards.Server.Wereld.IHolderElement.SetEntityHolder( MHGameWork.TheWizards.Server.Wereld.ServerEntityHolder nEntH )
        {
            ( (Server.Wereld.IHolderElement)serverBody ).SetEntityHolder( nEntH );
        }

        void MHGameWork.TheWizards.Server.Wereld.IHolderElement.Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            ( (Server.Wereld.IHolderElement)serverBody ).Process( e );
        }

        void MHGameWork.TheWizards.Server.Wereld.IHolderElement.Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            ( (Server.Wereld.IHolderElement)serverBody ).Tick( e );
        }

        #endregion

        #region IBody Members

        NxActor IBody.Actor
        {
            get
            {
                return clientBody.Actor;
            }
            set
            {
                clientBody.Actor = value;
            }
        }

        #endregion

        #region IBody Members

        NxActor MHGameWork.TheWizards.Server.Wereld.IBody.Actor
        {
            get
            {
                return serverBody.Actor;
            }
            set
            {
                serverBody.Actor = value;
            }
        }

        #endregion

        #region IBody Members

        public void AddEntityUpdate( int nTick, MHGameWork.TheWizards.Common.Network.UpdateEntityPacket p )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}

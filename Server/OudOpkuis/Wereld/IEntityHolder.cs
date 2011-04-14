using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Server.Wereld
{
    public interface IEntityHolder
    {

        void SetID( int nID );

        void MoveToNode( QuadTreeNode nNode );


        void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e );

        void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e );

        void EnablePhysics();
        void DisablePhysics();




        QuadTreeNode ContainingNode { get; }

        BoundingSphere BoundingSphere { get; }

        BoundingBox BoundingBox { get;  }

        bool Static { get; set;}

        int ID { get; }
    }
}

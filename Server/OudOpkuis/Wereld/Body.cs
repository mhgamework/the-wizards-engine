using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NovodexWrapper;

namespace MHGameWork.TheWizards.Server.Wereld
{
    [Obsolete]
    public class Body : IBody
    {
        private Vector3 positie;
        private Vector3 scale;
        private Matrix rotatie;

        private NxActor actor;

        public ServerEntityHolder entHolder;

        public Body()
        {
            positie = Vector3.Zero;
            scale = Vector3.One;
            rotatie = Matrix.Identity;
        }


        public void SetEntityHolder( ServerEntityHolder nEntH )
        {
            entHolder = nEntH;
        }

        public void OnBodyChanged()
        {
            if ( entHolder != null ) entHolder.OnBodyChanged();
            
        }


        public Vector3 Positie
        {
            get { return positie; }
            set
            {
                positie = value;
                if ( actor != null ) actor.GlobalPosition = positie;
                OnBodyChanged();
            }
        }
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                //TODO
                OnBodyChanged();
            }
        }
        public Matrix Rotatie
        {
            get { return rotatie; }
            set
            {
                rotatie = value;
                if ( actor != null ) actor.GlobalOrientation = rotatie;
                OnBodyChanged();
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
                rotatie = Matrix.CreateFromQuaternion( value );
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



        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {

        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

            positie = Actor.getGlobalPosition();
            rotatie = actor.getGlobalOrientation();

            OnBodyChanged();
        }


    }
}

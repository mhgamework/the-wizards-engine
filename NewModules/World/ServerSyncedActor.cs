using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.World
{
    public class ServerSyncedActor
    {
        private Vector3 positie;
        private Vector3 scale;
        private Matrix rotatie;

        private IWorldSyncActor actor;

        public ushort ID { get; set; }

        public ServerSyncedActor()
        {
            positie = Vector3.Zero;
            scale = Vector3.One;
            rotatie = Matrix.Identity;
        }


        public void OnBodyChanged()
        {
            //TODO
        }


        public Vector3 Positie
        {
            get { return positie; }
            set
            {
                positie = value;
                if (actor != null) actor.GlobalPosition = positie;
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
                if (actor != null) actor.GlobalOrientation = rotatie;
                OnBodyChanged();
            }
        }
        public Quaternion RotatieQuat
        {
            get
            {
                return Quaternion.CreateFromRotationMatrix(rotatie);
            }
            set
            {
                rotatie = Matrix.CreateFromQuaternion(value);
            }
        }

        public IWorldSyncActor Actor
        {
            get { return actor; }
            set
            {
                actor = value;
            }
        }


        public void Tick()
        {

            positie = Actor.GlobalPosition;
            rotatie = actor.GlobalOrientation;

            OnBodyChanged();
        }



    }
}

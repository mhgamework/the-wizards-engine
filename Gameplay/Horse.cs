using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Scripting;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Gameplay
{
    public class Horse : IStateScript
    {
        public Vector3 Position;

        private SphereMesh model;
        private Actor actor;

        private Vector3? targetPos;

        private Seeder seeder;




        private Vector3 movementRange = new Vector3(5, 0, 5);
        private float movementSpeed = 3;

        private bool dying;


        private int horseID;
        public static int lastHorseID = 0;

        public void Init()
        {
            model = ScriptLayer.CreateSphereModel();
            actor = ScriptLayer.CreateSphereActor(1, 200);
            actor.GlobalPosition = Position;
            model.WorldMatrix = Matrix.CreateTranslation(Position);

            actor.BodyFlags.Kinematic = true;

            seeder = new Seeder((new Random()).Next(0, 10000));

            lastHorseID++;
            actor.Name = "Horse" + lastHorseID;
            horseID = lastHorseID;
            actor.ContactReportFlags = ContactPairFlag.All;

            ScriptLayer.Physics.AddContactNotification(onContact);



        }

        private void onContact(ContactPair contactInformation, ContactPairFlag events)
        {
            if (contactInformation.ActorA == null || contactInformation.ActorB == null) return;
            if (contactInformation.ActorA.Name == null || contactInformation.ActorB.Name == null) return;
            if (!(contactInformation.ActorA.Name.StartsWith("Horse" + horseID) && contactInformation.ActorB.Name.StartsWith("EnergyOrb"))
                && !(contactInformation.ActorB.Name.StartsWith("Horse" + horseID) && contactInformation.ActorA.Name.StartsWith("EnergyOrb"))) return;

            dying = true;


        }

        public void Destroy()
        {
        }

        public void Update()
        {
            if (dying && actor != null)
            {
                //actor.Dispose();
                actor = null;
                model.WorldMatrix = new Matrix();
                dying = false;
            }
            if (actor == null) return;
            if (targetPos.HasValue)
            {
                var dir = targetPos.Value - Position;
                var dist = movementSpeed * ScriptLayer.Elapsed;
                var l = dir.Length();
                if (l < dist)
                {
                    dist = l;
                    targetPos = null;
                }

                dir /= l;

                Position += dir * dist;

                actor.MoveGlobalPositionTo(Position);


            }

            Position = actor.GlobalPosition;

            model.WorldMatrix = Matrix.CreateTranslation(Position);
            int a = 5;

            if (targetPos.HasValue == false && attemptWalk())
            {
                targetPos = seeder.NextVector3(Position - movementRange, Position + movementRange);
            }

        }

        private bool attemptWalk()
        {
            var lambda = 2;
            var t = ScriptLayer.Elapsed;
            var Ft = 1 - Math.Exp(-lambda * t);

            var chance = seeder.NextFloat(0, 1);

            if (chance < Ft)
                return true; // Run Horsie!!


            return false; // Lazy mode...
        }

        public void Draw()
        {
        }
    }
}

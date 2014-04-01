﻿using System;
using DirectX11;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Enemy : IIslandAddon
    {
        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {
            update();
        }

        public EnemyState CurrentState = EnemyState.INACTIVE;
        private Level level;
        private ScatteredPlayer player;

        private const float turnSpeed = (float)Math.PI * 2f;
        private const float moveSpeed = 5f;

        private Vector3 currentPos;
        private Vector3 currentRotation; //xrot, yrot, zrot
        private Vector3 newPos;
        private Vector3 newRotation;

        private Vector3 startLocation;

        private float currentHeight;
        private const float idleHeight = 1.2f; //heightdifference with island when starting powerup
        private const float patrolHeight = 7.5f; //heightdifference with island when patrolling
        private const float attackHeight = 5f; //heightdifference with player when attacking

        public Enemy(Level level, SceneGraphNode node, Vector3 relativeStartPos)
        {
            Node = node;
            node.AssociatedObject = this;

            this.level = level;
            player = level.LocalPlayer;

            var ent = level.CreateEntityNode(node.CreateChild());
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\EnemyRobot");
            ent.Node.Relative = Matrix.Scaling(2, 2, 2);

            Node.Relative = Matrix.Translation(relativeStartPos + new Vector3(0, idleHeight, 0));
            currentHeight = idleHeight;

            Vector3 s;
            Quaternion r;
            Node.Absolute.Decompose(out s, out r, out currentPos);
            currentRotation = new Vector3();

            newPos = currentPos;
            newRotation = currentRotation;

            startLocation = currentPos + new Vector3(0, patrolHeight - idleHeight, 0);
        }

        public void Activate()
        {
            CurrentState = EnemyState.POWERUP;
        }

        public void Destroy()
        {
            CurrentState = EnemyState.DEAD;
        }

        private void update()
        {
            newPos = currentPos;
            currentRotation = moduloRotation(currentRotation);
            newRotation = currentRotation;

            updateBehaviour();

            Node.Absolute = Matrix.RotationX(newRotation.X) * Matrix.RotationZ(newRotation.Z) * Matrix.RotationY(newRotation.Y) * Matrix.Translation(newPos);
            currentPos = newPos;
            currentRotation = newRotation;
        }

        private Vector3 moduloRotation(Vector3 rot)
        {
            const float pi = (float)Math.PI;
            return new Vector3(rot.X % (2 * pi), rot.Y % (2 * pi), rot.Z % (2 * pi));
        }

        private void updateBehaviour()
        {
            switch (CurrentState)
            {
                case EnemyState.INACTIVE:
                    break;
                case EnemyState.POWERUP:
                    updatePowerup();
                    break;
                case EnemyState.PATROL:
                    updatePatrol();
                    break;
                case EnemyState.ENGAGE:
                    updateEngage();
                    break;
                case EnemyState.ATTACK:
                    updateAttack();
                    break;
                case EnemyState.DISENGAGE:
                    updateDisengage();
                    break;
                case EnemyState.DEAD:
                    updateDead();
                    break;
            }
        }

        private float powerupRiseSpeed;
        private float powerupRotationSpeed;
        private float powerupIdleTime = 3f;
        private void updatePowerup()
        {
            if (powerupIdleTime > 0)
            {
                powerupIdleTime -= TW.Graphics.Elapsed;
                return;
            }

            if (powerupRiseSpeed < 10f)
                powerupRiseSpeed += TW.Graphics.Elapsed * 5f;

            newRotation += new Vector3(0, powerupRotationSpeed * TW.Graphics.Elapsed, 0);

            while (currentHeight < patrolHeight - 0.2f)
            {
                if (powerupRotationSpeed < 3f)
                    powerupRotationSpeed += TW.Graphics.Elapsed * 10f;

                var heightDiff = (MathHelper.Lerp(currentHeight, patrolHeight, 0.2f) - currentHeight) * TW.Graphics.Elapsed * powerupRiseSpeed;
                currentHeight += heightDiff;
                newPos += new Vector3(0, heightDiff, 0);
                return;
            }

            while (powerupRotationSpeed > 0)
            {
                powerupRotationSpeed -= TW.Graphics.Elapsed * 1f;
                return;
            }

            CurrentState = EnemyState.PATROL;
        }

        private const float patrolDetectionRange = 50f;
        private void updatePatrol()
        {
            if (Vector3.Distance(player.Position, currentPos) < patrolDetectionRange)
                CurrentState = EnemyState.ENGAGE;
        }

        private const float engageRange = 50f;
        private void updateEngage()
        {
            rotateYToPosition(player.Position);
            moveTo(player.Position + new Vector3(0, attackHeight, 0), moveSpeed);

            if (Vector3.Distance(player.Position, currentPos) > engageRange)
                CurrentState = EnemyState.DISENGAGE;

            if (Vector3.Distance(player.Position, currentPos) < attackRange)
            {
                CurrentState = EnemyState.ATTACK;
                attackPos = currentPos;
            }

        }

        private const float attackRange = 30f;
        private const float dodgeSpeed = 15f;
        private Vector3 dodgePos;
        private Vector3 attackPos;
        private float dodgeTimeout;
        private void updateAttack()
        {
            if (dodgeTimeout > 0)
            {
                dodgeTimeout -= TW.Graphics.Elapsed;
            }
            else
            {
                if (dodgePos == new Vector3(0, 0, 0))
                    setNewDodgePos();

                if (Vector3.Distance(dodgePos, currentPos) < 0.5f)
                {
                    setNewDodgePos();
                    dodgeTimeout = rnd.Next(1, 4);
                    //todo: shoot bullet
                }

                dodgeTo(dodgePos, dodgeSpeed);
            }

            rotateYToPosition(player.Position);

            if (Vector3.Distance(player.Position, currentPos) > attackRange)
            {
                dodgePos = new Vector3(0, 0, 0);
                dodgeTimeout = 0f;
                CurrentState = EnemyState.ENGAGE;
            }
        }

        private Random rnd = new Random();
        private void setNewDodgePos()
        {
            var dir = player.Position - currentPos;
            dir.Normalize();

            var right = Vector3.Cross(dir, Vector3.UnitY);
            right.Normalize();

            var tangent = Vector3.Cross(dir, right);

            dodgePos = attackPos + right * rnd.Next(-5, 5) + tangent * rnd.Next(-5, 5) + dir * rnd.Next(1, 3);
        }

        private void dodgeTo(Vector3 pos, float speed)
        {
            var lerpAmnt = 0.2f;
            var xInc = (MathHelper.Lerp(currentPos.X, pos.X, lerpAmnt) - currentPos.X) * speed * TW.Graphics.Elapsed;
            var yInc = (MathHelper.Lerp(currentPos.Y, pos.Y, lerpAmnt) - currentPos.Y) * speed * TW.Graphics.Elapsed;
            var zInc = (MathHelper.Lerp(currentPos.Z, pos.Z, lerpAmnt) - currentPos.Z) * speed * TW.Graphics.Elapsed;
            newPos += new Vector3(xInc, yInc, zInc);
        }

        private void rotateYToPosition(Vector3 pos)
        {
            var desiredDir = pos + new Vector3(0, patrolHeight, 0) - currentPos;
            desiredDir = new Vector3(desiredDir.X, 0, desiredDir.Z);
            desiredDir.Normalize();
            var desiredYRot = (float)Math.Acos(desiredDir.Z);
            desiredYRot = Math.Asin(desiredDir.X) > 0 ? desiredYRot - (float)Math.PI * 0.5f : -desiredYRot - (float)Math.PI * 0.5f;


            var rotIncrement01 = (MathHelper.Lerp(newRotation.Y, desiredYRot, 0.5f) - newRotation.Y) * TW.Graphics.Elapsed * turnSpeed;
            var rotIncrement02 = (MathHelper.Lerp(newRotation.Y, desiredYRot - (float)Math.PI * 2, 0.5f) - newRotation.Y) * TW.Graphics.Elapsed * turnSpeed;
            var rotIncrement = Math.Abs(rotIncrement01) < Math.Abs(rotIncrement02) ? rotIncrement01 : rotIncrement02;

            newRotation = new Vector3(newRotation.X, newRotation.Y + rotIncrement, newRotation.Z);
        }

        private void moveTo(Vector3 pos, float speed)
        {
            var distance = Vector3.Distance(pos, currentPos);
            if (distance < speed * TW.Graphics.Elapsed)
            {
                newPos = pos;
                return;
            }

            var dir = pos - currentPos;
            dir.Normalize();
            var posIncr = dir * speed * TW.Graphics.Elapsed;
            newPos += posIncr;
        }

        private void updateDisengage()
        {
            moveTo(startLocation, moveSpeed);
            rotateYToPosition(startLocation);

            if (Vector3.Distance(startLocation, currentPos) < 0.1f)
                CurrentState = EnemyState.PATROL;
        }

        private void updateDead()
        {
            //dying animation, cleanup
        }


        public enum EnemyState
        {
            INACTIVE, POWERUP, PATROL, ENGAGE, ATTACK, DISENGAGE, DEAD
        }
    }
}
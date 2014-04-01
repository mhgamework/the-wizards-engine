using System;
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

        private Vector3 currentPos;
        private Vector3 currentRotation; //xrot, yrot, zrot
        private Vector3 newPos;
        private Vector3 newRotation;

        private Vector3 startLocation;

        private float currentHeight;
        private const float idleHeight = 1.2f;
        private const float flyHeight = 10f;

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

            startLocation = currentPos + new Vector3(0, flyHeight - idleHeight, 0);
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
            updateBehaviour();

            Node.Absolute = Matrix.RotationX(currentRotation.X) * Matrix.RotationZ(currentRotation.Z) * Matrix.RotationY(currentRotation.Y) * Matrix.Translation(newPos);
            currentPos = newPos;
            currentRotation = newRotation;
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

            while (currentHeight < flyHeight - 0.2f)
            {
                if (powerupRotationSpeed < 3f)
                    powerupRotationSpeed += TW.Graphics.Elapsed * 10f;

                var heightDiff = (MathHelper.Lerp(currentHeight, flyHeight, 0.2f) - currentHeight) * TW.Graphics.Elapsed * powerupRiseSpeed;
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

        private const float patrolDetectionRange = 20f;
        private void updatePatrol()
        {
            if (Vector3.Distance(player.Position, currentPos) < patrolDetectionRange)
                CurrentState = EnemyState.ENGAGE;
        }

        private const float engageRange = 40f;
        private void updateEngage()
        {
            if (Vector3.Distance(player.Position, currentPos) > engageRange)
                CurrentState = EnemyState.DISENGAGE;

            if (Vector3.Distance(player.Position, currentPos) < attackRange)
                CurrentState = EnemyState.ATTACK;

            var desiredDir = player.Position + new Vector3(0, flyHeight, 0) - currentPos;
            desiredDir = new Vector3(desiredDir.X, 0, desiredDir.Z);
            desiredDir.Normalize();
            var desiredYRot = (float)Math.Acos(desiredDir.Z);
            desiredYRot = Math.Asin(desiredDir.X) > 0 ? desiredYRot - (float)Math.PI * 0.5f : -desiredYRot - (float)Math.PI * 0.5f;

            newRotation = new Vector3(newRotation.X, desiredYRot, newRotation.Z);
        }

        private const float attackRange = 20f;
        private void updateAttack()
        {
            if (Vector3.Distance(player.Position, currentPos) > attackRange)
                CurrentState = EnemyState.ENGAGE;
        }

        private void updateDisengage()
        {
            if (Vector3.Distance(startLocation, currentPos) < 1f)
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